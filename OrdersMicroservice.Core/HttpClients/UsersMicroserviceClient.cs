using System.Net.Http.Json;
using System.Text.Json;
using DnsClient.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrdersMicroservice.Core.DTO;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace OrdersMicroservice.Core.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersMicroserviceClient> _logger;
        private readonly IDistributedCache _distributedCache;
        public UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<UserDTO?> GetUserByUserID(Guid userID)
        {
            try
            {
                string cacheKey = "user_" + userID.ToString();
                string? cachedUserJson = await _distributedCache.GetStringAsync(cacheKey);
                if(!string.IsNullOrEmpty(cachedUserJson))
                {
                    _logger.LogInformation("User data for UserID {UserID} found in cache.", userID);
                    return JsonSerializer.Deserialize<UserDTO>(cachedUserJson);
                }
                HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{userID}");
                if (!response.IsSuccessStatusCode)
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        UserDTO? fallbackUser = await response.Content.ReadFromJsonAsync<UserDTO>();
                        if(fallbackUser == null)
                        {
                            throw new NotImplementedException("Fallback for users microservice not implemented .");
                        }
                        return fallbackUser;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException("Bad Request.", null, System.Net.HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        return new UserDTO(Guid.Empty, "Temporarily Unavailable", "Temporarily Unavailable", "Temporarily Unavailable");
                    }
                }

                UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

                if (user == null)
                {
                    throw new ArgumentException("Invalid UserID");
                }

                string serializedUser = JsonSerializer.Serialize(user);
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                await _distributedCache.SetStringAsync(cacheKey, serializedUser, cacheOptions);
                
                return user;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Circuit breaker is open. Users microservice is temporarily unavailable.");
                return new UserDTO(Guid.Empty, "Temporarily Unavailable (Circuit Breaker Open)", "Temporarily Unavailable (Circuit Breaker Open)", "Temporarily Unavailable (Circuit Breaker Open)");
            }
            catch(TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "Request to Users microservice timed out.");
                return new UserDTO(Guid.Empty, "Temporarily Unavailable (Timeout)", "Temporarily Unavailable (Timeout)", "Temporarily Unavailable (Timeout) ");
            }
        }
    }
}
