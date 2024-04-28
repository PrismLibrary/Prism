using Avalonia.Controls;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockClickableObject : Button // : ButtonBase
    {
        public void RaiseClick()
        {
            OnClick();
        }
    }
}
