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
            using var connection = _dbContext.GetDbConnection();
            int rowsAffected = await connection.ExecuteAsync(query, user);

            if(rowsAffected > 0)
            {
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password)
        {
            string query = "SELECT * FROM public.\"Users\" WHERE \"Email\" = @Email AND \"Password\" = @Password";
            using var connection = _dbContext.GetDbConnection();
            ApplicationUser? user = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, new { Email = email, Password = password });
            return user;
        }

        public async Task<ApplicationUser?> GetUserByUserID(Guid userID)
        {
            string query = "SELECT * FROM public.\"Users\" WHERE \"UserID\" = @UserID";
            var parameters = new { UserID = userID };
            using var connection = _dbContext.GetDbConnection();
            ApplicationUser? user = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(query, parameters);
            return user;
        }
    }
}
