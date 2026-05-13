using System;
using System.Collections.Generic;

namespace PortfolioConstruct.Models;

public partial class DesignSetting
{
    public int Id { get; set; }

    public int PortfolioId { get; set; }

    public string Color { get; set; } = null!;

    public string Font { get; set; } = null!;

    public virtual Portfolio Portfolio { get; set; } = null!;
}
