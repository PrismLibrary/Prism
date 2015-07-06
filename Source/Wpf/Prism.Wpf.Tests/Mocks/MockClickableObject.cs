

using System.Windows.Controls.Primitives;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockClickableObject : ButtonBase
    {
        public void RaiseClick()
        {
            OnClick();
        }
    }
}