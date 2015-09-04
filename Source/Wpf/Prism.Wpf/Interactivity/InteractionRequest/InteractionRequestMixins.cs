using System;
using System.Threading.Tasks;

namespace Prism.Interactivity.InteractionRequest
{
    public static class InteractionRequestMixins
    {
        /// <summary>
        /// Fires the Raised event asynchronously. Please note that this request may never return
        /// if the InteractionRequest is unhandled.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        /// <returns>The context after the request has been handled by the UI.</returns>
        public static async Task<T> RaiseAsync<T>(this InteractionRequest<T> request, T context) where T : INotification
        {
            return await AwaitCallbackResult<T>(callback => request.Raise(context, callback));
        }

        private static async Task<TResult> AwaitCallbackResult<TResult>(Action<Action<TResult>> f)
        {
            var tcs = new TaskCompletionSource<TResult>();
            f(n => tcs.SetResult(n));
            return await tcs.Task;
        }
    }
}
