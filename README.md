# MAUI.AppLinks.Sample
Sample .NET MAUI app which supports deep linking


# Android

## 1. Verify Domain Ownership

First you need to verify your ownership of the domain in the [Google Search Console](https://search.google.com/search-console).

## 2. Host .well-known Association file

Create an `assetlinks.json` file hosted on your domain's server under the `/.well-known/` folder.  Your URL should look like `https://redth.dev/.well-known/assetlinks.json`.  The file contents will need to include:

```
[
    {
        "relation": ["delegate_permission/common.handle_all_urls"],
        "target": {
            "namespace": "android_app",
            "package_name": "dev.redth.applinkssample",
            "sha256_cert_fingerprints":
            [
                "AA:BB:CC:DD:EE:FF:00:11:22:33:44:55:66:77:88:99:10:11:12:13:14:15:16:17:18:19:20:21:22:23:24:25"
            ]
        }
    }
]
```

Be sure to find your .keystore SHA256 fingerprints for your app.  In this case I have only included my androiddebug.keystore file's fingerprint which is used by default to sign .NET android apps.

You can use the [Statement List Generator Tool](https://developers.google.com/digital-asset-links/tools/generator) to help generate the correct json, but also to validate it is setup correctly.

## 3. Setup your Android Activity

You can reuse the `Platforms/Android/MainActivity.cs` in your MAUI app by adding the following class attribute to it:

```
[IntentFilter(
    new string[] { Intent.ActionView },
    AutoVerify = true,
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "https",
    DataHost = "redth.dev")]
```

Note that you should use your own data scheme and host values.  It's possible to associate multiple schemes/hosts here too.

You will also need to mark your activity as exportable.  You can do this by adding the `Exported = true` property to the existing `[Activity(...)]` attribute.

## 4. Handle the Lifecycle events for the Intent activation

In your `MauiProgram.cs` file, setup your lifecycle events with the `builder`:

```
builder.ConfigureLifecycleEvents(lifecycle =>
{
    #if IOS || MACCATALYST
        // ...
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
```

### 5. Testing A URL

You can use `adb` to simulate opening a URL to ensure your app links work correctly:

```
adb shell am start -a android.intent.action.VIEW -c android.intent.category.BROWSABLE -d "https://redth.dev/items/1234"
```


# iOS

## 1. Host .well-known Association File

```
{
  "applinks": {
      "details": [
           {
             "appIDs": [ "TEAMIDHERE.dev.redth.applinkssample" ],
             "components": [
               {
                  "#": "no_universal_links",
                  "exclude": true,
                  "comment": "Matches any URL with a fragment that equals no_universal_links and instructs the system not to open it as a universal link."
               },
               {
                  "/": "/*",
                  "comment": "Matches any URL with a path that starts with /."
               }
             ]
           }
       ]
   }
}
```

## 2. Add Domain Association Entitlements to your App

You will need to add custom entitlements to your app to declare the associated domain(s).  You can do this either by adding an Entitlements.plist file to your app, or you can simply add the following to your .csproj file in your MAUI app:

```
<ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios' Or $([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">
	<CustomEntitlements Include="com.apple.developer.associated-domains" Type="StringArray" Value="applinks:redth.dev" />
</ItemGroup>
```

Be sure to replace the `applinks:redth.dev` with the correct value for your own domain.  Also notice the `ItemGroup`'s `Condition` which only includes the entitlement when the app is built for iOS or MacCatalyst.


## 3. Add Lifecycle Handlers

In your `MauiProgram.cs` file, setup your lifecycle events with the `builder`:

```
builder.ConfigureLifecycleEvents(lifecycle =>
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
        // ...
    #endif
});
```