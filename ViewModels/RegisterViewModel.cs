using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHubApp.Data;
using EventHubApp.Models;

namespace EventHubApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AppDatabase _database;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string parish = string.Empty;

        [ObservableProperty]
        private string country = "Jamaica";

        [ObservableProperty]
        private string userType = "Patron";

        public List<string> UserTypes { get; } = new() { "Patron", "Promoter" };

        public RegisterViewModel(AppDatabase database)
        {
            _database = database;
        }

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Missing Data", "Name, email and password are required.", "OK");
                return;
            }

            var existingUser = await _database.GetUserByEmailAsync(Email.Trim());
            if (existingUser != null)
            {
                await Shell.Current.DisplayAlert("Duplicate Email", "That email is already registered.", "OK");
                return;
            }

            var user = new User
            {
                Name = Name.Trim(),
                Email = Email.Trim(),
                Password = Password,
                Parish = Parish.Trim(),
                Country = Country.Trim(),
                UserType = UserType
            };

            await _database.AddUserAsync(user);

            await Shell.Current.DisplayAlert("Success", "Registration completed. Please log in.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}
