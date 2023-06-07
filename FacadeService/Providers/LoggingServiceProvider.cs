using System.Text;
using System.Text.Json;
using Consul;
using FacadeService.Abstractions.Providers;
using FacadeService.Contracts.Logging;

namespace FacadeService.Providers;

public class LoggingServiceProvider : ILoggingServiceProvider
{
    private readonly IConsulClient _consulClient;
    private readonly HttpClient _client = new();
    private const string ServiceName = "LoggingService";

    public LoggingServiceProvider(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }


    public async Task CreateLog(LoggingRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request), 
            Encoding.UTF8, 
            "application/json");

        var random = new Random();

        var response = await _client.PostAsync(await GetServiceUrl(), content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Bad request");
        }
    }

    public async Task<string> GetLogs()
    {
        var result = string.Empty;

        var success = false;
        
        while (!success)
        {
            try
            {
                var response = await _client.GetAsync(await GetServiceUrl());
                result = await response.Content.ReadAsStringAsync();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
        }
        
        return result;
    }

    private async Task<string> GetServiceUrl()
    {
        var services = await _consulClient.Catalog.Service(ServiceName);
        var listOfServices = new List<string>();
        foreach (var element in services.Response)
        {
            var address = element.Address;
            var port = element.ServicePort;
            listOfServices.Add(address + ":" + port);
        }

        var random = new Random();

        return "https://" + listOfServices[random.Next(listOfServices.Count)] + "/" + "logging-service";
    }
}