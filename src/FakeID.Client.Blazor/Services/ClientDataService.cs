
using FakeID.Api;
using STrain;

namespace FakeID.Client.Blazor.Services
{
    public interface IClientDataService
    {
        IEnumerable<Models.Client> Clients { get; }

        Task LoadAsync(CancellationToken cancellationToken);
        Task SaveAsync(Models.Client client, CancellationToken cancellationToken);
    }

    public class ClientDataService : IClientDataService
    {
        private readonly IRequestSender _sender;

        public IEnumerable<Models.Client> Clients { get; private set; } = Enumerable.Empty<Models.Client>();

        public ClientDataService(IRequestSender sender)
        {
            _sender = sender;
        }

        public async Task LoadAsync(CancellationToken cancellationToken)
        {
            var result = await _sender.GetAsync<GetClientsQuery, IEnumerable<GetClientsQuery.Result>>(new GetClientsQuery(), cancellationToken);
            if (result == null) return;

            Clients = result.Select(r => new Models.Client
            {
                Id = r.Id,
                Name = r.Name,
                Selected = false
            });
        }

        public async Task SaveAsync(Models.Client client, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(new CreateClientCommand { Id = client.Id, Name = client.Name! }, cancellationToken);
        }
    }
}
