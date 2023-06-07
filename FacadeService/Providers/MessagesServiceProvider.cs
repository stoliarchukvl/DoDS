using FacadeService.Abstractions.Providers;

namespace FacadeService.Providers;

public class MessagesServiceProvider : IMessagesServiceProvider
{
    private readonly HttpClient _client = new();
    private readonly List<string> _urls = new()
    {
        "https://localhost:6101/messages-service",
        "https://localhost:6102/messages-service",
    };

    public async Task<string> Get()
    {
        var random = new Random();
        var response = await _client.GetAsync(_urls[random.Next(_urls.Count)]);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}