using Avalonia;
using Xunit;
using Prism.Extensions;

namespace Prism.Avalonia.Tests.Interactivity
{
    public class ObservableBehaviorFixture
    {
        [Fact]
        public void SubscribeWorksOnIObservableWithLambda()
        {
            var targetUIElement = new TestAvaloniaObject();
            targetUIElement.Test = "123";

            Assert.Equal("123", targetUIElement.OutTest);
        }
    }

    class TestAvaloniaObject : AvaloniaObject
    {
        /// <summary>
        /// The property to subscribe on.
        /// </summary>
        public static readonly StyledProperty<string> TestProperty =
            AvaloniaProperty.Register<TestAvaloniaObject, string>("Test");

        /// <summary>
        /// Gets or sets a value to the TestProperty.
        /// </summary>
        public string Test
        {
            get { return (string)GetValue(TestProperty); }
            set { SetValue(TestProperty, value); }
        }

        /// <summary>
        /// The out test property to check after TestProperty change
        /// </summary>
        public string OutTest { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestAvaloniaObject"/>.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TestAvaloniaObject()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            TestProperty.Changed.Subscribe(args => OnPropertyChanged(args));
        }

        private void OnPropertyChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {
            OutTest = args.NewValue.Value;
        }
    }
}
