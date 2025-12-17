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
    .AddGenericHttpSender("backend", (options, _) => { options.BaseAddress = new Uri("http://localhost:5000/"); options.Path = "api"; });

var app = builder.Build();
await app.RunAsync();
