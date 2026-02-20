using BlazX;
using FakeID.Client.Blazor;
using FakeID.Client.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STrain;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient();
builder.UseLightinject();

builder.Services
    .AddBxDialog()
    .AddBxLoader();

builder.Services.AddSingleton<IClientDataService, ClientDataService>();
builder.Services.AddSingleton<IPersonaDataService, PersonaDataService>();

builder.UseRequestRouter(_ => "backend")
    .AddGenericHttpSender("backend", (options, configuration) => configuration.Bind("Router:Backend", options));

var app = builder.Build();
await app.RunAsync();
