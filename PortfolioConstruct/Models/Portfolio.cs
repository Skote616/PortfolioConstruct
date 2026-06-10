using System;
using System.Collections.Generic;

namespace PortfolioConstruct.Models;

public partial class Portfolio
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual DesignSetting? DesignSetting { get; set; }

    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();

    public virtual User User { get; set; } = null!;
}
