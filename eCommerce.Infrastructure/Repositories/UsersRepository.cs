using Dapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Infrastructure.DbContext;

namespace eCommerce.Infrastructure.Repositories
{
    internal class UsersRepository : IUsersRepository
    {
        private readonly DapperDbContext _dbContext;
        public UsersRepository(DapperDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApplicationUser?> AddUser(ApplicationUser user)
        {
            Guid userID = Guid.NewGuid();
            string query = "INSERT INTO public.\"Users\" (\"UserID\", \"Email\", \"Password\", \"PersonName\", \"Gender\")" +
                            " VALUES (@UserID, @Email, @Password, @PersonName, @Gender)";
            int rowsAffected = await _dbContext.DbConnection.ExecuteAsync(query, user);

            if(rowsAffected > 0)
            {
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password)
        {
            string query = "SELECT * FROM public.\"Users\" WHERE \"Email\" = @Email AND \"Password\" = @Password";
            ApplicationUser? user = await _dbContext.DbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(query, new { Email = email, Password = password });
            return user;
        }
    }
}
