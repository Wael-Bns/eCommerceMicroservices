using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace eCommerce.Infrastructure.DbContext
{
    public class DapperDbContext
    {
        private readonly IConfiguration _configuration;
        public DapperDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
        public IDbConnection GetDbConnection()
        {
            string connectionStringTemplate = _configuration.GetConnectionString("PostgresConnection")!;
            string connectionString = connectionStringTemplate
                .Replace("$POSTGRES_HOST", Environment.GetEnvironmentVariable("POSTGRES_HOST"))
                .Replace("$POSTGRES_PASSWORD", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"))
                .Replace("$POSTGRES_USER", Environment.GetEnvironmentVariable("POSTGRES_USER"))
                .Replace("$POSTGRES_DATABASE", Environment.GetEnvironmentVariable("POSTGRES_DATABASE"))
                .Replace("$POSTGRES_PORT", Environment.GetEnvironmentVariable("POSTGRES_PORT"));
            var connection = new NpgsqlConnection(connectionString);
            return connection;
        }
    }
}
