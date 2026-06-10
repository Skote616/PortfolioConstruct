using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface IBlockRepository
    {
        void Add(Block block);
        void Update(Block block);
        void Delete(int blockId);
        void UpdateSortOrder(int blockId, int sortOrder);
        int GetMaxSortOrder(int sectionId);
        List<Block> GetBySectionOrdered(int sectionId);
    }
}
