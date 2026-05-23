using EventHubApp.Models;

namespace EventHubApp.Services
{
    public class SessionService
    {
        public User? CurrentUser { get; set; }

        public bool IsLoggedIn => CurrentUser != null;
        public bool IsPromoter => CurrentUser?.UserType == "Promoter";

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}

