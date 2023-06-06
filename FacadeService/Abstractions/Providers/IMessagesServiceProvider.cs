namespace FacadeService.Abstractions.Providers;

public interface IMessagesServiceProvider
{
    Task<string> Get();
}