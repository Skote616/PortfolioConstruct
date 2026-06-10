using PortfolioConstruct.Models;

namespace PortfolioConstruct.Services
{
    /// <summary>
    /// Сервис аутентификации — содержит бизнес-логику входа и регистрации.
    /// Репозиторий не знает про хеширование — это знает только сервис.
    /// </summary>
    public interface IAuthService
    {
        User? Login(string email, string password);
        bool Register(string email, string password);
    }
}
