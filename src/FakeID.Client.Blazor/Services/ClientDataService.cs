
using FakeID.Api;
using STrain;

namespace FakeID.Client.Blazor.Services
{
    public interface IClientDataService
    {
        Task SaveAsync(Models.Client client, CancellationToken cancellationToken);
    }

    public class ClientDataService : IClientDataService
    {
        private readonly IRequestSender _sender;

        public ClientDataService(IRequestSender sender)
        {
            _sender = sender;
        }

        public async Task SaveAsync(Models.Client client, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(new CreateClientCommand { Id = client.Id, Name = client.Name! }, cancellationToken);
        }
    }
}
