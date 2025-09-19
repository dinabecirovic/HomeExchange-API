using HomeExchange.Data;
using HomeExchange.Data.Models;
using HomeExchange.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeExchange.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public UserService(DatabaseContext databaseContext, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }

        // Vrati sve korisnike
        public async Task<List<Users>> GetAllUserAsync()
        {
            return await _databaseContext.Users.ToListAsync();
        }

        // Pronađi korisnika po email-u
        public async Task<Users?> GetUserByEmail(string email)
        {
            return await _databaseContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Registracija korisnika
        public async Task RegisterUser(Users users)
        {
            await _databaseContext.Users.AddAsync(users);
            await _databaseContext.SaveChangesAsync();
        }

        // Generisanje JWT tokena
        public string GenerateToken(Users users)
        {
            if (users == null)
                throw new ArgumentNullException(nameof(users), "User cannot be null");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Auth:Secret"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", users.Id.ToString()),
                new Claim("role", users.Roles)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        // Hashovanje lozinke
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha.ComputeHash(textBytes);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }
    }
}
