using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Prism.Windows.Tests.Mocks
{
    public class MockModelWithValidation : INotifyPropertyChanged
    {
        [Required]
        [RegularExpression("^[A-Z][ a-zA-Z]+$")]
        [CustomValidation(typeof(MockModelWithValidation), "ValidateTitle")]
        public string Title { get; set; }

        [Required]
        [RegularExpression("^[A-Z][ a-zA-Z]+$")]
        [CustomValidation(typeof(MockModelWithValidation), "ValidateDescription")]
        public string Description { get; set; }

        public static ValidationResult ValidateTitle(object value, ValidationContext validationContext)
        {
            // string length validation
            if (string.IsNullOrEmpty((string)value) || ((string)value).Length < 5)
            {
                return Task.FromResult(new ValidationResult("Title must have at least 5 characters")).Result;
            }

            return Task.FromResult(ValidationResult.Success).Result;
        }

        public static ValidationResult ValidateDescription(object value, ValidationContext validationContext)
        {
            // string length field validation
            if (string.IsNullOrEmpty((string)value) || ((string)value).Length < 5)
            {
                return Task.FromResult(new ValidationResult("Description must have at least 5 characters")).Result;
            }

            return Task.FromResult(ValidationResult.Success).Result;
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { }
            remove { }
        }
    }
}
