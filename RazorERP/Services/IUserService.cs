using RazorERP.Models;

namespace RazorERP.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserById(int id);
        Task<IEnumerable<UserDto>> GetUsersByCompanyId(int companyId, string requestingUserRole);
        Task<int> CreateUser(UserDto userDto, string password);
        Task<int> UpdateUser(int id, UserDto userDto, string password);
        Task<int> DeleteUser(int id);
        Task<UserDto> Authenticate(string username, string password);
        string GenerateJwtToken(UserDto user);
    }
}
