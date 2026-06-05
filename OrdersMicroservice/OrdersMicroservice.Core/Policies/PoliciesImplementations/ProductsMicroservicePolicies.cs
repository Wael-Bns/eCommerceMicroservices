using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.DTO;
using OrdersMicroservice.Core.Policies.PoliciesContracts;
using Polly;
using Polly.Bulkhead;
using Polly.Fallback;
using Polly.Wrap;

namespace OrdersMicroservice.Core.Policies.PoliciesImplementations
{
    public class ProductsMicroservicePolicies : IProductsMicroservicePolicies
    {
        private readonly ILogger<ProductsMicroservicePolicies> _logger;
        public ProductsMicroservicePolicies(ILogger<ProductsMicroservicePolicies> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy()
        {
            AsyncBulkheadPolicy<HttpResponseMessage> bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>
                (
                    maxParallelization: 2, // allows up to 2 concurrent requests
                    maxQueuingActions: 40, // allows up to 40 requests to wait in the queue
                    onBulkheadRejectedAsync: (context) =>
                    {
                        _logger.LogWarning("Bulkhead policy triggered. Can't send any more requests since the queue is full.");
                        throw new BulkheadRejectedException("Bulkhead queue is full .");
                    }
                );
            return bulkheadPolicy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            AsyncFallbackPolicy<HttpResponseMessage> fallbackPolicy =
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .FallbackAsync(async (context) =>
                    {
                        _logger.LogWarning("Fallback executed. Products microservice is temporarily unavailable.");
                        ProductDTO? productDTO = new ProductDTO(Guid.Empty, "Unknown Product (fallback)", "Unknown Category (fallback)", null, null);
                        var response = new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                        {
                            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(productDTO), System.Text.Encoding.UTF8, "application/json")
                        };
                        return response;
                    });

            return fallbackPolicy;
        }
        public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
        {
            var bulkheadPolicy = GetBulkheadIsolationPolicy();
            var fallbackPolicy = GetFallbackPolicy();
            AsyncPolicyWrap<HttpResponseMessage> combinedPolicy = Policy.WrapAsync(fallbackPolicy, bulkheadPolicy);
            return combinedPolicy;
        }
    }
}
