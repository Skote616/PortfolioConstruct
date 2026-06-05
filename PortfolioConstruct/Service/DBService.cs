using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Service
{
    public class DBService
    {
        private readonly PortfolioContext _context;

        public DBService(PortfolioContext context)
        {
            _context = context;
        }

        // ===== ПОЛЬЗОВАТЕЛИ =====

        public User? GetUserByEmail(string email)
            => _context.Users.FirstOrDefault(u => u.Email == email);

        public User? Login(string email, string password)
        {
            var hash = HashPassword(password);
            return _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
        }

        public bool Register(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email)) return false;
            _context.Users.Add(new User { Email = email, PasswordHash = HashPassword(password) });
            _context.SaveChanges();
            return true;
        }

        // ===== ПОРТФОЛИО =====

        public Portfolio? GetPortfolio(int userId)
        {
            return _context.Portfolios
                .AsNoTracking()
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Blocks)
                        .ThenInclude(b => b.GalleryImages.OrderBy(g => g.SortOrder))
                .Include(p => p.DesignSetting)
                .FirstOrDefault(p => p.UserId == userId);
        }

        public Portfolio GetOrCreatePortfolio(int userId)
        {
            var portfolio = GetPortfolio(userId);
            if (portfolio != null) return portfolio;

            portfolio = new Portfolio { UserId = userId, About = string.Empty };
            _context.Portfolios.Add(portfolio);
            _context.SaveChanges();

            var fixedSections = new[] { "О себе", "Достижения", "Работы", "Документы", "Я гражданин" };
            foreach (var title in fixedSections)
                _context.Sections.Add(new Section { PortfolioId = portfolio.Id, Title = title, IsCustom = false });

            _context.DesignSettings.Add(new DesignSetting { PortfolioId = portfolio.Id, Color = "#ffffff", Font = "Arial", Theme = "light", AccentColor = "#4f6ef7" });
            _context.SaveChanges();

            return GetPortfolio(userId)!;
        }

        // ===== РАЗДЕЛЫ =====

        public void AddCustomSection(int portfolioId, string title)
        {
            _context.Sections.Add(new Section { PortfolioId = portfolioId, Title = title, IsCustom = true });
            _context.SaveChanges();
        }

        public void DeleteSection(int sectionId)
        {
            var section = _context.Sections.Include(s => s.Blocks).ThenInclude(b => b.GalleryImages)
                .FirstOrDefault(s => s.Id == sectionId);
            if (section == null || !section.IsCustom) return;
            _context.Sections.Remove(section);
            _context.SaveChanges();
        }

        // ===== БЛОКИ =====

        public void AddBlock(int sectionId, string type, string content, string? caption = null)
        {
            _context.Blocks.Add(new Block { SectionId = sectionId, Type = type, Content = content, Caption = caption });
            _context.SaveChanges();
        }

        public void UpdateBlock(Block block)
        {
            _context.Blocks
                .Where(b => b.Id == block.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(b => b.Content,   block.Content)
                    .SetProperty(b => b.Caption,   block.Caption)
                    .SetProperty(b => b.TextAlign, block.TextAlign)
                    .SetProperty(b => b.FontSize,  block.FontSize)
                    .SetProperty(b => b.TextColor, block.TextColor));
        }

        public void DeleteBlock(int blockId)
        {
            var block = _context.Blocks.Include(b => b.GalleryImages).FirstOrDefault(b => b.Id == blockId);
            if (block == null) return;
            _context.Blocks.Remove(block);
            _context.SaveChanges();
        }

        // ===== ДИЗАЙН =====

        public void UpdateDesign(DesignSetting settings)
        {
            _context.DesignSettings
                .Where(d => d.Id == settings.Id)
                .ExecuteUpdate(s => s
                    .SetProperty(d => d.Color,       settings.Color)
                    .SetProperty(d => d.Font,        settings.Font)
                    .SetProperty(d => d.Theme,       settings.Theme)
                    .SetProperty(d => d.AccentColor, settings.AccentColor));
        }

        // ===== ГАЛЕРЕЯ =====

        public void AddGalleryImage(int blockId, string imagePath, string? caption, int sortOrder)
        {
            _context.GalleryImages.Add(new GalleryImage
            {
                BlockId = blockId,
                ImagePath = imagePath,
                Caption = caption,
                SortOrder = sortOrder
            });
            _context.SaveChanges();
        }

        public void DeleteGalleryImage(int galleryImageId)
        {
            var img = _context.GalleryImages.Find(galleryImageId);
            if (img == null) return;
            _context.GalleryImages.Remove(img);
            _context.SaveChanges();
        }

        public void UpdateGalleryImageCaption(int galleryImageId, string? caption)
        {
            _context.GalleryImages
                .Where(g => g.Id == galleryImageId)
                .ExecuteUpdate(s => s.SetProperty(g => g.Caption, caption));
        }

        // ===== ФАЙЛЫ =====

        public async Task<string> SaveImageAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
        {
            var uploadsDir = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsDir);
            var ext = Path.GetExtension(file.Name);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);
            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(fs);
            return $"/uploads/{fileName}";
        }

        public async Task<string> SaveDocumentAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
        {
            var docsDir = Path.Combine("wwwroot", "documents");
            Directory.CreateDirectory(docsDir);
            var ext = Path.GetExtension(file.Name);
            var safeName = Path.GetFileNameWithoutExtension(file.Name)
                .Replace(" ", "_")
                .Replace(",", "")
                .Replace(";", "");
            var fileName = $"{safeName}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(docsDir, fileName);
            await using var fs = new FileStream(fullPath, FileMode.Create);
            // Документы до 50 МБ
            await file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024).CopyToAsync(fs);
            // Content хранит путь, Caption хранит оригинальное имя файла
            return $"/documents/{fileName}|{file.Name}";
        }

        // ===== ВСПОМОГАТЕЛЬНОЕ =====

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
