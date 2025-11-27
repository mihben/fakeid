using FakeID.Client.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STrain;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient();
builder.UseLightinject();

builder.UseRequestRouter(_ => "backend")
    .AddGenericHttpSender("backend", (options, _) => { options.BaseAddress = new Uri("http://localhost:5100/"); options.Path = "api"; });

await builder.Build().RunAsync();
