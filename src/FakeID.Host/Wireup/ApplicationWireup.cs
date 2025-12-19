using FakeID.Application.Contexts;
using FakeID.Application.Options;
using FakeID.Application.Services;
using FakeID.Host.HostServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;

namespace FakeID.Host.Wireup
{
    public static class ApplicationWireup
    {
        public static void AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<DatabaseInitializer>();

            builder.Services.AddDbContext<ApplicationContext>(builder => builder.UseNpgsql("Host=mihben.space;Port=5432;Database=fakeid_dev;user id=admin;password=admin"));

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddTransient<IOauthService, OAuthService>();

            builder.Services.AddTransient<ISystemClock, SystemClock>();

            builder.Services.AddOptions<ApplicationOptions>()
                .BindConfiguration("Application")
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}
