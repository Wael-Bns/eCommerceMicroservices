using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.Policies.PoliciesContracts;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace OrdersMicroservice.Core.Policies.PoliciesImplementations
{
    public class PollyPolicies : IPollyPolicies
    {
        private  readonly ILogger<PollyPolicies> _logger;
        public PollyPolicies(ILogger<PollyPolicies> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                         .CircuitBreakerAsync(
                                handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
                                durationOfBreak: durationOfBreak,
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

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                         .WaitAndRetryAsync(retryCount: retryCount,
                             sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                             onRetry: (outcome, timespan, retryAttempt, context) =>
                             {
                                 _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
                             });
            return retryPolicy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
        {
            AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy =
                Policy.TimeoutAsync<HttpResponseMessage>(timeout);
            return timeoutPolicy;
        }
    }
}
