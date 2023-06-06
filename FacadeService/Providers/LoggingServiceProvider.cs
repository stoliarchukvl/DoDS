using System.Text;
using System.Text.Json;
using FacadeService.Abstractions.Providers;
using FacadeService.Contracts.Logging;

namespace FacadeService.Providers;

public class LoggingServiceProvider : ILoggingServiceProvider
{
    private readonly HttpClient _client = new();
    private const string Url = "https://localhost:4002/logging-service";


    public async Task CreateLog(LoggingRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request), 
            Encoding.UTF8, 
            "application/json");

        var response = await _client.PostAsync(Url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Bad request");
        }
    }

    public async Task<string> GetLogs()
    {
        var response = await _client.GetAsync(Url);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}