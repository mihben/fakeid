
using FakeID.Api;
using Microsoft.AspNetCore.Components;
using STrain;

namespace FakeID.Client.Blazor.Services
{
    public interface IClientDataService
    {
        IEnumerable<Models.Client> Clients { get; }
        Models.Client? Selected { get; set; }

        event Func<object?, Models.Client, Task> SelectionChanged;

        Task LoadAsync(CancellationToken cancellationToken);
        Task SaveAsync(Models.Client client, CancellationToken cancellationToken);
        Task DeleteAsync(CancellationToken cancellationToken);
    }

    public class ClientDataService : IClientDataService
    {
        private readonly IRequestSender _sender;

        public IEnumerable<Models.Client> Clients { get; private set; } = [];

        private Models.Client? _selected;
        public Models.Client? Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
                NotifySelectionChanged(_selected);
            }
        }

        public event Func<object?, Models.Client, Task> SelectionChanged;

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
                Name = r.Name
            });
        }

        public async Task SaveAsync(Models.Client client, CancellationToken cancellationToken)
        {
            if (Clients.Any(c => c.Id == client.Id)) await _sender.SendAsync(new UpdateClientCommand { Id = client.Id, Name = client.Name! }, cancellationToken);
            else await _sender.SendAsync(new CreateClientCommand { Id = client.Id, Name = client.Name! }, cancellationToken);

            await LoadAsync(cancellationToken);

            Selected = null;
            Selected = Clients.SingleOrDefault(c => c.Id.Equals(client.Id));
        }

        public async Task DeleteAsync(CancellationToken cancellationToken)
        {
            if (Selected is null) return;

            await _sender.SendAsync(new DeleteClientCommand { Id = Selected.Id }, cancellationToken);
            await LoadAsync(cancellationToken);

            Selected = null;
        }

        private void NotifySelectionChanged(Models.Client? client)
        {
            SelectionChanged?.Invoke(this, client);
        }
    }
}
