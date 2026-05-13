using System;
using System.Collections.Generic;

namespace PortfolioConstruct.Models;

public partial class Block
{
    public int Id { get; set; }

    public int SectionId { get; set; }

    // Тип блока: "text", "image", "link"
    public string Type { get; set; } = null!;

    // Основное содержимое: текст / путь к файлу / URL
    public string Content { get; set; } = string.Empty;

    // Подпись: подпись к фото, описание ссылки и т.п.
    public string? Caption { get; set; }

    public virtual Section Section { get; set; } = null!;
}
