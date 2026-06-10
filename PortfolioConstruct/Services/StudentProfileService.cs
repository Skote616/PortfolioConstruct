using PortfolioConstruct.Models;
using PortfolioConstruct.Repositories;
using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioConstruct.Services
{
    public class StudentProfileService : IStudentProfileService
    {
        private readonly IStudentProfileRepository _profiles;
        public StudentProfileService(IStudentProfileRepository profiles) => _profiles = profiles;

        public StudentProfile GetOrCreate(int userId)
            => _profiles.GetByUserId(userId) ?? new StudentProfile { UserId = userId };

        public void Save(StudentProfile profile) => _profiles.Save(profile);

        public async Task<string> SaveAvatarAsync(IBrowserFile file)
        {
            var dir = Path.Combine("wwwroot", "avatars");
            Directory.CreateDirectory(dir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
            await using var fs = new FileStream(Path.Combine(dir, fileName), FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024).CopyToAsync(fs);
            return $"/avatars/{fileName}";
        }
    }
}
