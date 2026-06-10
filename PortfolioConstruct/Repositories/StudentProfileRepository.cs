using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class StudentProfileRepository : IStudentProfileRepository
    {
        private readonly PortfolioContext _context;
        public StudentProfileRepository(PortfolioContext context) => _context = context;

        public StudentProfile? GetByUserId(int userId)
            => _context.StudentProfiles.AsNoTracking().FirstOrDefault(p => p.UserId == userId);

        public void Save(StudentProfile profile)
        {
            var existing = _context.StudentProfiles.FirstOrDefault(p => p.UserId == profile.UserId);
            if (existing == null)
            {
                _context.StudentProfiles.Add(profile);
            }
            else
            {
                existing.FullName  = profile.FullName;
                existing.GroupName = profile.GroupName;
                existing.Specialty = profile.Specialty;
                existing.Phone     = profile.Phone;
                existing.AvatarPath = profile.AvatarPath;
                _context.StudentProfiles.Update(existing);
            }
            _context.SaveChanges();
        }
    }
}
