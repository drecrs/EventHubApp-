using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHubApp.Data;
using EventHubApp.Models;
using EventHubApp.Services;
using System.Collections.ObjectModel;

namespace EventHubApp.ViewModels
{
    public partial class EventsViewModel : ObservableObject
    {
        private readonly AppDatabase _database;
        private readonly SessionService _sessionService;

        private List<EventItem> allEvents = new();

        public ObservableCollection<EventItem> Events { get; } = new();

        public List<string> Categories { get; } = new()
       {
           "All",
           "Soca",
           "Dancehall",
           "Corporate",
           "Clubbing",
           "Sports",
           "Fitness"
       };

        [ObservableProperty]
        private string welcomeMessage = "Events";

        [ObservableProperty]
        private bool isPromoter;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string selectedCategory = "All";

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool useDateFilter;

        [ObservableProperty]
        private bool showFilters;


        public EventsViewModel(AppDatabase database, SessionService sessionService)
        {
            _database = database;
            _sessionService = sessionService;
        }

        [RelayCommand]
        private void ToggleFilters()
        {
            ShowFilters = !ShowFilters;
        }

        [RelayCommand]
        public async Task LoadEvents()
        {
            await _database.SeedSampleEventsAsync();

            allEvents = await _database.GetEventsAsync();

            if (_sessionService.CurrentUser != null)
            {
                WelcomeMessage = $"Welcome, {_sessionService.CurrentUser.Name} ({_sessionService.CurrentUser.UserType})";
                IsPromoter = _sessionService.IsPromoter;
            }

            ApplyFilters();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilters();
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            ApplyFilters();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            ApplyFilters();
        }

        partial void OnUseDateFilterChanged(bool value)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (allEvents == null)
                return;

            IEnumerable<EventItem> filteredEvents = allEvents;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.Trim().ToLower();

                filteredEvents = filteredEvents.Where(e =>
                    e.Name.ToLower().Contains(search) ||
                    e.Venue.ToLower().Contains(search) ||
                    e.Description.ToLower().Contains(search) ||
                    e.Sponsors.ToLower().Contains(search) ||
                    e.Category.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(SelectedCategory) && SelectedCategory != "All")
            {
                filteredEvents = filteredEvents.Where(e =>
                    e.Category.Contains(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (UseDateFilter)
            {
                filteredEvents = filteredEvents.Where(e =>
                    e.Date.Date == SelectedDate.Date);
            }

            Events.Clear();

            foreach (var eventItem in filteredEvents.OrderBy(e => e.Date))
            {
                Events.Add(eventItem);
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = "All";
            SelectedDate = DateTime.Today;
            UseDateFilter = false;

            ApplyFilters();
        }

        [RelayCommand]
        private async Task Logout()
        {
            _sessionService.Logout();
            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        private async Task GoToAddEvent()
        {
            if (!_sessionService.IsPromoter)
            {
                await Shell.Current.DisplayAlert("Access Denied", "Only promoters can add events.", "OK");
                return;
            }

            await Shell.Current.GoToAsync(nameof(Views.ManageEventPage));
        }

        [RelayCommand]
        private async Task ViewDetails(EventItem eventItem)
        {
            if (eventItem == null)
                return;

            await Shell.Current.GoToAsync(nameof(Views.EventDetailsPage), true,
                new Dictionary<string, object>
                {
                   { "Event", eventItem }
                });
        }
    }
}
