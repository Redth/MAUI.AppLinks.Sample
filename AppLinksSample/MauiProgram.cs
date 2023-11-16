using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace AppLinksSample
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				})
				.ConfigureLifecycleEvents(lifecycle =>
				{
#if IOS || MACCATALYST
					lifecycle.AddiOS(ios =>
					{
						ios.FinishedLaunching((app, data)
							=> HandleAppLink(app.UserActivity));

						ios.ContinueUserActivity((app, userActivity, handler)
							=> HandleAppLink(userActivity));

						if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
						{
							ios.SceneWillConnect((scene, sceneSession, sceneConnectionOptions)
								=> HandleAppLink(sceneConnectionOptions.UserActivities.ToArray()
									.FirstOrDefault(a => a.ActivityType == Foundation.NSUserActivityType.BrowsingWeb)));

							ios.SceneContinueUserActivity((scene, userActivity)
								=> HandleAppLink(userActivity));
						}
					});
#elif ANDROID
					lifecycle.AddAndroid(android => {
						android.OnCreate((activity, bundle) =>
						{
							var action = activity.Intent?.Action;
							var data = activity.Intent?.Data?.ToString();

							if (action == Android.Content.Intent.ActionView && data is not null)
							{
								HandleAppLink(data);
							}
						});
					});
#endif
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}


#if IOS || MACCATALYST
		static bool HandleAppLink(Foundation.NSUserActivity? userActivity)
		{
			if (userActivity is not null && userActivity.ActivityType == Foundation.NSUserActivityType.BrowsingWeb && userActivity.WebPageUrl is not null)
			{
				HandleAppLink(userActivity.WebPageUrl.ToString());
				return true;
			}
			return false;
		}
#endif

		static void HandleAppLink(string url)
		{
			if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
			{
				App.Current?.SendOnAppLinkRequestReceived(uri);
			}
		}
	}
}
