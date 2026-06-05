using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.DTO;
using Polly.Bulkhead;

namespace OrdersMicroservice.Core.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;
        public ProductsMicroserviceClient(HttpClient httpClient, ILogger<ProductsMicroserviceClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }
        public async Task<ProductDTO?> GetProductByProductID(Guid productID)
        {
            try
            {
                // Check cache first
                string? cacheKey = "product_" + productID.ToString();
                string? cachedProduct = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedProduct))
                {
                    _logger.LogInformation("Product found in cache for ProductID: {ProductID}", productID);
                    return JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
                }
                // If not in cache, call the microservice
                HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/products/search/product-id/{productID}");
                if (!response.IsSuccessStatusCode)
                {
                    if(response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        ProductDTO? productFromFallback = await response.Content.ReadFromJsonAsync<ProductDTO>();

                        if (productFromFallback == null)
                        {
                            throw new NotImplementedException("Fallback not implemented.");
                        }
                        return productFromFallback;
                    }
                    else if(response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else if(response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        throw new HttpRequestException($"An error occurred : {HttpStatusCode.InternalServerError} {nameof(ProductsMicroserviceClient)}",null, System.Net.HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        throw new HttpRequestException("Http request failed .", null, response.StatusCode);
                    }
                }
                ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();
            
                if(product == null)
                {
                    throw new ArgumentException("Invalid ProductID");
                }
                string serializedProduct = JsonSerializer.Serialize(product);
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                await _distributedCache.SetStringAsync(cacheKey, serializedProduct, cacheOptions);
                return product;
            }
            catch(BulkheadRejectedException ex)
            {
                _logger.LogError("Bulkhead limit exceeded in ProductsMicroserviceClient: {Message}", ex.Message);
                return new ProductDTO(Guid.Empty, "Unknown Product", "Unknown Category", null, null);
            }
        }
    }
}
