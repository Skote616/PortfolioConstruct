using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface IUserRepository
    {
        User? GetById(int id);
        User? GetByEmail(string email);
        User? GetByEmailAndHash(string email, string passwordHash);
        bool EmailExists(string email);
        List<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(int userId);
        void UpdatePassword(int userId, string passwordHash);
    }
}
