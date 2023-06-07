using Consul;

namespace FacadeService.Consul;

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
        var serviceName = "FacadeService";
        var serviceId = "Facade";
        var adderss = "localhost";
        var servicePort = 4001;

        var registration = new AgentServiceRegistration
        {
            ID = serviceId,
            Name = serviceName,
            Address = adderss,
            Port = servicePort,
            Check = new AgentServiceCheck
            {
                Name = "Checks the facades http response",
                HTTP = $"https://{adderss}:{servicePort}/facade-service/health",
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