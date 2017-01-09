
namespace CloudUtilities.FaultTolerance
{
    using System;
    using System.Threading.Tasks;
    public interface IRetryStrategy
    {
        Task<T> DoOperationWithRetry<T>(Func<T> taskToExecute);
        Task<T> DoTaskWithRetry<T>(Func<T> taskToExecute) where T : Task;
    }
}