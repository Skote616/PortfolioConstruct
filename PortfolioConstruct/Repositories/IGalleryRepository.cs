using PortfolioConstruct.Models;

namespace PortfolioConstruct.Repositories
{
    public interface IGalleryRepository
    {
        void Add(GalleryImage image);
        void Delete(int galleryImageId);
        void UpdateCaption(int galleryImageId, string? caption);
        int GetMaxSortOrder(int blockId);
    }
}
