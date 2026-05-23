using EventHubApp.ViewModels;

namespace EventHubApp.Views
{
    public partial class EventDetailsPage : ContentPage
    {
        public EventDetailsPage(EventDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
