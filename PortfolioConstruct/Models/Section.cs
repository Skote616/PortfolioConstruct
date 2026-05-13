using System;
using System.Collections.Generic;

namespace PortfolioConstruct.Models;

public partial class Section
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsCustom { get; set; }

    public virtual ICollection<Block> Blocks { get; set; } = new List<Block>();

    public virtual Portfolio Portfolio { get; set; } = null!;
}
