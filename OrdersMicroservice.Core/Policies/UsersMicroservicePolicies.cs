using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace OrdersMicroservice.Core.Policies
{
    public class UsersMicroservicePolicies : IUsersMicroservicePolicies
    {
        private readonly ILogger<UsersMicroservicePolicies> _logger;
        public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger) 
        {
            _logger = logger;
        }
        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                         .WaitAndRetryAsync(retryCount: 3,
                             sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                             onRetry: (outcome, timespan, retryAttempt, context) =>
                             {
                                 _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
                             });
            return retryPolicy;
        }
        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                         .CircuitBreakerAsync(
                                handledEventsAllowedBeforeBreaking: 3,
                                durationOfBreak: TimeSpan.FromMinutes(2),
                                onBreak: (outcome, timespan) =>
                                {
                                    _logger.LogWarning($"Circuit breaker opened for {timespan.TotalMinutes} minutes, the subsequent requests will be blocked .");
                                },
                                onReset: () =>
                                {
                                    _logger.LogInformation("Circuit breaker closed, the subsequent requests will be allowed.");
                                }
                );
            return policy;
        }
    }
}
