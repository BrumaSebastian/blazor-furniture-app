using Microsoft.Extensions.Logging;

namespace BlazorFurniture.Core.Shared.Utils.Extensions;

public static class TaskExtensions
{
    extension( Task task )
    {
        public Task LogOnFaulted( ILogger logger, string message = "" )
        {
            task.ContinueWith(t =>
            {
                logger.LogError(exception: t.Exception, message: message);
            }, TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }
    }
}
