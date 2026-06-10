using PortfolioConstruct.Models;

namespace PortfolioConstruct.Services
{
    public interface IStudentProfileService
    {
        StudentProfile GetOrCreate(int userId);
        void Save(StudentProfile profile);
        Task<string> SaveAvatarAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file);
    }
}
