namespace System.Threading.Tasks
{
    /// <summary>
    /// Extension methods for the Task object.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits a task without blocking the main thread.
        /// </summary>
        /// <remarks>Primarily used to replace async void scenarios such as ctor's and ICommands.</remarks>
        /// <param name="task">The task to be awaited</param>
        public static void Await(this Task task)
        {
            task.Await(null, null, false);
        }

        /// <summary>
        /// Awaits a task without blocking the main thread.
        /// </summary>
        /// <remarks>Primarily used to replace async void scenarios such as ctor's and ICommands.</remarks>
        /// <param name="task">The task to be awaited</param>
        /// <param name="configureAwait">Configures an awaiter used to await this task</param>
        public static void Await(this Task task, bool configureAwait)
        {
            task.Await(null, null, configureAwait);
        }

        /// <summary>
        /// Awaits a task without blocking the main thread.
        /// </summary>
        /// <remarks>Primarily used to replace async void scenarios such as ctor's and ICommands.</remarks>
        /// <param name="task">The task to be awaited</param>
        /// <param name="completedCallback">The action to perform when the task is complete.</param>
        public static void Await(this Task task, Action completedCallback)
        {
            task.Await(completedCallback, null, false);
        }

        /// <summary>
        /// Awaits a task without blocking the main thread.
        /// </summary>
        /// <remarks>Primarily used to replace async void scenarios such as ctor's and ICommands.</remarks>
        /// <param name="task">The task to be awaited</param>
        /// <param name="completedCallback">The action to perform when the task is complete.</param>
        /// <param name="errorCallback">The action to perform when an error occurs executing the task.</param>
        public static void Await(this Task task, Action completedCallback, Action<Exception> errorCallback)
        {
            task.Await(completedCallback, errorCallback, false);
        }

        /// <summary>
        /// Awaits a task without blocking the main thread.
        /// </summary>
        /// <remarks>Primarily used to replace async void scenarios such as ctor's and ICommands.</remarks>
        /// <param name="task">The task to be awaited</param>
        /// <param name="errorCallback">The action to perform when an error occurs executing the task.</param>
        public static void Await(this Task task, Action<Exception> errorCallback)
        {
            task.Await(null, errorCallback, false);
        }

        /// <summary>
        /// Awaits a task without blocking the main thread.
        /// </summary>
        /// <remarks>Primarily used to replace async void scenarios such as ctor's and ICommands.</remarks>
        /// <param name="task">The task to be awaited</param>
        /// <param name="completedCallback">The action to perform when the task is complete.</param>
        /// <param name="errorCallback">The action to perform when an error occurs executing the task.</param>
        /// <param name="configureAwait">Configures an awaiter used to await this task</param>
        public async static void Await(this Task task, Action completedCallback, Action<Exception> errorCallback, bool configureAwait)
        {
            try
            {
                await task.ConfigureAwait(configureAwait);
                completedCallback?.Invoke();
            }
            catch (Exception ex)
            {
                errorCallback?.Invoke(ex);
            }
        }
    }
}
