using EventHubApp.Services;
using EventHubApp.Views;

namespace EventHubApp
{
    public partial class AppShell : Shell
    {
        private readonly SessionService _sessionService;

        public AppShell(SessionService sessionService)
        {
            InitializeComponent();

            _sessionService = sessionService;

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ManageEventPage), typeof(ManageEventPage));
            Routing.RegisterRoute(nameof(EventDetailsPage), typeof(EventDetailsPage));
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            FlyoutIsPresented = false;
            await GoToAsync("//profile");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            _sessionService.Logout();
            FlyoutIsPresented = false;
            await GoToAsync("//login");
        }
    }
}
