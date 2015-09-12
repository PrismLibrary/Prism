using System;
using System.Threading.Tasks;

namespace Prism.Common
{
    internal static class CallbackHelper
    {
        public static async Task<TResult> AwaitCallbackResult<TResult>(Action<Action<TResult>> f)
        {
            var tcs = new TaskCompletionSource<TResult>();
            f(n => tcs.SetResult(n));
            return await tcs.Task;
        }
    }
}