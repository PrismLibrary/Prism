using System;

namespace Prism.Ioc.Mocks.Views
{
    public class BadView : ViewBase
    {
        public BadView()
        {
            throw new XamlParseException("You write bad XAML");
        }
    }

    public class XamlParseException : Exception
    {
        public XamlParseException(string message)
            : base(message)
        {
        }
    }
}
