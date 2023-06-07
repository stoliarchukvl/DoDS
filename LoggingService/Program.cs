using Consul;
using LoggingService.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConsulClient>(sp => new ConsulClient(cfg =>
{
    cfg.Address = new Uri("http://localhost:8500");
}));

builder.Services.AddSingleton<IHostedService, ConsulHostedService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
