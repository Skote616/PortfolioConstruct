namespace PortfolioConstruct.Models;

/// <summary>
/// Персональные данные студента — хранятся отдельно от учётных данных (User)
/// Связь 1:1 с User
/// </summary>
public class StudentProfile
{
    public int     Id           { get; set; }
    public int     UserId       { get; set; }
    public string? FullName     { get; set; }  // ФИО
    public string? GroupName    { get; set; }  // Группа, например "ИСП-23"
    public string? Specialty    { get; set; }  // Код специальности, например "09.02.07"
    public string? Phone        { get; set; }  // Телефон (опционально)
    public string? AvatarPath   { get; set; }  // Путь к фото профиля

    public virtual User User { get; set; } = null!;
}
