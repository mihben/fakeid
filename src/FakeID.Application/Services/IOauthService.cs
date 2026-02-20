namespace FakeID.Application.Services
{
    public interface IOauthService
    {
        Task GetMetadataAsync(CancellationToken cancellationToken);
        Task AuthorizeAsync(CancellationToken cancellationToken);
    }
}
