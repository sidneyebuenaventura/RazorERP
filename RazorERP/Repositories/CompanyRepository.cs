using RazorERP.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace RazorERP.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly string _connectionString;

        public CompanyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Company> GetCompanyById(int id)
        {
            const string sql = "SELECT * FROM Companies WHERE Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Company>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            const string sql = "SELECT * FROM Companies";
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Company>(sql);
            }
        }

        public async Task<int> CreateCompany(Company company)
        {
            const string sql = @"
                INSERT INTO Companies (Name, CreatedAt)
                VALUES (@Name, @CreatedAt)";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, company);
            }
        }

        public async Task<int> UpdateCompany(Company company)
        {
            const string sql = @"
                UPDATE Companies 
                SET Name = @Name 
                WHERE Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, company);
            }
        }

        public async Task<int> DeleteCompany(int id)
        {
            const string sql = "DELETE FROM Companies WHERE Id = @Id";
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
