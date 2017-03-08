namespace Prism.Logging
{
    /// <summary>
    /// Defines values for the priorities used by <see cref="ILoggerFacade"/>.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// No priority specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// High priority entry.
        /// </summary>
        High = 1,

        /// <summary>
        /// Medium priority entry.
        /// </summary>
        Medium,

        /// <summary>
        /// Low priority entry.
        /// </summary>
        Low
    }
}