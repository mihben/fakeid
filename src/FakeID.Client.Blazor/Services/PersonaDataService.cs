using FakeID.Api;
using FakeID.Client.Blazor.Models;
using STrain;

namespace FakeID.Client.Blazor.Services
{
    public interface IPersonaDataService
    {
        IEnumerable<Models.Persona> Personas { get; }

        Task LoadAsync(Guid client, CancellationToken cancellationToken);
        void AddPersona();
        Task SaveAsync(Guid client, Persona persona, CancellationToken cancellationToken);
        Task DeleteAsync(Guid client, Persona persona, CancellationToken cancellationToken);
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
            Personas = [];

            var result = await _sender.GetAsync<GetPersonasQuery, IEnumerable<GetPersonasQuery.Result>>(new GetPersonasQuery(client), cancellationToken).ConfigureAwait(false);

            Personas = [.. result!.Select(r => new Models.Persona
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Role = r.Role,
                Email = r.Email,
                IsNew = false
            })];
        }

        public void AddPersona()
        {
            Personas = [.. Personas, new Persona { Id = Guid.NewGuid(), IsNew = true }];
        }

        public async Task SaveAsync(Guid client, Persona persona, CancellationToken cancellationToken)
        {
            if (persona.IsNew)
            {
                await _sender.SendAsync(new CreatePersonaCommand
                {
                    Client = client,
                    Id = persona.Id,
                    FirstName = persona.FirstName!,
                    LastName = persona.LastName!,
                    Email = persona.Email!,
                    Role = persona.Role!
                }, cancellationToken);
            }
            else
            {
                await _sender.SendAsync(new UpdatePersonaCommand
                {
                    Client = client,
                    Persona = persona.Id,
                    FirstName = persona.FirstName!,
                    LastName = persona.LastName!,
                    Email = persona.Email!,
                    Role = persona.Role!
                }, cancellationToken);
            }

            await LoadAsync(client, cancellationToken);
        }

        public async Task DeleteAsync(Guid client, Persona persona, CancellationToken cancellationToken)
        {
            Personas = [];

            await _sender.SendAsync(new DeletePersonaCommand
            {
                Client = client,
                Persona = persona.Id
            }, cancellationToken);
            await LoadAsync(client, cancellationToken);
        }
    }
}
