using Core.Models.Tenant;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace API.Abstraction.Utilities
{
    public interface IAttachmentService
    {
        Task<AttachmentDataModel> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<(Stream Stream, string ContentType, string FileName)?> GetFileAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAttachmentAsync(int id, CancellationToken cancellationToken = default);
    }
}
