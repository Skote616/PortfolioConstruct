using PortfolioConstruct.Models;
using PortfolioConstruct.Repositories;
using Microsoft.AspNetCore.Components.Forms;

namespace PortfolioConstruct.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolios;
        private readonly ISectionRepository   _sections;
        private readonly IBlockRepository     _blocks;
        private readonly IDesignRepository    _designs;
        private readonly IGalleryRepository   _gallery;
        private readonly IBlockTypeRepository  _blockTypes;
        private readonly IFileService         _files;

        public PortfolioService(
            IPortfolioRepository portfolios,
            ISectionRepository   sections,
            IBlockRepository     blocks,
            IDesignRepository    designs,
            IGalleryRepository   gallery,
            IFileService         files,
            IBlockTypeRepository blockTypes)
        {
            _portfolios = portfolios;
            _sections   = sections;
            _blocks     = blocks;
            _designs    = designs;
            _gallery    = gallery;
            _files      = files;
            _blockTypes = blockTypes;
        }

        // ===== ПОРТФОЛИО =====

        public Portfolio GetOrCreatePortfolio(int userId)
        {
            var portfolio = _portfolios.GetByUserId(userId);
            if (portfolio != null) return portfolio;

            // Бизнес-правило: новое портфолио создаётся с пятью фиксированными разделами
            var id = _portfolios.Add(new Portfolio { UserId = userId});

            var fixedSections = new[] { "О себе", "Достижения", "Работы", "Документы", "Я гражданин" };
            for (int i = 0; i < fixedSections.Length; i++)
                _sections.Add(new Section { PortfolioId = id, Title = fixedSections[i], IsCustom = false, SortOrder = i });

            _designs.Add(new DesignSetting
            {
                PortfolioId = id,
                Color       = "#ffffff",
                Font        = "Arial",
                Theme       = "light",
                AccentColor = "#4f6ef7"
            });

            return _portfolios.GetByUserId(userId)!;
        }

        // ===== РАЗДЕЛЫ =====

        public void AddCustomSection(int portfolioId, string title)
        {
            var sortOrder = _sections.GetMaxSortOrder(portfolioId) + 1;
            _sections.Add(new Section { PortfolioId = portfolioId, Title = title, IsCustom = true, SortOrder = sortOrder });
        }

        public void DeleteSection(int sectionId)
            => _sections.Delete(sectionId);

        public void MoveSectionUp(int sectionId, int portfolioId)
        {
            var portfolio = _portfolios.GetByUserId(portfolioId);
            if (portfolio == null) return;
            var ordered = portfolio.Sections.OrderBy(s => s.SortOrder).ToList();
            var idx = ordered.FindIndex(s => s.Id == sectionId);
            if (idx <= 0) return;
            // Меняем местами с предыдущим
            _sections.UpdateSortOrder(ordered[idx].Id,     ordered[idx - 1].SortOrder);
            _sections.UpdateSortOrder(ordered[idx - 1].Id, ordered[idx].SortOrder);
        }

        public void MoveSectionDown(int sectionId, int portfolioId)
        {
            var portfolio = _portfolios.GetByUserId(portfolioId);
            if (portfolio == null) return;
            var ordered = portfolio.Sections.OrderBy(s => s.SortOrder).ToList();
            var idx = ordered.FindIndex(s => s.Id == sectionId);
            if (idx < 0 || idx >= ordered.Count - 1) return;
            _sections.UpdateSortOrder(ordered[idx].Id,     ordered[idx + 1].SortOrder);
            _sections.UpdateSortOrder(ordered[idx + 1].Id, ordered[idx].SortOrder);
        }

        // ===== БЛОКИ =====

        public void AddBlock(int sectionId, string typeCode)
        {
            var sortOrder   = _blocks.GetMaxSortOrder(sectionId) + 1;
            var blockTypeId = _blockTypes.GetIdByCode(typeCode);
            _blocks.Add(new Block
            {
                SectionId   = sectionId,
                BlockTypeId = blockTypeId,
                Content     = string.Empty,
                SortOrder   = sortOrder
            });
        }

        public void UpdateBlock(Block block) => _blocks.Update(block);
        public void DeleteBlock(int blockId) => _blocks.Delete(blockId);

        public void MoveBlockUp(int blockId, int sectionId)
        {
            var blocks = _blocks.GetBySectionOrdered(sectionId);
            var idx = blocks.FindIndex(b => b.Id == blockId);
            if (idx <= 0) return;
            _blocks.UpdateSortOrder(blocks[idx].Id,     blocks[idx - 1].SortOrder);
            _blocks.UpdateSortOrder(blocks[idx - 1].Id, blocks[idx].SortOrder);
        }

        public void MoveBlockDown(int blockId, int sectionId)
        {
            var blocks = _blocks.GetBySectionOrdered(sectionId);
            var idx = blocks.FindIndex(b => b.Id == blockId);
            if (idx < 0 || idx >= blocks.Count - 1) return;
            _blocks.UpdateSortOrder(blocks[idx].Id,     blocks[idx + 1].SortOrder);
            _blocks.UpdateSortOrder(blocks[idx + 1].Id, blocks[idx].SortOrder);
        }

        // ===== ДИЗАЙН =====

        public void UpdateDesign(DesignSetting settings) => _designs.Update(settings);

        // ===== ГАЛЕРЕЯ =====

        public async Task AddGalleryImagesAsync(int blockId, IReadOnlyList<IBrowserFile> files)
        {
            var sortOrder = _gallery.GetMaxSortOrder(blockId) + 1;
            foreach (var file in files)
            {
                var path = await _files.SaveImageAsync(file);
                _gallery.Add(new GalleryImage { BlockId = blockId, ImagePath = path, SortOrder = sortOrder++ });
            }
        }

        public void DeleteGalleryImage(int id)            => _gallery.Delete(id);
        public void UpdateGalleryImageCaption(int id, string? caption) => _gallery.UpdateCaption(id, caption);

        // ===== ФАЙЛЫ =====

        public Task<string> SaveImageAsync(IBrowserFile file)    => _files.SaveImageAsync(file);
        public Task<string> SaveDocumentAsync(IBrowserFile file) => _files.SaveDocumentAsync(file);

    }
}
