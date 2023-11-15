using Foundation;
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
                        ios.SceneWillConnect((scene, sceneSession, sceneConnectionOptions) =>
                        {
                            var appLinkUserActivity = sceneConnectionOptions.UserActivities.ToArray().FirstOrDefault(act => act.ActivityType == NSUserActivityType.BrowsingWeb);

                            if (appLinkUserActivity is not null && appLinkUserActivity.WebPageUrl is not null)
                            {
                                HandleAppLink(appLinkUserActivity.WebPageUrl.ToString());
                            }
                        });

                        ios.SceneContinueUserActivity((scene, userActivity) =>
                        {
                            if (userActivity.ActivityType == NSUserActivityType.BrowsingWeb && userActivity.WebPageUrl is not null)
                            {
                                return HandleAppLink(userActivity.WebPageUrl.ToString());
                            }

                            return false;
                        });
                    });
#elif ANDROID
                    lifecycle.AddAndroid(android => {
                        android.OnCreate((activity, bundle) =>
                        {
                            var action = activity.Intent?.Action;
                            var data = activity.Intent?.Data?.ToString();

                            if (action == Intent.ActionView && data is not null)
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


        static bool HandleAppLink(string url)
        {
            if (url.Contains("redth.dev"))
            {
                return true;
            }

            return false;
        }
    }
}
