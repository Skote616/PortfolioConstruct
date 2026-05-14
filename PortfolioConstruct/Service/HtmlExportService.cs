using PortfolioConstruct.Models;
using System.Text;

namespace PortfolioConstruct.Service
{
    public class HtmlExportService
    {
        /// <summary>
        /// Генерирует ZIP-архив с HTML + CSS + JS файлами портфолио
        /// </summary>
        public byte [ ] GenerateHtmlZip (Portfolio portfolio, string userEmail)
        {
            var html = GenerateHtml(portfolio, userEmail);
            var css = GenerateCss(portfolio);
            var js = GenerateJs( );

            // Упаковываем всё в ZIP в памяти
            using var ms = new MemoryStream( );
            using (var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                WriteEntry(zip, "index.html", html);
                WriteEntry(zip, "style.css", css);
                WriteEntry(zip, "script.js", js);

                // Копируем загруженные изображения если они есть на диске
                foreach (var section in portfolio.Sections)
                {
                    foreach (var block in section.Blocks)
                    {
                        if (block.Type == "image" && !string.IsNullOrWhiteSpace(block.Content))
                        {
                            // block.Content вида "/uploads/filename.jpg"
                            var localPath = Path.Combine("wwwroot", block.Content.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (File.Exists(localPath))
                            {
                                var entryName = "images/" + Path.GetFileName(localPath);
                                var entry = zip.CreateEntry(entryName);
                                using var entryStream = entry.Open( );
                                using var fileStream = File.OpenRead(localPath);
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
            }

            ms.Position = 0;
            return ms.ToArray( );
        }

        // ===== HTML =====
        private string GenerateHtml (Portfolio portfolio, string userEmail)
        {
            var sb = new StringBuilder( );
            var bgColor = portfolio.DesignSetting?.Color ?? "#f0f4f8";
            var font = portfolio.DesignSetting?.Font ?? "Arial";

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"ru\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"UTF-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"  <title>Портфолио — {userEmail}</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"style.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine($"<body style=\"background:{bgColor}; font-family:'{font}', sans-serif\">");

            // Шапка
            sb.AppendLine("  <header class=\"portfolio-header\">");
            sb.AppendLine($"    <div><h1 class=\"portfolio-name\">{userEmail}</h1>");
            sb.AppendLine("    <p class=\"portfolio-sub\">Студенческое портфолио</p></div>");
            sb.AppendLine("  </header>");

            // Навигация — генерируем вкладки
            sb.AppendLine("  <nav class=\"portfolio-nav\" id=\"main-nav\">");
            bool first = true;
            foreach (var section in portfolio.Sections)
            {
                if (!section.Blocks.Any( )) continue;
                var active = first ? " active" : "";
                sb.AppendLine($"    <button class=\"nav-btn{active}\" onclick=\"showSection('section-{section.Id}', this)\">{section.Title}</button>");
                first = false;
            }
            sb.AppendLine("  </nav>");

            // Разделы
            sb.AppendLine("  <main class=\"portfolio-content\">");
            first = true;
            foreach (var section in portfolio.Sections)
            {
                if (!section.Blocks.Any( )) continue;
                var display = first ? "block" : "none";
                sb.AppendLine($"    <section id=\"section-{section.Id}\" class=\"portfolio-section\" style=\"display:{display}\">");
                sb.AppendLine($"      <h2 class=\"section-heading\">{section.Title}</h2>");

                foreach (var block in section.Blocks)
                {
                    sb.AppendLine(RenderBlock(block));
                }

                sb.AppendLine("    </section>");
                first = false;
            }
            sb.AppendLine("  </main>");

            sb.AppendLine("  <script src=\"script.js\"></script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString( );
        }

        private string RenderBlock (Block block)
        {
            var caption = string.IsNullOrWhiteSpace(block.Caption) ? "" :
                $"<p class=\"block-caption\">{block.Caption}</p>";

            return block.Type switch
            {
                "text" => $"      <div class=\"block-text\">{block.Content}</div>",

                "image" when !string.IsNullOrWhiteSpace(block.Content) =>
                    $"      <div class=\"block-image\">" +
                    $"<img src=\"images/{Path.GetFileName(block.Content)}\" alt=\"{block.Caption ?? "Изображение"}\">" +
                    $"{caption}</div>",

                "link" when !string.IsNullOrWhiteSpace(block.Content) =>
                    $"      <div class=\"block-link\">" +
                    (string.IsNullOrWhiteSpace(block.Caption) ? "" : $"<p class=\"link-label\">{block.Caption}</p>") +
                    $"<a href=\"{block.Content}\" target=\"_blank\">🔗 {block.Content}</a></div>",

                _ => ""
            };
        }

        // ===== CSS =====
        private string GenerateCss (Portfolio portfolio)
        {
            var accent = "#4f6ef7";
            return $@"
*, *::before, *::after {{ box-sizing: border-box; margin: 0; padding: 0; }}

body {{
    min-height: 100vh;
    font-size: 16px;
    line-height: 1.6;
    color: #374151;
}}

/* Шапка */
.portfolio-header {{
    padding: 2rem 3rem;
    background: rgba(255,255,255,0.85);
    backdrop-filter: blur(6px);
    border-bottom: 3px solid {accent};
}}
.portfolio-name {{ font-size: 1.8rem; font-weight: 700; color: #1a1a2e; }}
.portfolio-sub  {{ color: #6b7280; font-size: 0.95rem; margin-top: 0.25rem; }}

/* Навигация */
.portfolio-nav {{
    display: flex;
    flex-wrap: wrap;
    gap: 0.25rem;
    padding: 0 2rem;
    background: rgba(255,255,255,0.7);
    border-bottom: 1px solid #e5e7eb;
    position: sticky;
    top: 0;
    z-index: 10;
}}
.nav-btn {{
    padding: 0.75rem 1.25rem;
    background: none;
    border: none;
    border-bottom: 3px solid transparent;
    color: #374151;
    font-size: 0.95rem;
    cursor: pointer;
    font-family: inherit;
    transition: all 0.15s;
}}
.nav-btn:hover {{ color: {accent}; }}
.nav-btn.active {{ color: {accent}; border-bottom-color: {accent}; font-weight: 600; }}

/* Основной контент */
.portfolio-content {{
    max-width: 800px;
    margin: 0 auto;
    padding: 2.5rem 2rem;
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
}}

.section-heading {{
    font-size: 1.5rem;
    font-weight: 700;
    color: {accent};
    padding-bottom: 0.5rem;
    border-bottom: 2px solid {accent};
    margin-bottom: 1rem;
}}

.portfolio-section {{
    display: flex;
    flex-direction: column;
    gap: 1.25rem;
}}

/* Блоки */
.block-text {{
    background: rgba(255,255,255,0.85);
    border-radius: 12px;
    padding: 1.25rem 1.5rem;
    font-size: 1rem;
    line-height: 1.7;
    white-space: pre-wrap;
    box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}}

.block-image {{
    border-radius: 12px;
    overflow: hidden;
    box-shadow: 0 2px 12px rgba(0,0,0,0.1);
    background: white;
}}
.block-image img {{
    width: 100%;
    display: block;
}}
.block-caption {{
    padding: 0.6rem 1rem;
    font-size: 0.85rem;
    color: #6b7280;
    font-style: italic;
    text-align: center;
    background: white;
}}

.block-link {{
    background: rgba(255,255,255,0.85);
    border-radius: 12px;
    padding: 1rem 1.5rem;
    box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}}
.link-label {{
    font-size: 0.9rem;
    font-weight: 600;
    color: #374151;
    margin-bottom: 0.4rem;
}}
.block-link a {{
    color: {accent};
    text-decoration: none;
    word-break: break-all;
}}
.block-link a:hover {{ text-decoration: underline; }}

/* Адаптив */
@media (max-width: 600px) {{
    .portfolio-header {{ padding: 1.5rem; }}
    .portfolio-content {{ padding: 1.5rem 1rem; }}
    .portfolio-name {{ font-size: 1.4rem; }}
}}
";
        }

        // ===== JS =====
        private string GenerateJs () => @"
// Переключение разделов
function showSection(sectionId, btn) {
    // Скрываем все разделы
    document.querySelectorAll('.portfolio-section').forEach(s => s.style.display = 'none');
    // Убираем active у всех кнопок
    document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));

    // Показываем нужный раздел
    document.getElementById(sectionId).style.display = 'block';
    btn.classList.add('active');

    // Прокрутка к контенту на мобильных
    document.querySelector('.portfolio-content').scrollIntoView({ behavior: 'smooth', block: 'start' });
}
";

        // ===== Вспомогательное =====
        private static void WriteEntry (System.IO.Compression.ZipArchive zip, string name, string content)
        {
            var entry = zip.CreateEntry(name);
            using var writer = new StreamWriter(entry.Open( ), Encoding.UTF8);
            writer.Write(content);
        }
    }
}