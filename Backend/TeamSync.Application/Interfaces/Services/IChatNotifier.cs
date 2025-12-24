namespace TeamSync.Application.Interfaces.Services
{
    public interface IChatNotifier
    {
        Task MessageCreatedAsync(
            string projectId,
            object messageDto
        );
    }
}
