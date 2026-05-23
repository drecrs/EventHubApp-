using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHubApp.Data;
using EventHubApp.Services;

namespace EventHubApp.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AppDatabase _database;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string parish = string.Empty;

        [ObservableProperty]
        private string country = string.Empty;

        [ObservableProperty]
        private string userType = "Patron";

        public List<string> UserTypes { get; } = new()
       {
           "Patron",
           "Promoter"
       };

        public ProfileViewModel(AppDatabase database, SessionService sessionService)
        {
            _database = database;
            _sessionService = sessionService;
        }

        public void LoadProfile()
        {
            var user = _sessionService.CurrentUser;

            if (user == null)
                return;

            Name = user.Name;
            Email = user.Email;
            Password = user.Password;
            Parish = user.Parish;
            Country = user.Country;
            UserType = user.UserType;
        }

        [RelayCommand]
        private async Task SaveProfile()
        {
            var user = _sessionService.CurrentUser;

            if (user == null)
            {
                await Shell.Current.DisplayAlert("Error", "No user is currently logged in.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Missing Data", "Name and password are required.", "OK");
                return;
            }

            user.Name = Name.Trim();
            user.Password = Password;
            user.Parish = Parish.Trim();
            user.Country = Country.Trim();
            user.UserType = UserType;

            await _database.UpdateUserAsync(user);

            _sessionService.CurrentUser = user;

            await Shell.Current.DisplayAlert("Saved", "Your profile has been updated.", "OK");

            await Shell.Current.GoToAsync("//events");
        }
    }
}
