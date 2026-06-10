using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface IDesignRepository
    {
        void Add(DesignSetting settings);
        void Update(DesignSetting settings);
    }
}
