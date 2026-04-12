using System.Net.Http.Json;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersMicroserviceClient> _logger;
        public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserDTO?> GetUserByUserID(Guid userID)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userID}");
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad Request.",null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    return new UserDTO(Guid.Empty, "Temporarily Unavailable", "Temporarily Unavailable", "Temporarily Unavailable");
                }
            }

            UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();
            
            if(user == null)
            {
                throw new ArgumentException("Invalid UserID");
            }

            return user;
        }
    }
}
