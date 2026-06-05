namespace PortfolioConstruct.Models;

public class GalleryImage
{
    public int Id { get; set; }
    public int BlockId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
    public virtual Block Block { get; set; } = null!;
}
