using System.Net.Http.Json;
using OrdersMicroservice.Core.DTO;

namespace OrdersMicroservice.Core.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        public UsersMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                else if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException("Bad request",null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    throw new HttpRequestException("Http request failed .", null, response.StatusCode);
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
