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
    public class Dialog_Tests : TestBase
    {
        [Test]
        public void ShowDialog()
        {
            Query testSelector = q => q.Marked("showDialog");
            Query OkSelector = q => q.Marked("OKButton");

            App.WaitForElement(testSelector);
            TakeScreenshot("Initial");

            App.Tap(testSelector);

            App.WaitForElement(OkSelector);

            TakeScreenshot("Opened");

            App.Tap(OkSelector);

            TakeScreenshot("Closed");
        }
    }
}
