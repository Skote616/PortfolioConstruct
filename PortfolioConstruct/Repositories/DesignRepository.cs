using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class DesignRepository : IDesignRepository
    {
        private readonly PortfolioContext _context;
        public DesignRepository(PortfolioContext context) => _context = context;

        public void Add(DesignSetting settings)
        {
            _context.DesignSettings.Add(settings);
            _context.SaveChanges();
        }

        public void Update(DesignSetting settings)
        {
            _context.DesignSettings
                .Where(d => d.Id == settings.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(d => d.Color,       settings.Color)
                    .SetProperty(d => d.Font,        settings.Font)
                    .SetProperty(d => d.Theme,       settings.Theme)
                    .SetProperty(d => d.AccentColor, settings.AccentColor));
        }
    }
}
