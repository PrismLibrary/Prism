
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace $safeprojectname$
{
    [Activity( Label = "$saferootprojectname$", MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation )]
    public class SlashScreenActivity : Activity
    {
        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate( savedInstanceState );

            var intent = new Intent( this, typeof( MainActivity ) );
            StartActivity( intent );
        }
    }
}