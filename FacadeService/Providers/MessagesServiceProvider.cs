using FacadeService.Abstractions.Providers;

namespace FacadeService.Providers;

public class MessagesServiceProvider : IMessagesServiceProvider
{
    private readonly HttpClient _client = new();
    private const string Url = "https://localhost:4003/messages-service";

    public async Task<string> Get()
    {
        var response = await _client.GetAsync(Url);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}