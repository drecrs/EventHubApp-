using CommunityToolkit.Mvvm.Messaging;
using EventHubApp.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHubApp.Data;
using EventHubApp.Models;
using EventHubApp.Services;


namespace EventHubApp.ViewModels
{
    public partial class ManageEventViewModel : ObservableObject, IQueryAttributable
    {
        private readonly AppDatabase _database;
        private readonly SessionService _sessionService;

        private EventItem? existingEvent;

        [ObservableProperty]
        private string pageTitle = "Add New Event";

        [ObservableProperty]
        private string saveButtonText = "Save Event";

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private DateTime date = DateTime.Today;

        [ObservableProperty]
        private string venue = string.Empty;

        [ObservableProperty]
        private string ticketCost = string.Empty;

        [ObservableProperty]
        private string sponsors = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string promotionFilePath = string.Empty;

        [ObservableProperty]
        private bool isSocaSelected;

        [ObservableProperty]
        private bool isDancehallSelected;

        [ObservableProperty]
        private bool isCorporateSelected;

        [ObservableProperty]
        private bool isClubbingSelected;

        [ObservableProperty]
        private bool isSportsSelected;

        [ObservableProperty]
        private bool isFitnessSelected;

        [ObservableProperty]
        private bool isReligiousSelected;

        public ManageEventViewModel(AppDatabase database, SessionService sessionService)
        {
            _database = database;
            _sessionService = sessionService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Event", out var eventObject) && eventObject is EventItem selectedEvent)
            {
                existingEvent = selectedEvent;

                PageTitle = "Edit Event";
                SaveButtonText = "Update Event";

                Name = selectedEvent.Name;
                Date = selectedEvent.Date;
                Venue = selectedEvent.Venue;
                TicketCost = selectedEvent.TicketCost.ToString();
                Sponsors = selectedEvent.Sponsors;
                Description = selectedEvent.Description;
                PromotionFilePath = selectedEvent.PromotionFilePath;

                LoadSelectedCategories(selectedEvent.Category);
            }
            else
            {
                existingEvent = null;

                PageTitle = "Add New Event";
                SaveButtonText = "Save Event";

                ClearForm();
            }
        }

        private void LoadSelectedCategories(string categories)
        {
            IsSocaSelected = categories.Contains("Soca");
            IsDancehallSelected = categories.Contains("Dancehall");
            IsCorporateSelected = categories.Contains("Corporate");
            IsClubbingSelected = categories.Contains("Clubbing");
            IsSportsSelected = categories.Contains("Sports");
            IsFitnessSelected = categories.Contains("Fitness");
            IsReligiousSelected = categories.Contains("Religious");
        }

        private string GetSelectedCategories()
        {
            List<string> selectedCategories = new();

            if (IsSocaSelected)
                selectedCategories.Add("Soca");

            if (IsDancehallSelected)
                selectedCategories.Add("Dancehall");

            if (IsCorporateSelected)
                selectedCategories.Add("Corporate");

            if (IsClubbingSelected)
                selectedCategories.Add("Clubbing");

            if (IsSportsSelected)
                selectedCategories.Add("Sports");

            if (IsFitnessSelected)
                selectedCategories.Add("Fitness");

            if (IsReligiousSelected)
                selectedCategories.Add("Religious");

            return string.Join(", ", selectedCategories);
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Date = DateTime.Today;
            Venue = string.Empty;
            TicketCost = string.Empty;
            Sponsors = string.Empty;
            Description = string.Empty;
            PromotionFilePath = string.Empty;

            IsSocaSelected = false;
            IsDancehallSelected = false;
            IsCorporateSelected = false;
            IsClubbingSelected = false;
            IsSportsSelected = false;
            IsFitnessSelected = false;
            IsReligiousSelected = false;
        }

        [RelayCommand]
        private async Task PickFlyer()
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select event flyer"
            });

            if (result == null)
                return;

            string fileName = $"{Guid.NewGuid()}_{result.FileName}";
            string destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using Stream sourceStream = await result.OpenReadAsync();
            using FileStream destinationStream = File.Create(destinationPath);

            await sourceStream.CopyToAsync(destinationStream);

            PromotionFilePath = destinationPath;
        }

        [RelayCommand]
        private async Task SaveEvent()
        {
            if (!_sessionService.IsPromoter)
            {
                await Shell.Current.DisplayAlert("Access Denied", "Only promoters can add or edit events.", "OK");
                return;
            }

            string selectedCategories = GetSelectedCategories();

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Venue) ||
                string.IsNullOrWhiteSpace(selectedCategories))
            {
                await Shell.Current.DisplayAlert("Missing Data", "Please enter the event name, venue and at least one category.", "OK");
                return;
            }

            decimal.TryParse(TicketCost, out decimal cost);

            if (existingEvent == null)
            {
                var newEvent = new EventItem
                {
                    Name = Name.Trim(),
                    Date = Date,
                    Venue = Venue.Trim(),
                    TicketCost = cost,
                    Sponsors = Sponsors.Trim(),
                    Category = selectedCategories,
                    Description = Description.Trim(),
                    PromotionFilePath = PromotionFilePath,
                    CreatedByUserId = _sessionService.CurrentUser!.Id
                };

                await _database.AddEventAsync(newEvent);
                await Shell.Current.DisplayAlert("Success", "Event saved successfully.", "OK");
            }
            else
            {
                existingEvent.Name = Name.Trim();
                existingEvent.Date = Date;
                existingEvent.Venue = Venue.Trim();
                existingEvent.TicketCost = cost;
                existingEvent.Sponsors = Sponsors.Trim();
                existingEvent.Category = selectedCategories;
                existingEvent.Description = Description.Trim();
                existingEvent.PromotionFilePath = PromotionFilePath;
                
                await _database.UpdateEventAsync(existingEvent);
                WeakReferenceMessenger.Default.Send(new EventUpdatedMessage(existingEvent.Id));
                await Shell.Current.DisplayAlert("Success", "Event updated successfully.", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}
