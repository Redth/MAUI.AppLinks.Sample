namespace AppLinksSample
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();
		}

		protected override void OnAppLinkRequestReceived(Uri uri)
		{
			base.OnAppLinkRequestReceived(uri);

			// Show an alert to test the app link worked
			this.Windows.First().Page!.DisplayAlert(
				"App Link Opened",
				uri.ToString(),
				"OK");

			Console.WriteLine("APP LINK: " + uri.ToString());
		}
	}
}
