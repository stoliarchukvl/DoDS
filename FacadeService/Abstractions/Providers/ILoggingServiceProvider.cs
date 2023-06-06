using FacadeService.Contracts.Logging;

namespace FacadeService.Abstractions.Providers;

public interface ILoggingServiceProvider
{
    Task CreateLog(LoggingRequest request);
    Task<string> GetLogs();
}