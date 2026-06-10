using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface ISectionRepository
    {
        void Add(Section section);
        void Delete(int sectionId);
        void UpdateSortOrder(int sectionId, int sortOrder);
        int GetMaxSortOrder(int portfolioId);
    }
}
