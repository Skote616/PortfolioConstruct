using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface IPortfolioRepository
    {
        Portfolio? GetByUserId(int userId);
        int Add(Portfolio portfolio);
    }
}
