using Android.App;
using Android.Content.PM;
using Android.OS;

namespace PrismMauiDemo;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
#if PRISM_NATIVE_AOT_VALIDATION
        if (Intent?.GetBooleanExtra("prism-nativeaot-autorun", false) == true)
            System.Environment.SetEnvironmentVariable("PRISM_NATIVEAOT_AUTORUN", "1");
#endif
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
