using System.Net.Http.Json;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        public ProductsMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ProductDTO?> GetProductByProductID(Guid productID)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/products/search/product-id/{productID}");
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException("An internal error occurred in the products microservice",null, System.Net.HttpStatusCode.BadRequest);
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
            return product;
        }
    }
}
