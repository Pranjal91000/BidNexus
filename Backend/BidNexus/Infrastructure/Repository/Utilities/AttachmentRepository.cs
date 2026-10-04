using Core.Abstraction.Utilities;
using Core.Entities.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Utilities
{
    public class AttachmentRepository(AppDbContext dbContext) : IAttachmentRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<Attachment> AddAsync(Attachment attachment, CancellationToken cancellationToken = default)
        {
            await _dbContext.Attachments.AddAsync(attachment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return attachment;
        }

        public async Task<Attachment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Attachments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var attachment = await _dbContext.Attachments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (attachment == null) return false;

            _dbContext.Attachments.Remove(attachment);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
