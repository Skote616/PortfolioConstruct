using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly PortfolioContext _context;
        public PortfolioRepository(PortfolioContext context) => _context = context;

        public Portfolio? GetByUserId(int userId)
        {
            return _context.Portfolios
                .AsNoTracking()
                .Include(p => p.Sections.OrderBy(s => s.SortOrder))
                    .ThenInclude(s => s.Blocks.OrderBy(b => b.SortOrder))
                        .ThenInclude(b => b.BlockType)          // <-- это было пропущено
                .Include(p => p.Sections.OrderBy(s => s.SortOrder))
                    .ThenInclude(s => s.Blocks.OrderBy(b => b.SortOrder))
                        .ThenInclude(b => b.GalleryImages.OrderBy(g => g.SortOrder))
                .Include(p => p.DesignSetting)
                .FirstOrDefault(p => p.UserId == userId);
        }

        public int Add(Portfolio portfolio)
        {
            _context.Portfolios.Add(portfolio);
            _context.SaveChanges();
            return portfolio.Id;
        }
    }
}
