using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.Policies.PoliciesContracts;
using Polly;
using Polly.Fallback;
using Polly.Wrap;

namespace OrdersMicroservice.Core.Policies.PoliciesImplementations
{
    public class UsersMicroservicePolicies : IUsersMicroservicePolicies
    {
        private readonly IPollyPolicies _pollyPolicies;
        private readonly ILogger<UsersMicroservicePolicies> _logger;
        public UsersMicroservicePolicies(IPollyPolicies pollyPolicies, ILogger<UsersMicroservicePolicies> logger) 
        {
            _pollyPolicies = pollyPolicies;
            _logger = logger;
        }
        public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
        {
            var retryPolicy = _pollyPolicies.GetRetryPolicy(3);
            var circuitBreakerPolicy = _pollyPolicies.GetCircuitBreakerPolicy(3, TimeSpan.FromMinutes(2));
            var timeoutPolicy = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromMilliseconds(1500));
            var fallbackPolicy = GetFallbackPolicy();
            AsyncPolicyWrap<HttpResponseMessage> combinedPolicy = Policy.WrapAsync(fallbackPolicy, retryPolicy, circuitBreakerPolicy, timeoutPolicy);
            return combinedPolicy;
        }
        public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            AsyncFallbackPolicy<HttpResponseMessage> fallbackPolicy =
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<Polly.CircuitBreaker.BrokenCircuitException>()
                    .Or<Polly.Timeout.TimeoutRejectedException>()
                    .FallbackAsync(async (context) =>
                    {
                        _logger.LogWarning("Fallback executed. Users microservice is temporarily unavailable.");
                        UserDTO? userDTO = new UserDTO(Guid.Empty, "Temporarily Unavailable", "Temporarily Unavailable", "Temporarily Unavailable");
                        var response = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                        {
                            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(userDTO), System.Text.Encoding.UTF8, "application/json")
                        };
                        return response;
                    });

            return fallbackPolicy;
        }
    }
}
