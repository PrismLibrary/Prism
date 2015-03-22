using System;
using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace Prism.Wpf.Tests.Mocks
{
    public class MockInteractionRequestAwareElement : FrameworkElement, IInteractionRequestAware
    {
        public INotification Notification { get; set; }

        public Action FinishInteraction { get; set; }
    }
}
