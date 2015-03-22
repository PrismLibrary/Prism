// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.ViewModel;

namespace Prism.Tests.Mvvm
{
    [TestClass]
    public class ErrorsContainerFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenCreatingAnInstanceWithANullAction_ThenAnExceptionIsThrown()
        {
            new ErrorsContainer<object>(null);
        }

        [TestMethod]
        public void WhenCreatingInstance_ThenHasNoErrors()
        {
            var validation = new ErrorsContainer<string>(pn => { });

            Assert.IsFalse(validation.HasErrors);
            Assert.IsFalse(validation.GetErrors("property1").Any());
        }

        [TestMethod]
        public void WhenSettingErrorsForPropertyWithNoErrors_ThenNotifiesChangesAndHasErrors()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message"});

            Assert.IsTrue(validation.HasErrors);
            Assert.IsTrue(validation.GetErrors("property1").Contains("message"));
            CollectionAssert.AreEqual(new[] { "property1" }, validatedProperties);
        }

        [TestMethod]
        public void WhenSettingNoErrorsForPropertyWithNoErrors_ThenDoesNotNotifyChangesAndHasNoErrors()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new string[0]);

            Assert.IsFalse(validation.HasErrors);
            Assert.IsFalse(validation.GetErrors("property1").Any());
            Assert.IsFalse(validatedProperties.Any());
        }

        [TestMethod]
        public void WhenSettingErrorsForPropertyWithErrors_ThenNotifiesChangesAndHasErrors()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message" });

            validatedProperties.Clear();

            validation.SetErrors("property1", new[] { "message" });

            Assert.IsTrue(validation.HasErrors);
            Assert.IsTrue(validation.GetErrors("property1").Contains("message"));
            CollectionAssert.AreEqual(new[] { "property1" }, validatedProperties);
        }

        [TestMethod]
        public void WhenSettingNoErrorsForPropertyWithErrors_ThenNotifiesChangesAndHasNoErrors()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message" });

            validatedProperties.Clear();

            validation.SetErrors("property1", new string[0]);

            Assert.IsFalse(validation.HasErrors);
            Assert.IsFalse(validation.GetErrors("property1").Any());
            CollectionAssert.AreEqual(new[] { "property1" }, validatedProperties);
        }


    }
}
