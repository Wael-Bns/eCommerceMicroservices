using eCommerce.Core.DTO;

namespace eCommerce.Core.ServiceContracts
{
    public interface IUsersService
    {
        /// <summary>
        /// login to the app
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        Task<AuthenticationResponse?> Login(LoginRequest loginRequest);
        /// <summary>
        /// register a user
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        Task<AuthenticationResponse?> Register(RegisterRequest registerRequest);
        /// <summary>
        /// Get user details by userID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<UserDTO?> GetUserByUserID(Guid userID);
    }
}
