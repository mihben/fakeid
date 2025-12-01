using FakeID.Api;
using FakeID.Application.Contexts;
using FakeID.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using STrain;

namespace FakeID.Application.Handlers
{
    public class ClientPerformers : ICommandPerformer<CreateClientCommand>
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<ClientPerformers> _logger;

        public ClientPerformers(ApplicationContext context, ILogger<ClientPerformers> logger)
        {
            _context = context;
            _logger = logger;
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
    }
}
