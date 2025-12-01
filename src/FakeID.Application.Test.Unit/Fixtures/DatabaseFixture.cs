using FakeID.Application.Contexts;
using FakeID.Application.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FakeID.Application.Test.Unit.Fixtures
{
    public class DatabaseFixture
    {
        private const string connectionString = "Data Source=:memory:";
        private SqliteConnection? _connection = null;

        public void OpenConnection()
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        public ApplicationContext CreateContext()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>()
                .UseSqlite(_connection!);

            var result = new ApplicationContext(builder.Options);
            result.Database.EnsureCreated();

            return result;
        }

        public void CloseConnection()
        {
            _connection?.Close();
        }

        public async Task<IEnumerable<ClientEntity>> GetClientsAsync()
        {
            using var context = CreateContext();

            return await context.Clients.ToListAsync();
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
