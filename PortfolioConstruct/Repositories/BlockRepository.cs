using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        private readonly PortfolioContext _context;
        public BlockRepository(PortfolioContext context) => _context = context;

        public void Add(Block block)
        {
            _context.Blocks.Add(block);
            _context.SaveChanges();
        }

        public void Update(Block block)
        {
            _context.Blocks
                .Where(b => b.Id == block.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(b => b.Content,     block.Content)
                    .SetProperty(b => b.Caption,     block.Caption)
                    .SetProperty(b => b.TextAlign,   block.TextAlign)
                    .SetProperty(b => b.FontSize,    block.FontSize)
                    .SetProperty(b => b.TextColor,   block.TextColor));
        }

        public void Delete(int blockId)
        {
            var block = _context.Blocks.Include(b => b.GalleryImages).FirstOrDefault(b => b.Id == blockId);
            if (block == null) return;
            _context.Blocks.Remove(block);
            _context.SaveChanges();
        }

        public void UpdateSortOrder(int blockId, int sortOrder)
        {
            _context.Blocks
                .Where(b => b.Id == blockId)
                .ExecuteUpdate(s => s.SetProperty(x => x.SortOrder, sortOrder));
        }

        public int GetMaxSortOrder(int sectionId)
        {
            return _context.Blocks
                .Where(b => b.SectionId == sectionId)
                .Select(b => (int?)b.SortOrder)
                .Max() ?? -1;
        }

        public List<Block> GetBySectionOrdered(int sectionId)
        {
            return _context.Blocks
                .AsNoTracking()
                .Where(b => b.SectionId == sectionId)
                .OrderBy(b => b.SortOrder)
                .ToList();
        }

        // Получаем BlockTypeId по строковому коду
        public int GetBlockTypeId(PortfolioContext context, string typeCode)
        {
            return context.BlockTypes.FirstOrDefault(t => t.Code == typeCode)?.Id ?? 1;
        }
    }
}
