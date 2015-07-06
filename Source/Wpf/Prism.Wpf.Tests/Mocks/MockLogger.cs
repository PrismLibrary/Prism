

using Prism.Logging;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockLogger : ILoggerFacade
    {
        public string LastMessage;
        public Category LastMessageCategory;
        public void Log(string message, Category category, Priority priority)
        {
            LastMessage = message;
            LastMessageCategory = category;
        }
    }
}