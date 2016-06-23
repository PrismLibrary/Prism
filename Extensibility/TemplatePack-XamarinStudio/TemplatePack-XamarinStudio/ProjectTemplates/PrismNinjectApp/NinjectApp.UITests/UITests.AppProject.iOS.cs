using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace ${Namespace}
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest ()
		{
			app = ConfigureApp.iOS.StartApp ();
		}

		[Test]
		public void WelcomeTextIsDisplayed ()
		{
			AppResult[] results = app.WaitForElement (c => c.Marked ("Welcome to Xamarin Forms!"));
			app.Screenshot ("Welcome screen.");

			Assert.IsTrue (results.Any ());
		}
	}
}
