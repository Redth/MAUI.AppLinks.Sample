using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using Microsoft.Maui.Platform;
using System.Diagnostics;

namespace AppLinksSample
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        Exported = true,
        ConfigurationChanges = ConfigChanges.ScreenSize
            | ConfigChanges.Orientation 
            | ConfigChanges.UiMode
            | ConfigChanges.ScreenLayout
            | ConfigChanges.SmallestScreenSize
            | ConfigChanges.KeyboardHidden
            | ConfigChanges.Density)]
    [IntentFilter(
       new string[] { Intent.ActionView },
       AutoVerify = true,
       Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
       DataScheme = "https",
       DataHost = "redth.dev")]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
