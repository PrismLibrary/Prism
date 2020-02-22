using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace HelloWorld.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public abstract class BaseTestFixture
    {
        protected IApp app => AppManager.App;
        protected bool OnAndroid => AppManager.Platform == Platform.Android;
        protected bool OniOS => AppManager.Platform == Platform.iOS;

        protected BaseTestFixture(Platform platform)
        {
            AppManager.Platform = platform;
        }

        [SetUp]
        public virtual void BeforeEachTest()
        {
            AppManager.StartApp();
        }

        // You can edit this file to define functionality that is common across many or all tests.
        // For example, you could add a method here to log in or dismiss a welcome dialogue.
    }
}