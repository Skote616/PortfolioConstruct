using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly PortfolioContext _context;
        public SectionRepository(PortfolioContext context) => _context = context;

        public void Add(Section section)
        {
            _context.Sections.Add(section);
            _context.SaveChanges();
        }

        public void Delete(int sectionId)
        {
            var section = _context.Sections
                .Include(s => s.Blocks)
                    .ThenInclude(b => b.GalleryImages)
                .FirstOrDefault(s => s.Id == sectionId);
            if (section == null) return;
            _context.Sections.Remove(section);
            _context.SaveChanges();
        }

        public void UpdateSortOrder(int sectionId, int sortOrder)
        {
            _context.Sections
                .Where(s => s.Id == sectionId)
                .ExecuteUpdate(s => s.SetProperty(x => x.SortOrder, sortOrder));
        }

        public int GetMaxSortOrder(int portfolioId)
        {
            return _context.Sections
                .Where(s => s.PortfolioId == portfolioId)
                .Select(s => (int?)s.SortOrder)
                .Max() ?? -1;
        }
    }
}
