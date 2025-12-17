
using FakeID.Application.Contexts;

namespace FakeID.Host.HostServices
{
    public class DatabaseInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _factory;

        public DatabaseInitializer(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _factory.CreateScope();

            await scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database.EnsureCreatedAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
