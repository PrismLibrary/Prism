namespace Prism.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILoggerFacade"/> that does nothing. This
    /// implementation is useful when the application does not need logging
    /// but there are infrastructure pieces that assume there is a logger.
    /// </summary>
    public class EmptyLogger : ILoggerFacade
    {
        /// <summary>
        /// This method does nothing.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {

        }
    }
}