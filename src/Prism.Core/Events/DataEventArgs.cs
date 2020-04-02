


using System;

namespace Prism.Events
{
    /// <summary>
    /// Generic arguments class to pass to event handlers that need to receive data.
    /// </summary>
    /// <typeparam name="TData">The type of data to pass.</typeparam>
    public class DataEventArgs<TData> : EventArgs
    {
        private readonly TData _value;

        /// <summary>
        /// Initializes the DataEventArgs class.
        /// </summary>
        /// <param name="value">Information related to the event.</param>
        public DataEventArgs(TData value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the information related to the event.
        /// </summary>
        /// <value>Information related to the event.</value>
        public TData Value
        {
            get { return _value; }
        }
    }
}