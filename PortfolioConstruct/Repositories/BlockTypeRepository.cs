using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public class BlockTypeRepository : IBlockTypeRepository
    {
        private readonly PortfolioContext _context;
        public BlockTypeRepository(PortfolioContext context) => _context = context;

        public int GetIdByCode(string code)
            => _context.BlockTypes.FirstOrDefault(t => t.Code == code)?.Id
               ?? throw new InvalidOperationException($"BlockType '{code}' не найден в справочнике");
    }
}
