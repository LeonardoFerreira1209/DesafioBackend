using Domain.Contracts.Configurations.ApplicationInsights;
using Microsoft.ApplicationInsights;
using System.Diagnostics.CodeAnalysis;

namespace Application.Configurations.ApplicationInsights;

[ExcludeFromCodeCoverage]
public sealed class TelemetryProxy(
    TelemetryClient telemetryClient) : ITelemetryProxy
{
    private readonly TelemetryClient _telemetryClient = telemetryClient;

    public void TrackEvent(string eventName) 
        => _telemetryClient.TrackEvent(eventName);
}
