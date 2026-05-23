using CommunityToolkit.Mvvm.Messaging;
using EventHubApp.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EventHubApp.Data;
using EventHubApp.Models;
using EventHubApp.Services;

namespace EventHubApp.ViewModels
{
    public partial class EventDetailsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly AppDatabase _database;
        private readonly SessionService _sessionService;

        [ObservableProperty]
        private EventItem eventItem = new();

        [ObservableProperty]
        private string flyerImageSource = string.Empty;

        [ObservableProperty]
        private bool hasFlyer;

        [ObservableProperty]
        private bool hasFlyerImage;

        [ObservableProperty]
        private bool hasFlyerMessage;

        [ObservableProperty]
        private string flyerMessage = "No flyer was added for this event.";

        [ObservableProperty]
        private bool isPromoter;

        public EventDetailsViewModel(AppDatabase database, SessionService sessionService)
        {
            _database = database;
            _sessionService = sessionService;
            IsPromoter = _sessionService.IsPromoter;

            WeakReferenceMessenger.Default.Register<EventUpdatedMessage>(this, async (recipient, message) =>
            {
                if (EventItem != null && EventItem.Id == message.Value)
                {
                    var updatedEvent = await _database.GetEventByIdAsync(message.Value);

                    if (updatedEvent != null)
                    {
                        EventItem = updatedEvent;
                        UpdateFlyerDisplay();
                    }
                }
            });
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsPromoter = _sessionService.IsPromoter;

            if (query.TryGetValue("Event", out var eventObject) && eventObject is EventItem selectedEvent)
            {
                EventItem = selectedEvent;
                UpdateFlyerDisplay();
            }
        }

        private void UpdateFlyerDisplay()
        {
            FlyerImageSource = EventItem.PromotionFilePath;
            HasFlyer = !string.IsNullOrWhiteSpace(EventItem.PromotionFilePath);

            if (!HasFlyer)
            {
                HasFlyerImage = false;
                HasFlyerMessage = true;
                FlyerMessage = "No flyer was added for this event.";
                return;
            }

            string file = EventItem.PromotionFilePath.ToLower();

            HasFlyerImage =
                file.EndsWith(".jpg") ||
                file.EndsWith(".jpeg") ||
                file.EndsWith(".png") ||
                file.EndsWith(".gif");

            HasFlyerMessage = !HasFlyerImage;

            FlyerMessage = HasFlyerImage
                ? string.Empty
                : "This flyer is not an image. Use the button below to open the file.";
        }

        [RelayCommand]
        private async Task OpenFlyer()
        {
            if (!HasFlyer)
            {
                await Shell.Current.DisplayAlert("No Flyer", "No flyer was added for this event.", "OK");
                return;
            }

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(EventItem.PromotionFilePath)
            });
        }

        [RelayCommand]
        private async Task OpenMap()
        {
            if (string.IsNullOrWhiteSpace(EventItem.Venue))
            {
                await Shell.Current.DisplayAlert("No Venue", "No venue was entered for this event.", "OK");
                return;
            }

            string encodedVenue = Uri.EscapeDataString(EventItem.Venue);
            string mapUrl = $"https://www.google.com/maps/search/?api=1&query={encodedVenue}";

            await Launcher.Default.OpenAsync(mapUrl);
        }

        [RelayCommand]
        private async Task EditEvent()
        {
            if (!_sessionService.IsPromoter)
            {
                await Shell.Current.DisplayAlert("Access Denied", "Only promoters can edit events.", "OK");
                return;
            }

            await Shell.Current.GoToAsync(nameof(Views.ManageEventPage), true,
                new Dictionary<string, object>
                {
                   { "Event", EventItem }
                });
        }

        [RelayCommand]
        private async Task DeleteEvent()
        {
            if (!_sessionService.IsPromoter)
            {
                await Shell.Current.DisplayAlert("Access Denied", "Only promoters can delete events.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert( "Delete Event",$"Are you sure you want to delete '{EventItem.Name}'?","Yes","No");

            if (!confirm)
                return;

            await _database.DeleteEventAsync(EventItem);

            await Shell.Current.DisplayAlert("Deleted", "The event has been deleted.", "OK");

            await Shell.Current.GoToAsync("..");
        }
    }
}
