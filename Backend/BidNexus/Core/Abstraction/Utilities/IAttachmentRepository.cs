using Core.Entities.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Abstraction.Utilities
{
    public interface IAttachmentRepository
    {
        Task<Attachment> AddAsync(Attachment attachment, CancellationToken cancellationToken = default);
        Task<Attachment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
