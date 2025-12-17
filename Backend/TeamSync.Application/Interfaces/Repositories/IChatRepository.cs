using TeamSync.Domain.Entities;

namespace TeamSync.Application.Interfaces.Repositories
{
    public interface IChatRepository: IRepository<ChatMessage>
    {
        Task<List<ChatMessage>> GetByProjectIdAsync(string projectId);
    }
}
