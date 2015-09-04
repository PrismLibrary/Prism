using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Prism.Windows.Tests.Mocks;
using Prism.Windows.Validation;

namespace Prism.Windows.Tests
{
    [TestClass]
    public class BindableValidatorFixture
    {
        [TestMethod]
        public void Validation_Of_Field_When_Valid_Should_Succeeed()
        {
            var model = new MockModelWithValidation() { Title = "A valid Title" };
            var target = new BindableValidator(model);

            bool isValid = target.ValidateProperty("Title");

            Assert.IsTrue(isValid);
            Assert.IsTrue(target.GetAllErrors().Values.Count == 0);
        }

        [TestMethod]
        public void Validation_Of_Field_When_Invalid_Should_Fail()
        {
            var model = new MockModelWithValidation() { Title = string.Empty };
            var target = new BindableValidator(model);

            bool isValid = target.ValidateProperty("Title");

            Assert.IsFalse(isValid);
            Assert.IsFalse(target.GetAllErrors().Values.Count == 0);
        }

        [TestMethod]
        public void Validation_Of_Fields_When_Valid_Should_Succeeed()
        {
            var model = new MockModelWithValidation()
            {
                Title = "A valid title",
                Description = "A valid description"
            };
            var target = new BindableValidator(model);

            bool isValid = target.ValidateProperties();

            Assert.IsTrue(isValid);
            Assert.IsTrue(target.GetAllErrors().Values.Count == 0);
        }

        [TestMethod]
        public void Validation_Of_Fields_When_Invalid_Should_Fail()
        {
            // Test model with invalid title
            var modelWithInvalidTitle = new MockModelWithValidation()
            {
                Title = string.Empty,
                Description = "A valid description"
            };
            var targetWithInvalidTitle = new BindableValidator(modelWithInvalidTitle);
            bool resultWithInvalidTitle = targetWithInvalidTitle.ValidateProperties();

            Assert.IsFalse(resultWithInvalidTitle);
            Assert.IsFalse(targetWithInvalidTitle.GetAllErrors().Values.Count == 0);

            // Test model with invalid description
            var modelWithInvalidDescription = new MockModelWithValidation()
            {
                Title = "A valid Title",
                Description = string.Empty
            };
            var targetWithInvalidDescription = new BindableValidator(modelWithInvalidDescription);
            bool resultWithInvalidDescription = targetWithInvalidDescription.ValidateProperties();

            Assert.IsFalse(resultWithInvalidDescription);
            Assert.IsFalse(targetWithInvalidDescription.GetAllErrors().Values.Count == 0);

            // Test model with invalid title + description
            var modelInvalid = new MockModelWithValidation()
            {
                Title = "1234567894",
                Description = string.Empty
            };
            var targetInvalid = new BindableValidator(modelInvalid);
            bool resultInvalid = targetInvalid.ValidateProperties();

            Assert.IsFalse(resultInvalid);
            Assert.IsFalse(targetInvalid.GetAllErrors().Values.Count == 0);
        }

        [TestMethod]
        public void Validation_Of_A_Nonexistent_Property_Should_Throw()
        {
            var model = new MockModelWithValidation();
            var target = new BindableValidator(model, (mapId, key) => "ErrorMessage");
   
           var exception = Assert.ThrowsException<ArgumentException>(() =>
                                       {
                                           target.ValidateProperty("DoesNotExist");
                                       });
            const string expectedMessage = "ErrorMessage\r\nParameter name: DoesNotExist";

            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}
