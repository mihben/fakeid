using FakeID.Api;
using FakeID.Application.Contexts;
using FakeID.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using STrain;

namespace FakeID.Application.Performers
{
    public class PersonaPerformers : IQueryPerformer<GetPersonasQuery, IEnumerable<GetPersonasQuery.Result>>,
        ICommandPerformer<CreatePersonaCommand>,
        ICommandPerformer<UpdatePersonaCommand>,
        ICommandPerformer<DeletePersonaCommand>,
        IQueryPerformer<GetPersonasByClientQuery, IEnumerable<GetPersonasByClientQuery.Result>>
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<PersonaPerformers> _logger;

        public PersonaPerformers(ApplicationContext context, ILogger<PersonaPerformers> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GetPersonasQuery.Result>> PerformAsync(GetPersonasQuery query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Querying personas of client {Client}", query.Client);

            var client = await _context.Personas.Where(p => p.Client.Id == query.Client).ToListAsync(cancellationToken);

            return client.ConvertAll(c => new GetPersonasQuery.Result
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Role = c.Role
            });
        }

        public async Task PerformAsync(CreatePersonaCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Creating {Persona} persona for {Client} client", command.Id, command.Client);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var client = await _context.Clients.Include(c => c.Personas).SingleOrDefaultAsync(c => c.Id == command.Client, cancellationToken).ConfigureAwait(false);
                if (client is null) throw Exceptions.Clients.NotFound;
                if (client.Personas.Any(p => p.Username == command.Username)) throw Exceptions.Personas.Conflict(command.Username);

                await _context.Personas.AddAsync(new PersonaEntity
                {
                    Client = client,
                    Id = command.Id,
                    Username = command.Username,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                    Role = command.Role
                }, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public async Task PerformAsync(UpdatePersonaCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Updating {Persona} persona", command.Persona, cancellationToken);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var client = await _context.Clients.Include(c => c.Personas).SingleOrDefaultAsync(c => c.Id == command.Client, cancellationToken).ConfigureAwait(false);
                if (client is null) throw Exceptions.Clients.NotFound;
                if (client.Personas.Any(p => p.Id != command.Persona && p.Username == command.Username)) throw Exceptions.Personas.Conflict(command.Username);

                var persona = client.Personas.SingleOrDefault(p => p.Id == command.Persona);
                if (persona is null) throw Exceptions.Personas.NotFound;

                persona.Username = command.Username;
                persona.FirstName = command.FirstName;
                persona.LastName = command.LastName;
                persona.Email = command.Email;
                persona.Role = command.Role;

                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public async Task PerformAsync(DeletePersonaCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Deleting {Persona} persona", command.Persona, cancellationToken);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var persona = await _context.Personas.FindAsync([command.Persona], cancellationToken).ConfigureAwait(false);
                if (persona is null)
                {
                    _logger.LogDebug("{Persona} persona was not found", command.Persona, cancellationToken);
                    return;
                }

                _context.Personas.Remove(persona);

                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public async Task<IEnumerable<GetPersonasByClientQuery.Result>> PerformAsync(GetPersonasByClientQuery query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting personas for {Client} client", query.Client);

            var personas = await _context.Personas.Where(p => p.Client.Name == query.Client).ToListAsync().ConfigureAwait(false);

            return personas.ConvertAll(p => new GetPersonasByClientQuery.Result
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Role = p.Role
            });
        }
    }
}
