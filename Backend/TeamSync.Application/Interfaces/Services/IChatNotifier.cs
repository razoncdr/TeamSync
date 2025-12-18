namespace TeamSync.Application.Interfaces.Services
{
    public interface IChatNotifier
    {
        Task NotifyMessageCreatedAsync(
            string projectId,
            object messageDto
        );
    }
}
