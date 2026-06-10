using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Repositories
{
    public class GalleryRepository : IGalleryRepository
    {
        private readonly PortfolioContext _context;
        public GalleryRepository(PortfolioContext context) => _context = context;

        public void Add(GalleryImage image)
        {
            _context.GalleryImages.Add(image);
            _context.SaveChanges();
        }

        public void Delete(int galleryImageId)
        {
            var img = _context.GalleryImages.Find(galleryImageId);
            if (img == null) return;
            _context.GalleryImages.Remove(img);
            _context.SaveChanges();
        }

        public void UpdateCaption(int galleryImageId, string? caption)
        {
            _context.GalleryImages
                .Where(g => g.Id == galleryImageId)
                .ExecuteUpdate(s => s.SetProperty(g => g.Caption, caption));
        }

        public int GetMaxSortOrder(int blockId)
        {
            return _context.GalleryImages
                .Where(g => g.BlockId == blockId)
                .Select(g => (int?)g.SortOrder)
                .Max() ?? -1;
        }
    }
}
