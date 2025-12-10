using FakeID.Application.Contexts;
using FakeID.Host.HostServices;
using Microsoft.EntityFrameworkCore;

namespace FakeID.Host.Wireup
{
    public static class ApplicationWireup
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<DatabaseInitializer>();

            builder.Services.AddDbContext<ApplicationContext>(builder => builder.UseNpgsql("Host=mihben.space;Port=5432;Database=fakeid_dev;user id=admin;password=admin"));
        }
    }
}
