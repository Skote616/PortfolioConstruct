using PortfolioConstruct.Models;

namespace PortfolioConstruct.Services
{
    public interface IAdminService
    {
        List<User> GetAllUsers();
        User? GetUser(int id);
        void UpdateUser(User user);
        void UpdateStudentName(int userId, string? fullName);
        void DeleteUser(int userId);
        void ArchiveUser(int userId);
        void RestoreUser(int userId);
        void ChangePassword(int userId, string newPassword);
        void CreateUser(string email, string password, string roleCode, string? fullName);
    }
}
