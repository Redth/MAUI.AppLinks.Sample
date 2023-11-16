
namespace AppLinksSample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

		protected override async void OnAppLinkRequestReceived(Uri uri)
		{
			base.OnAppLinkRequestReceived(uri);

            // Show an alert to test the app link worked

            await this.Dispatcher.DispatchAsync(() =>
            {
                this.Windows[0].Page!.DisplayAlert(
					"App Link Opened",
					uri.ToString(),
					"OK");
            });

			Console.WriteLine("APP LINK: " + uri.ToString());
		}

		protected override Window CreateWindow(IActivationState? activationState)
			=> new Window(new AppShell());
    }
}
