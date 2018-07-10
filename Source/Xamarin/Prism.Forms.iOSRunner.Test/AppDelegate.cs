﻿using System.Reflection;
using Foundation;
using UIKit;
using Xunit.Runner;
using Xunit.Sdk;

namespace Prism.Forms.iOSRunner.Test
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : RunnerAppDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //Hack to get around calling UIKit outside the UI thread for IOC specific tests
            UIApplication.CheckForIllegalCrossThreadCalls = false;

            // We need this to ensure the execution assembly is part of the app bundle
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            // tests can be inside the main assembly
            //AddTestAssembly(Assembly.GetExecutingAssembly());
            // otherwise you need to ensure that the test assemblies will 
            // become part of the app bundle
            AddTestAssembly(typeof(Prism.Forms.Tests.Common.PageUtilitiesFixture).Assembly);
            AddTestAssembly(typeof(Prism.Autofac.Forms.Tests.Fixtures.ContainerProviderFixture).Assembly);
            AddTestAssembly(typeof(Prism.DryIoc.Forms.Tests.Fixtures.ContainerProviderFixture).Assembly);
            AddTestAssembly(typeof(Prism.Unity.Forms.Tests.Fixtures.ContainerProviderFixture).Assembly);

#if false
            // you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
			Writer = new TcpTextWriter ("10.0.1.2", 16384);
			// start running the test suites as soon as the application is loaded
			AutoStart = true;
			// crash the application (to ensure it's ended) and return to springboard
			TerminateAfterExecution = true;
#endif
            return base.FinishedLaunching(app, options);
        }
    }
}

