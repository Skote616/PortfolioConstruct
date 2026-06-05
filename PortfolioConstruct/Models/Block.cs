namespace PortfolioConstruct.Models;

public partial class Block
{
    public int Id { get; set; }
    public int SectionId { get; set; }

    // Тип: "text", "image", "link", "gallery"
    public string Type { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public string? Caption { get; set; }

    // Настройки дизайна блока
    public string TextAlign  { get; set; } = "left";   // left | center | right
    public int    FontSize   { get; set; } = 16;        // px
    public string TextColor  { get; set; } = "#374151"; // hex

    public virtual Section Section { get; set; } = null!;
    public virtual ICollection<GalleryImage> GalleryImages { get; set; } = new List<GalleryImage>();
}
