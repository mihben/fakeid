using FakeID.Api;
using FakeID.Client.Blazor.Models;
using STrain;

namespace FakeID.Client.Blazor.Services
{
    public interface IPersonaDataService
    {
        IEnumerable<Models.Persona> Personas { get; }

        Task LoadAsync(Guid client, CancellationToken cancellationToken);
    }

    public class PersonaDataService : IPersonaDataService
    {
        private readonly IRequestSender _sender;
        public IEnumerable<Persona> Personas { get; private set; } = [];

        public PersonaDataService(IRequestSender sender)
        {
            _sender = sender;
        }

        public async Task LoadAsync(Guid client, CancellationToken cancellationToken)
        {
            var result = await _sender.GetAsync<GetPersonasQuery, IEnumerable<GetPersonasQuery.Result>>(new GetPersonasQuery(client), cancellationToken).ConfigureAwait(false);

            Personas = [.. result!.Select(r => new Models.Persona
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Role = r.Role,
                Email = r.Email
            })];
        }
    }
}
