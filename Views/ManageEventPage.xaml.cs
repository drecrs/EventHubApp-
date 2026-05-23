using EventHubApp.ViewModels;

namespace EventHubApp.Views
{
    public partial class ManageEventPage : ContentPage
    {
        public ManageEventPage(ManageEventViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
