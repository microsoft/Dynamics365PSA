using System.Threading.Tasks;

namespace Common.Utilities.Extensions
{
    /// <summary>
    /// Class containing extension methods for the System.Threading.Tasks.Task class.
    /// </summary>
    public static class TaskExtension
    {
        /// <summary>
        /// Supresses Warning CS4014
        /// Because this call is not awaited, execution of the current method continues before the call is completed.
        /// Consider applying the 'await' operator to the result of the call.
        /// </summary>
        /// <param name="task">Task that is not "awaited".</param>
        public static void DoNotAwait(this Task task)
        {
        }
    }
}
