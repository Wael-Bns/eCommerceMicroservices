using eCommerce.Core.Entities;

namespace eCommerce.Core.RepositoryContracts
{
    public interface IUsersRepository
    {
        /// <summary>
        /// method to add a user in the database and return the added user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ApplicationUser?> AddUser(ApplicationUser user);
        /// <summary>
        /// method to retrieve existing user by email and password
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ApplicationUser?> GetUserByEmailAndPassword(string? Email, string? password);
        /// <summary>
        /// method to retrieve existing user by userID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<ApplicationUser?> GetUserByUserID(Guid userID);
    }
}
