using System;
using System.Windows;
using Avalonia;
using Avalonia.Controls;
using Prism.Interactivity.InteractionRequest;

namespace Prism.Avalonia.Tests.Mocks
{
    public class MockInteractionRequestAwareElement : StyledElement, IInteractionRequestAware
    {
        public INotification Notification { get; set; }

        public Action FinishInteraction { get; set; }
    }
}
