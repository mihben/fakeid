using FakeID.Application.Handlers;
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
    }
}
