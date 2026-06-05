using Polly;

namespace OrdersMicroservice.Core.Policies.PoliciesContracts
{
    public interface IProductsMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
        IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy();
        IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
    }
}
