namespace PortfolioConstruct.Models;

public class BlockType
{
    public int    Id       { get; set; }
    public string Code     { get; set; } = null!;
    public string Icon     { get; set; } = "📎";
    public bool   IsActive { get; set; } = true;

    public virtual ICollection<Block> Blocks { get; set; } = new List<Block>();
}
