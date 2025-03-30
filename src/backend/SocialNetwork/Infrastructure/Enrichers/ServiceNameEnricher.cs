using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Enrichers;

public class ServiceNameEnricher : ILogEventEnricher
{
    public ServiceNameEnricher()
    {
    }
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var str = Environment.GetEnvironmentVariable("SERVICE_NAME");
        
        str = string.IsNullOrEmpty(str?.Trim()) ? "None" : str.Trim();
        propertyFactory.CreateProperty("ServiceName", str);
    }
}