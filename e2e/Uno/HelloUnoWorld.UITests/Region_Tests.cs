using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Uno.UITest.Helpers.Queries;
using Query = System.Func<Uno.UITest.IAppQuery, Uno.UITest.IAppQuery>;

namespace Sample.UITests
{
    public class Region_Tests : TestBase
    {
        [Test]
        public void InitialView()
        {
            Query initialView = q => q.Marked("InitialViewTextBlock");

            App.WaitForElement(initialView);
            TakeScreenshot("Initial");

            App.WaitForDependencyPropertyValue(initialView, "Text", "Initial view");
        }

        [Test]
        public void TestViewA()
        {
            Query testSelector = q => q.Marked("viewAButton");
            Query viewASelector = q => q.Marked("viewAText");

            App.WaitForElement(testSelector);
            TakeScreenshot("Initial");

            App.Tap(testSelector);

            App.WaitForElement(viewASelector);

            TakeScreenshot("Done");
        }
    }
}
