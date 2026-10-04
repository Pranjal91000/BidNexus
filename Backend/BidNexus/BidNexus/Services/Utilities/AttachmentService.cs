using API.Abstraction.Utilities;
using Core.Abstraction.Services;
using Core.Abstraction.Utilities;
using Core.Entities.Utilities;
using Core.Exceptions;
using Core.Models.Tenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services.Utilities
{
    public class AttachmentService(
        IAttachmentRepository attachmentRepository,
        IJwtHelperService jwtHelper,
        IWebHostEnvironment environment) : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository = attachmentRepository;
        private readonly IJwtHelperService _jwtHelper = jwtHelper;
        private readonly IWebHostEnvironment _environment = environment;

        private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".webp", ".svg", ".gif"];
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        private string GetUploadsDirectory()
        {
            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsPath = Path.Combine(webRoot, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            return uploadsPath;
        }

        public async Task<AttachmentDataModel> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("No file was uploaded.");

            if (file.Length > MaxFileSizeBytes)
                throw new ValidationException($"File size exceeds maximum allowed limit of {MaxFileSizeBytes / (1024 * 1024)}MB.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new ValidationException($"Unsupported file type '{ext}'. Allowed extensions: {string.Join(", ", AllowedExtensions)}.");

            var tenantId = _jwtHelper.GetTenantId();
            var storedFileName = $"{Guid.NewGuid():N}{ext}";
            var uploadsDir = GetUploadsDirectory();
            var physicalPath = Path.Combine(uploadsDir, storedFileName);

            await using (var fileStream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            var relativePath = Path.Combine("uploads", storedFileName).Replace('\\', '/');

            var attachment = new Attachment
            {
                TenantId = tenantId,
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = storedFileName,
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                FileSize = file.Length,
                RelativePath = relativePath,
                CreatedDateTime = DateTimeOffset.UtcNow
            };

            var saved = await _attachmentRepository.AddAsync(attachment, cancellationToken);

            return new AttachmentDataModel
            {
                Id = saved.Id,
                TenantId = saved.TenantId,
                OriginalFileName = saved.OriginalFileName,
                ContentType = saved.ContentType,
                FileSize = saved.FileSize,
                RelativePath = saved.RelativePath,
                Url = $"/api/attachments/{saved.Id}"
            };
        }

        public async Task<(Stream Stream, string ContentType, string FileName)?> GetFileAsync(int id, CancellationToken cancellationToken = default)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id, cancellationToken);
            if (attachment == null) return null;

            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var physicalPath = Path.Combine(webRoot, attachment.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(physicalPath)) return null;

            var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, attachment.ContentType, attachment.OriginalFileName);
        }

        public async Task<bool> DeleteAttachmentAsync(int id, CancellationToken cancellationToken = default)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id, cancellationToken);
            if (attachment == null) return false;

            var webRoot = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var physicalPath = Path.Combine(webRoot, attachment.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(physicalPath))
            {
                try
                {
                    File.Delete(physicalPath);
                }
                catch
                {
                    // Ignore disk lock/concurrency issues during file removal
                }
            }

            return await _attachmentRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
