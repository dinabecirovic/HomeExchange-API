using HomeExchange.Data.Models;

namespace HomeExchange.Interfaces
{
    public interface IUserService
    {
        Task<List<Users>> GetAllUserAsync();
        Task<Users?> GetUserByEmail(string email);
        Task RegisterUser(Users users);
        string HashPassword(string password);
        string GenerateToken(Users users);

    }
}
