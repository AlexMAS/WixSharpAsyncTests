using System;
using System.Threading;
using System.Threading.Tasks;

namespace WixSharpAsyncTests
{
    public static class WixSharpAsyncHelper
    {
        public static Task<T> ExecuteInNewContext<T>(Func<T> action)
        {
            var taskResult = new TaskCompletionSource<T>();

            var asyncFlow = ExecutionContext.SuppressFlow();

            try
            {
                Task.Run(() =>
                         {
                             try
                             {
                                 var result = action();

                                 taskResult.SetResult(result);
                             }
                             catch (Exception exception)
                             {
                                 taskResult.SetException(exception);
                             }
                         })
                    .Wait();
            }
            finally
            {
                asyncFlow.Undo();
            }

            return taskResult.Task;
        }
    }
}