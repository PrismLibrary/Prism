using Prism.Logging;
using System.Diagnostics;

namespace Prism
{
    public class DebugLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            var priority_text = priority.ToString();
            var category_text = category.ToString();
            if (priority == Priority.None && category == Category.Info)
            {
                priority_text = string.Empty;
                category_text = string.Empty;
            }

            Debug.WriteLine($"{priority_text} {category_text} {message}");
        }
    }
}
