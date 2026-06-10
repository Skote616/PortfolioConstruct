namespace PortfolioConstruct.Models;

public partial class Block
{
    public int    Id          { get; set; }
    public int    SectionId   { get; set; }

    // Внешний ключ на справочник типов
    public int    BlockTypeId { get; set; }

    // Строковый код оставляем как вычисляемое свойство для совместимости с кодом
    // который проверяет block.Type == "text" и т.д.
    // Заполняется при Include(b => b.BlockType)
    public string Type => BlockType?.Code ?? string.Empty;

    public string  Content   { get; set; } = string.Empty;
    public string? Caption   { get; set; }
    public string  TextAlign { get; set; } = "left";
    public int     FontSize  { get; set; } = 16;
    public string  TextColor { get; set; } = "#374151";
    public int     SortOrder { get; set; }

    public virtual Section   Section   { get; set; } = null!;
    public virtual BlockType BlockType { get; set; } = null!;
    public virtual ICollection<GalleryImage> GalleryImages { get; set; } = new List<GalleryImage>();
}
