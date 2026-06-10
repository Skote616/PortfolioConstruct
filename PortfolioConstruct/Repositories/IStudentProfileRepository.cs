using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface IStudentProfileRepository
    {
        StudentProfile? GetByUserId(int userId);
        void Save(StudentProfile profile); // создаёт или обновляет
    }
}
