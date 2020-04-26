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
    public class Module_Tests : TestBase
    {
        [Test]
        public void ModulePageA()
        {
            Query modulesSelector = q => q.Marked("showModules");
            Query testSelector = q => q.Marked("ModuleViewAButton");
            Query moduleEntry = q => q.Marked("ModuleAModule");
            Query endResult = q => q.Marked("ModulePageATextBlock");

            TakeScreenshot("Initial");

            App.WaitForElement(modulesSelector);

            App.Tap(modulesSelector);

            App.WaitForElement(moduleEntry);

            TakeScreenshot("Opened");

            App.Tap(moduleEntry);

            TakeScreenshot("Tapped Module");

            App.Tap(testSelector);

            TakeScreenshot("Tapped ViewA");

            App.WaitForElement(endResult);

            App.WaitForDependencyPropertyValue(endResult, "Text", "ModulePage A");
        }
    }
}
