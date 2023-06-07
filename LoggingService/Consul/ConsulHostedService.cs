using Consul;

namespace LoggingService.Consul;

public class ConsulHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly IHostApplicationLifetime _appLifetime;

    public ConsulHostedService(IConsulClient consulClient, IHostApplicationLifetime appLifetime)
    {
        _consulClient = consulClient;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serviceName = "LoggingService";
        var serviceId = "Logging1";
        var adderss = "localhost";
        var servicePort = 5101;

        var registration = new AgentServiceRegistration
        {
            ID = serviceId,
            Name = serviceName,
            Address = adderss,
            Port = servicePort,
            Check = new AgentServiceCheck
            {
                HTTP = $"https://{adderss}:{servicePort}/logging-service/health",
                Interval = TimeSpan.FromSeconds(10),
            }
        };
        await _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);
        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);

        _appLifetime.ApplicationStopping.Register(() =>
        {
            _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken).GetAwaiter().GetResult();
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}