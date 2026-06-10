namespace PortfolioConstruct.Models;

/// <summary>
/// Справочник ролей пользователей
/// </summary>
public class Role
{
    public int    Id          { get; set; }
    public string Code        { get; set; } = null!; // "User", "Admin"
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
