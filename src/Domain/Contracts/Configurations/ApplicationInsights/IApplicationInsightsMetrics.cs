using Domain.Dtos.Configurations;

namespace Domain.Contracts.Configurations.ApplicationInsights;

public interface IApplicationInsightsMetrics
{
    void AddMetric(CustomMetricDto metrica);
}
