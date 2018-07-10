using System;
using System.Reflection;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Xunit.Runners.UI;
using Xunit.Sdk;

namespace Prism.Forms.DroidRunner.Test
{
    [Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            //AddTestAssembly(Assembly.GetExecutingAssembly());

            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            // or in any reference assemblies			

            AddTestAssembly(typeof(Prism.Forms.Tests.Common.PageUtilitiesFixture).Assembly);
            AddTestAssembly(typeof(Prism.Autofac.Forms.Tests.Fixtures.ContainerProviderFixture).Assembly);
            AddTestAssembly(typeof(Prism.DryIoc.Forms.Tests.Fixtures.ContainerProviderFixture).Assembly);
            AddTestAssembly(typeof(Prism.Unity.Forms.Tests.Fixtures.ContainerProviderFixture).Assembly);
            // or in any assembly that you load (since JIT is available)

#if false
            // you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
			Writer = new TcpTextWriter ("10.0.1.2", 16384);
			// start running the test suites as soon as the application is loaded
			AutoStart = true;
			// crash the application (to ensure it's ended) and return to springboard
			TerminateAfterExecution = true;
#endif
            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}

