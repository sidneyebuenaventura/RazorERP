using RazorERP.Exceptions;
using RazorERP.Models;
using RazorERP.Repositories;
using RazorERP.Utilities;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace RazorERP.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, PasswordHasher passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null) throw new UserNotFoundException(id);

            return new UserDto
            {
                Username = user.Username,
                Role = user.Role,
                CompanyId = user.CompanyId
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersByCompanyId(int companyId, string requestingUserRole)
        {
            var users = await _userRepository.GetUsersByCompanyId(companyId);

            if (requestingUserRole == "User")
            {
                users = users.Where(user => user.Role != "Admin");
            }

            return users.Select(user => new UserDto
            {
                Username = user.Username,
                Role = user.Role,
                CompanyId = user.CompanyId
            });
        }

        public async Task<int> CreateUser(UserDto userDto, string password)
        {
            var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(password);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = userDto.Role,
                CompanyId = userDto.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            return await _userRepository.CreateUser(user);
        }

        public async Task<int> UpdateUser(int id, UserDto userDto, string password)
        {
            var (passwordHash, passwordSalt) = _passwordHasher.HashPassword(password);

            var user = new User
            {
                Id = id,
                Username = userDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = userDto.Role,
                CompanyId = userDto.CompanyId
            };

            return await _userRepository.UpdateUser(user);
        }

        public async Task<int> DeleteUser(int id)
        {
            return await _userRepository.DeleteUser(id);
        }

        public async Task<UserDto> Authenticate(string username, string password)
        {
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null) throw new UserNotFoundException(username);

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid password.");

            return new UserDto
            {
                Username = user.Username,
                Role = user.Role,
                CompanyId = user.CompanyId
            };
        }

        public string GenerateJwtToken(UserDto user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
