using FakeID.Host.Wireup;
using Serilog;
using STrain.CQS.NetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Host.UseLightInject();

builder.Logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger());

builder.Services.AddExceptionHandler().UseDefaultWriters();

builder.Services.AddMvc()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

builder.AddCQS(STrainWireup.AddCQS);
builder.AddDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();
app.MapGenericRequestController();

app.Run();
