using FakeID.Application.Entities;
using FakeID.Application.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FakeID.Application.Test.Unit.Fixtures
{
    public class ClientPerformersFixture : DatabaseFixture, IDisposable
    {
        private readonly ILogger<ClientPerformers> _logger;

        public ClientPerformersFixture()
        {
            _logger = LoggerFactory.Create(builder => builder
                                             .AddXUnit()
                                             .SetMinimumLevel(LogLevel.Trace))
                                     .CreateLogger<ClientPerformers>();
        }

        public ClientPerformers CreateSUT()
        {
            OpenConnection();

            return new ClientPerformers(CreateContext(), _logger);
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public async Task<IEnumerable<ClientEntity>> GetAsync()
        {
            using var context = CreateContext();

            return await context.Clients.ToListAsync();
        }

        public async Task<ClientEntity?> GetAsync(Guid id)
        {
            using var context = CreateContext();

            return await context.Clients.FindAsync(id);
        }

        public async Task InsertAsync(ClientEntity client)
        {
            var context = CreateContext();

            await using var transation = await context.Database.BeginTransactionAsync();

            await context.AddAsync(client);
            await context.SaveChangesAsync();

            await transation.CommitAsync();
        }
    }
}
