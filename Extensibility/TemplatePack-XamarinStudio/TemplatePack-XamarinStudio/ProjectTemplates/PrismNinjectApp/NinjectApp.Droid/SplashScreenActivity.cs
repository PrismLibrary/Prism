
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace ${Namespace}
{
	[Activity(Label = "${ProjectName}", MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class SlashScreenActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
		}
	}
}

