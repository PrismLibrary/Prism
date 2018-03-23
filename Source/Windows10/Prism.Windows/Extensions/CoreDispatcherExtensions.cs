using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Prism.Windows.Extensions
{
    /// <summary>
    /// Provides extension methods to easily run tasks on a CoreDispatcher instance.
    /// </summary>
    public static class CoreDispatcherExtensions
    {
        /// <summary>
        /// Runs the given task asynchronously on the given CoreDispatcher instance.
        /// </summary>
        /// <typeparam name="T">The tasks return type.</typeparam>
        /// <param name="dispatcher">The CoreDispatcher instance to run the task on.</param>
        /// <param name="func">The function delegate to be running within the task.</param>
        /// <param name="priority">The core dispatcher priority.</param>
        /// <returns>An awaitable task object.</returns>
        public static async Task<T> RunTaskAsync<T>(this CoreDispatcher dispatcher,
            Func<Task<T>> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            await dispatcher.RunAsync(priority, async () =>
            {
                try
                {
                    taskCompletionSource.SetResult(await func());
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Runs the given function delegate asynchronously on the given CoreDispatcher instance.
        /// </summary>
        /// <typeparam name="T">The delegates return type.</typeparam>
        /// <param name="dispatcher">The CoreDispatcher instance to run the delegate on.</param>
        /// <param name="func">The function delegate to be running on the dispatcher.</param>
        /// <param name="priority">The core dispatcher priority.</param>
        /// <returns>An awaitable task object.</returns>
        public static async Task<T> RunAsync<T>(this CoreDispatcher dispatcher,
            Func<T> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();
            await dispatcher.RunAsync(priority, () =>
            {
                try
                {
                    taskCompletionSource.SetResult(func());
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Runs the given task asynchronously on the given CoreDispatcher instance.
        /// </summary>
        /// <param name="dispatcher">The CoreDispatcher instance to run the task on.</param>
        /// <param name="func">The function delegate to be running within the task.</param>
        /// <param name="priority">The core dispatcher priority.</param>
        /// <returns>An awaitable task object.</returns>
        public static async Task RunTaskAsync(this CoreDispatcher dispatcher,
            Func<Task> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal) =>
                await RunTaskAsync(dispatcher, async () => { await func(); return false; }, priority);
    }
}