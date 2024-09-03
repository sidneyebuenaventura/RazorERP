using RazorERP.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace RazorERP.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<User> GetUserById(int id)
        {
            const string sql = "SELECT * FROM Users WHERE Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyId(int companyId)
        {
            const string sql = "SELECT * FROM Users WHERE CompanyId = @CompanyId";
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<User>(sql, new { CompanyId = companyId });
            }
        }

        public async Task<int> CreateUser(User user)
        {
            const string sql = @"
                INSERT INTO Users (Username, PasswordHash, PasswordSalt, Role, CompanyId, CreatedAt)
                VALUES (@Username, @PasswordHash, @PasswordSalt, @Role, @CompanyId, @CreatedAt)";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, user);
            }
        }

        public async Task<int> UpdateUser(User user)
        {
            const string sql = @"
                UPDATE Users 
                SET Username = @Username, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt, Role = @Role 
                WHERE Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, user);
            }
        }

        public async Task<int> DeleteUser(int id)
        {
            const string sql = "DELETE FROM Users WHERE Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            const string sql = "SELECT * FROM Users WHERE Username = @Username";
            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
            }
        }
    }
}
