using EventHubApp.ViewModels;

namespace EventHubApp.Views
{
    public partial class EventsPage : ContentPage
    {
        private readonly EventsViewModel _viewModel;
       
        public EventsPage(EventsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadEventsCommand.ExecuteAsync(null);
        }
    }
}
