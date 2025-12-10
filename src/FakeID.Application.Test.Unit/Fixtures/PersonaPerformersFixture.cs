using FakeID.Application.Entities;
using FakeID.Application.Performers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FakeID.Application.Test.Unit.Fixtures
{
    public class PersonaPerformersFixture : DatabaseFixture
    {
        private readonly ILogger<PersonaPerformers> _logger;

        public PersonaPerformersFixture()
        {
            _logger = LoggerFactory.Create(builder => builder
                                             .AddXUnit()
                                             .SetMinimumLevel(LogLevel.Trace))
                                     .CreateLogger<PersonaPerformers>();
        }

        public PersonaPerformers CreateSUT()
        {
            OpenConnection();

            return new PersonaPerformers(CreateContext(), _logger);
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public async Task<IEnumerable<PersonaEntity>> GetAsync(Guid client)
        {
            await using var context = CreateContext();

            return await context.Personas.Where(p => p.Client.Id == client).ToListAsync();
        }

        public async Task InsertAsync(PersonaEntity persona)
        {
            await using var context = CreateContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await context.Clients.AddAsync(persona.Client);
                await context.Personas.AddAsync(persona);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task InsertAsync(ClientEntity client)
        {
            await using var context = CreateContext();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await context.Clients.AddAsync(client);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
