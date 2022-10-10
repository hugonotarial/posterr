using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace Api.Extensions
{
    public static class HealthCheckExtension
    {
        public static void UseCustomMapHealthChecksEndpoint(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapHealthChecks("health/ready", new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("ready"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoint.MapHealthChecks("health/live", new HealthCheckOptions
            {
                Predicate = (_) => false,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}
