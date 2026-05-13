using PortfolioConstruct.Models;
using Microsoft.EntityFrameworkCore;

namespace PortfolioConstruct.Service
{
    // DBService теперь не Singleton — он получает контекст через DI
    // Это безопасно для Blazor Server: каждое подключение имеет свой экземпляр
    public class DBService
    {
        private readonly PortfolioContext _context;

        public DBService(PortfolioContext context)
        {
            _context = context;
        }

        // ===== ПОЛЬЗОВАТЕЛИ =====

        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? Login(string email, string password)
        {
            // Хешируем введённый пароль и сравниваем с хранимым
            var hash = HashPassword(password);
            return _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);
        }

        public bool Register(string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
                return false; // email уже занят

            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password)
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        // ===== ПОРТФОЛИО =====

        public Portfolio? GetPortfolio(int userId)
        {
            // AsNoTracking — EF не отслеживает эти объекты,
            // поэтому несохранённые изменения не утекут в БД при следующем SaveChanges
            return _context.Portfolios
                .AsNoTracking()
                .Include(p => p.Sections)
                    .ThenInclude(s => s.Blocks)
                .Include(p => p.DesignSetting)
                .FirstOrDefault(p => p.UserId == userId);
        }

        public Portfolio GetOrCreatePortfolio(int userId)
        {
            var portfolio = GetPortfolio(userId);
            if (portfolio != null) return portfolio;

            // Создаём портфолио с пятью фиксированными разделами из задания
            portfolio = new Portfolio
            {
                UserId = userId,
                About = string.Empty
            };
            _context.Portfolios.Add(portfolio);
            _context.SaveChanges();

            var fixedSections = new[]
            {
                "О себе", "Достижения", "Работы", "Документы", "Я гражданин"
            };

            foreach (var title in fixedSections)
            {
                _context.Sections.Add(new Section
                {
                    PortfolioId = portfolio.Id,
                    Title = title,
                    IsCustom = false
                });
            }

            // Настройки дизайна по умолчанию
            _context.DesignSettings.Add(new DesignSetting
            {
                PortfolioId = portfolio.Id,
                Color = "#ffffff",
                Font = "Arial"
            });

            _context.SaveChanges();

            // Перечитываем с Include чтобы вернуть полный объект
            return GetPortfolio(userId)!;
        }

        // ===== РАЗДЕЛЫ =====

        public void AddCustomSection(int portfolioId, string title)
        {
            _context.Sections.Add(new Section
            {
                PortfolioId = portfolioId,
                Title = title,
                IsCustom = true
            });
            _context.SaveChanges();
        }

        public void DeleteSection(int sectionId)
        {
            var section = _context.Sections
                .Include(s => s.Blocks)
                .FirstOrDefault(s => s.Id == sectionId);

            if (section == null || !section.IsCustom) return; // фиксированные не удаляем

            _context.Sections.Remove(section);
            _context.SaveChanges();
        }

        // ===== БЛОКИ =====

        public void AddBlock(int sectionId, string type, string content, string? caption = null)
        {
            _context.Blocks.Add(new Block
            {
                SectionId = sectionId,
                Type = type,
                Content = content,
                Caption = caption
            });
            _context.SaveChanges();
        }

        public void UpdateBlock(Block block)
        {
            _context.Blocks.Update(block);
            _context.SaveChanges();
        }

        public void DeleteBlock(int blockId)
        {
            var block = _context.Blocks.Find(blockId);
            if (block == null) return;
            _context.Blocks.Remove(block);
            _context.SaveChanges();
        }

        // ===== ДИЗАЙН =====

        public void UpdateDesign(DesignSetting settings)
        {
            _context.DesignSettings.Update(settings);
            _context.SaveChanges();
        }


        // ===== ФАЙЛЫ =====

        // Сохраняет загруженный файл на диск и возвращает путь для хранения в БД
        public async Task<string> SaveImageAsync(Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
        {
            // Папка uploads внутри wwwroot
            var uploadsDir = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsDir);

            // Уникальное имя файла чтобы не было коллизий
            var ext = Path.GetExtension(file.Name);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            // Максимум 10 МБ
            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(fs);

            // Возвращаем относительный путь для <img src="...">
            return $"/uploads/{fileName}";
        }
        // ===== ВСПОМОГАТЕЛЬНОЕ =====

        // Простое SHA256-хеширование пароля
        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}