using RazorERP.Models;
namespace RazorERP.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetUsersByCompanyId(int companyId);
        Task<int> CreateUser(User user);
        Task<int> UpdateUser(User user);
        Task<int> DeleteUser(int id);
        Task<User> GetUserByUsername(string username);
    }
}
