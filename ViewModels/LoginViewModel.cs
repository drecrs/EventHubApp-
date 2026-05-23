using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHubApp.Data;
using EventHubApp.Services;

namespace EventHubApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AppDatabase _database;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public LoginViewModel(AppDatabase database, SessionService sessionService)
        {
            _database = database;
            _sessionService = sessionService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Missing Data", "Please enter email and password.", "OK");
                return;
            }

            var user = await _database.LoginAsync(Email.Trim(), Password);

            if (user == null)
            {
                await Shell.Current.DisplayAlert("Login Failed", "Invalid email or password.", "OK");
                return;
            }

            _sessionService.CurrentUser = user;
            await Shell.Current.GoToAsync("//events");
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }
    }
}
