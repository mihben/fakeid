namespace FakeID.Application.Services
{
    public interface IOauthService
    {
        Task AuthorizeAsync(CancellationToken cancellationToken);
    }
}
