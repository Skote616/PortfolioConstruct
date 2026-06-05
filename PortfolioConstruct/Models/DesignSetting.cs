namespace PortfolioConstruct.Models;

public partial class DesignSetting
{
    public int Id { get; set; }
    public int PortfolioId { get; set; }

    // Старые поля
    public string Color { get; set; } = "#ffffff";
    public string Font  { get; set; } = "Arial";

    // Тема оформления: "light", "dark", "ocean", "forest", "sunset"
    public string Theme { get; set; } = "light";

    // Акцентный цвет (цвет заголовков, кнопок)
    public string AccentColor { get; set; } = "#4f6ef7";

    public virtual Portfolio Portfolio { get; set; } = null!;
}
