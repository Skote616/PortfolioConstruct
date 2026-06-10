namespace PortfolioConstruct.Models
{
    public class SessionService
    {
        public int    CurrentUserId   { get; private set; }
        public string CurrentUserEmail { get; private set; } = string.Empty;
        public string CurrentUserRole  { get; private set; } = "User";

        public bool IsLoggedIn => CurrentUserId > 0;
        public bool IsAdmin    => IsLoggedIn && CurrentUserRole == "Admin";

        public void Login(User user)
        {
            CurrentUserId    = user.Id;
            CurrentUserEmail = user.Email;
            // Role загружается через Include в репозитории
            CurrentUserRole  = user.Role?.Code ?? "User";
        }

        public void Logout()
        {
            CurrentUserId    = 0;
            CurrentUserEmail = string.Empty;
            CurrentUserRole  = "User";
        }
    }
}
