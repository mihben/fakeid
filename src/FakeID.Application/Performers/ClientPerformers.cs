using FakeID.Api;
using FakeID.Application.Contexts;
using FakeID.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using STrain;

namespace FakeID.Application.Handlers
{
    public class ClientPerformers : IQueryPerformer<GetClientsQuery, IEnumerable<GetClientsQuery.Result>>,
        ICommandPerformer<CreateClientCommand>,
        ICommandPerformer<UpdateClientCommand>,
        ICommandPerformer<DeleteClientCommand>
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ClientPerformers> _logger;

        public ClientPerformers(ApplicationContext context, ILogger<ClientPerformers> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<IEnumerable<GetClientsQuery.Result>> PerformAsync(GetClientsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Querying clients");
            var entities = await _context.Clients.ToListAsync(cancellationToken);

            return entities.ConvertAll(e => new GetClientsQuery.Result { Id = e.Id, Name = e.Name });
        }

        public async Task PerformAsync(CreateClientCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Creating {Client} client", command.Id);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (await _context.Clients.AnyAsync(c => c.Name == command.Name, cancellationToken).ConfigureAwait(false)) throw Exceptions.Clients.AlreadyExist(command.Name);

                await _context.Clients.AddAsync(new ClientEntity { Id = command.Id, Name = command.Name }, cancellationToken).ConfigureAwait(false);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public async Task PerformAsync(UpdateClientCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Updating {Client} client", command.Id);

            if (await _context.Clients.AnyAsync(c => c.Name == command.Name)) throw Exceptions.Clients.AlreadyExist(command.Name);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var entity = await _context.Clients.FindAsync([command.Id], cancellationToken: cancellationToken).ConfigureAwait(false);

                if (entity is null) throw Exceptions.Clients.NotFound;

                entity.Name = command.Name;

                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }

        public async Task PerformAsync(DeleteClientCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Deleting {Client} client", command.Id);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                var entity = await _context.Clients.FindAsync([command.Id], cancellationToken: cancellationToken).ConfigureAwait(false);
                if (entity is null)
                {
                    _logger.LogDebug("{Client} client was not found", command.Id);
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    return;
                }

                _context.Remove(entity);

                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
