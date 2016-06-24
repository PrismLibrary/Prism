using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace ${Namespace}
{
    [Activity( Label = "${ProjectName}", MainLauncher = true, NoHistory = true, Theme = "@style/SplashTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation )]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate( savedInstanceState );
            var intent = new Intent( this, typeof( MainActivity ) );
            StartActivity( intent );
        }
    }
}