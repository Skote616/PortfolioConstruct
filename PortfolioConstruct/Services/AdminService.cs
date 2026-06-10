using PortfolioConstruct.Models;
using PortfolioConstruct.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioConstruct.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository            _users;
        private readonly IStudentProfileRepository  _profiles;

        public AdminService(IUserRepository users, IStudentProfileRepository profiles)
        {
            _users    = users;
            _profiles = profiles;
        }

        public List<User> GetAllUsers() => _users.GetAll();
        public User? GetUser(int id)    => _users.GetById(id);

        public void UpdateUser(User user) => _users.Update(user);

        public void UpdateStudentName(int userId, string? fullName)
        {
            var profile = _profiles.GetByUserId(userId) ?? new StudentProfile { UserId = userId };
            profile.FullName = fullName;
            _profiles.Save(profile);
        }

        public void DeleteUser(int userId) => _users.Delete(userId);

        public void ArchiveUser(int userId)
        {
            var user = _users.GetById(userId);
            if (user == null) return;
            user.IsArchived = true;
            _users.Update(user);
        }

        public void RestoreUser(int userId)
        {
            var user = _users.GetById(userId);
            if (user == null) return;
            user.IsArchived = false;
            _users.Update(user);
        }

        public void ChangePassword(int userId, string newPassword)
            => _users.UpdatePassword(userId, HashPassword(newPassword));

        public void CreateUser(string email, string password, string roleCode, string? fullName)
        {
            // roleCode "User" -> RoleId 1, "Admin" -> RoleId 2
            int roleId = roleCode == "Admin" ? 2 : 1;
            var user = new User
            {
                Email        = email,
                PasswordHash = HashPassword(password),
                RoleId       = roleId,
                CreatedAt    = DateTime.UtcNow
            };
            _users.Add(user);

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                _profiles.Save(new StudentProfile { UserId = user.Id, FullName = fullName });
            }
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
