using PortfolioConstruct.Models;
using PortfolioConstruct.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioConstruct.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        public AuthService(IUserRepository users) => _users = users;

        public User? Login(string email, string password)
        {
            var hash = HashPassword(password);
            return _users.GetByEmailAndHash(email, hash);
        }

        public bool Register(string email, string password)
        {
            if (_users.EmailExists(email)) return false;
            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password)
            };
            _users.Add(user);
            return true;
        }

        // Хеширование — бизнес-логика, место в сервисе, не в репозитории
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToHexString(sha.ComputeHash(bytes));
        }
    }
}
