using Polly;

namespace OrdersMicroservice.Core.Policies
{
    public interface IUsersMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
        IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy();
    }
}
