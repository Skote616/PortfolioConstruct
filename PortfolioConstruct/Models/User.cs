namespace PortfolioConstruct.Models;

public partial class User
{
    public int    Id           { get; set; }
    public string Email        { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    // Внешний ключ на справочник ролей
    public int    RoleId       { get; set; } = 1; // 1 = User по умолчанию

    public bool     IsArchived { get; set; } = false;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;

    public virtual Role            Role           { get; set; } = null!;
    public virtual Portfolio?      Portfolio      { get; set; }
    public virtual StudentProfile? StudentProfile { get; set; }
}
