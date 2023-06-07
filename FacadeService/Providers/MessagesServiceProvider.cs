using Consul;
using FacadeService.Abstractions.Providers;

namespace FacadeService.Providers;

public class MessagesServiceProvider : IMessagesServiceProvider
{
    private readonly IConsulClient _consulClient;
    private readonly HttpClient _client = new();
    private const string ServiceName = "MessagesService";

    public MessagesServiceProvider(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }

    public async Task<string> Get()
    {
        var random = new Random();
        var response = await _client.GetAsync(await GetServiceUrl());
        var result = await response.Content.ReadAsStringAsync();
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

        return "https://" + listOfServices[random.Next(listOfServices.Count)] + "/" + "messages-service";
    }
}