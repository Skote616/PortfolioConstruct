using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PortfolioContext _context;
        public UserRepository(PortfolioContext context) => _context = context;

        public User? GetById(int id)
            => _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefault(u => u.Id == id);

        public User? GetByEmail(string email)
            => _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefault(u => u.Email == email);

        public User? GetByEmailAndHash(string email, string passwordHash)
            => _context.Users.AsNoTracking().Include(u => u.Role)
               .FirstOrDefault(u => u.Email == email && u.PasswordHash == passwordHash && !u.IsArchived);

        public bool EmailExists(string email)
            => _context.Users.Any(u => u.Email == email);

        public List<User> GetAll()
            => _context.Users.AsNoTracking()
               .Include(u => u.Role)
               .Include(u => u.StudentProfile)
               .OrderBy(u => u.CreatedAt).ToList();

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(u => u.Email,      user.Email)
                    .SetProperty(u => u.RoleId,     user.RoleId)
                    .SetProperty(u => u.IsArchived, user.IsArchived));
        }

        public void UpdatePassword(int userId, string passwordHash)
        {
            _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdate(s => s.SetProperty(u => u.PasswordHash, passwordHash));
        }

        public void Delete(int userId)
        {
            var user = _context.Users.Include(u => u.Portfolio).FirstOrDefault(u => u.Id == userId);
            if (user == null) return;
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
