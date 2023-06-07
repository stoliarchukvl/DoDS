using System.Text;
using System.Text.Json;
using FacadeService.Abstractions.Providers;
using FacadeService.Contracts.Logging;

namespace FacadeService.Providers;

public class LoggingServiceProvider : ILoggingServiceProvider
{
    private readonly HttpClient _client = new();
    private readonly List<string> _urls = new ()
    {
        "https://localhost:5101/logging-service",
        "https://localhost:5102/logging-service",
        "https://localhost:5103/logging-service",
    };


    public async Task CreateLog(LoggingRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request), 
            Encoding.UTF8, 
            "application/json");

        var random = new Random();

        var response = await _client.PostAsync(_urls[random.Next(_urls.Count)], content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Bad request");
        }
    }

    public async Task<string> GetLogs()
    {
        var random = new Random();
        var result = string.Empty;

        var success = false;
        
        while (!success)
        {
            try
            {
                var response = await _client.GetAsync(_urls[random.Next(_urls.Count)]);
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
}