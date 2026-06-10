using PortfolioConstruct.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioConstruct.Services
{
    /// <summary>
    /// Сервис портфолио — оркестрирует работу нескольких репозиториев.
    /// Содержит бизнес-логику: создание с разделами по умолчанию,
    /// сортировку, загрузку файлов.
    /// </summary>
    public interface IPortfolioService
    {
        Portfolio GetOrCreatePortfolio(int userId);

        // Разделы
        void AddCustomSection(int portfolioId, string title);
        void DeleteSection(int sectionId);
        void MoveSectionUp(int sectionId, int portfolioId);
        void MoveSectionDown(int sectionId, int portfolioId);

        // Блоки
        void AddBlock(int sectionId, string type);
        void UpdateBlock(Block block);
        void DeleteBlock(int blockId);
        void MoveBlockUp(int blockId, int sectionId);
        void MoveBlockDown(int blockId, int sectionId);

        // Дизайн
        void UpdateDesign(DesignSetting settings);

        // Галерея
        Task AddGalleryImagesAsync(int blockId, IReadOnlyList<IBrowserFile> files);
        void DeleteGalleryImage(int galleryImageId);
        void UpdateGalleryImageCaption(int galleryImageId, string? caption);

        // Файлы
        Task<string> SaveImageAsync(IBrowserFile file);
        Task<string> SaveDocumentAsync(IBrowserFile file);
    }
}
