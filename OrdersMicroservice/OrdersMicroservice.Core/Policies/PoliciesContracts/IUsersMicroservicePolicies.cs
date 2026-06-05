using Polly;

namespace OrdersMicroservice.Core.Policies.PoliciesContracts
{
    public interface IUsersMicroservicePolicies
    {
        IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
        IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
    }
}
