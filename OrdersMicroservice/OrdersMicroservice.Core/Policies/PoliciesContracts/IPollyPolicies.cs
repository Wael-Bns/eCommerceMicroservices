using Polly;

namespace OrdersMicroservice.Core.Policies.PoliciesContracts
{
    /// <summary>
    /// An abstraction of common polly policies that can be used across different microservices.
    /// </summary>
    public interface IPollyPolicies
    {
        /// <summary>
        /// Returns a retry policy that retries a specified number of times with an exponential backoff strategy when the HTTP response is not successful.
        /// </summary>
        /// <param name="retryCount">The number of times to retry the operation.</param>
        /// <returns>An asynchronous retry policy.</returns>
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount);
        /// <summary>
        /// Returns a circuit breaker policy that breaks the circuit for a specified duration after a certain number of handled events.
        /// </summary>
        /// <param name="handledEventsAllowedBeforeBreaking">The number of handled events before breaking the circuit.</param>
        /// <param name="durationOfBreak">The duration to break the circuit.</param>
        /// <returns>An asynchronous circuit breaker policy.</returns>
        IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak);
        /// <summary>
        /// Returns a timeout policy that cancels the operation if it does not complete within the specified duration.
        /// </summary>
        /// <param name="timeout">The maximum duration to wait for the operation to complete.</param>
        /// <returns>An asynchronous timeout policy.</returns>
        IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout);
    }
}
