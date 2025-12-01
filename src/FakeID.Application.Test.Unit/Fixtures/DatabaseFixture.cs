using FakeID.Application.Contexts;
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
    }
}
