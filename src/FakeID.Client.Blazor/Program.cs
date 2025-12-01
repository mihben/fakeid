using FakeID.Client.Blazor;
using FakeID.Client.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using STrain;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient();
builder.UseLightinject();

builder.Services.AddFluentUIComponents();

builder.Services.AddSingleton<IClientDataService, ClientDataService>();

builder.UseRequestRouter(_ => "backend")
    .AddGenericHttpSender("backend", (options, _) => { options.BaseAddress = new Uri("http://localhost:5000/"); options.Path = "api"; });

await builder.Build().RunAsync();
