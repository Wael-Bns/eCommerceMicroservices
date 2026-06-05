using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace ProductsService.Infrastructure.Context
{
    public class ApplicationDbContext 
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("MySqlConnection");
            _dbConnection = new MySqlConnection(connectionString);
        }

        public IDbConnection DbConnection => _dbConnection;
    }
}
