namespace PortfolioConstruct.Models
{
    // Scoped — создаётся отдельно для каждого подключения в Blazor Server
    // В отличие от static, каждый пользователь имеет свою копию
    public class SessionService
    {
        public int CurrentUserId { get; set; }
        public string CurrentUserEmail { get; set; } = string.Empty;
        public bool IsLoggedIn => CurrentUserId > 0;

        public void Login(User user)
        {
            CurrentUserId = user.Id;
            CurrentUserEmail = user.Email;
        }

        public void Logout()
        {
            CurrentUserId = 0;
            CurrentUserEmail = string.Empty;
        }
    }
}
