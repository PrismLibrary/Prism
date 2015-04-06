// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mvvm;
using Prism.Tests.Mocks.ViewModels;

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

        [TestMethod]
        public void WhenClearingErrorsForPropertyWithErrors_ThenPropertyHasNoErrors()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message" });
            validation.SetErrors("property2", new[] { "message2" });

            validation.ClearErrors("property1");

            Assert.IsTrue(validation.HasErrors);
            Assert.IsFalse(validation.GetErrors("property1").Any());
            Assert.IsTrue(validation.GetErrors("property2").Any());
        }

        [TestMethod]
        public void WhenClearingErrorsWithNullPropertyname_ThenHasErrors()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message" });

            validation.ClearErrors(null);

            Assert.IsTrue(validation.HasErrors);
            Assert.IsTrue(validation.GetErrors("property1").Any());
        }

        [TestMethod]
        public void WhenClearingErrorsForPropertyWithErrorsGeneric_ThenPropertyHasNoErrors()
        {
            var viewModel = new Prism.Tests.Mocks.ViewModels.MockValidatingViewModel();
            viewModel.MockProperty = -5;
            Assert.IsTrue(viewModel.HasErrors);

            viewModel.ClearMockPropertyErrors();

            Assert.IsFalse(viewModel.HasErrors);
        }

        [TestMethod]
        public void WhenGettingErrorsWithPropertyName_ThenErrorsReturned()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message" });

            var errors = validation.GetErrors("property1");

            Assert.IsTrue(errors.Any());
        }

        [TestMethod]
        public void WhenGettingErrorsWithNullPropertyName_ThenNoErrorsReturned()
        {
            List<string> validatedProperties = new List<string>();

            var validation = new ErrorsContainer<string>(pn => validatedProperties.Add(pn));

            validation.SetErrors("property1", new[] { "message" });

            var errors = validation.GetErrors(null);

            Assert.IsTrue(validation.HasErrors);
            Assert.IsTrue(errors.Count() == 0);
        }

        [TestMethod]
        public void WhenSettingsErrorsForPropertyWithNullCollection_ThenPropertyHasNoErrors()
        {
            var viewModel = new Prism.Tests.Mocks.ViewModels.MockValidatingViewModel();
            viewModel.MockProperty = 10;
            Assert.IsFalse(viewModel.HasErrors);

            viewModel.SetMockPropertyErrorsWithNullCollection();

            Assert.IsFalse(viewModel.HasErrors);
        }
    }
}
