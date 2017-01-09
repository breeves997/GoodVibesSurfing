using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CloudUtilities.FaultTolerance
{
    public class BasicRetryPattern : IRetryStrategy
    {
        private int retryCount = 3;
        private int timeToWait = 1000;

        public async Task<T> DoTaskWithRetry<T>(Func<T> taskToExecute) where T : Task
        {
            int currentRetry = 0;

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    //Run the job we were passed. However, if the job was itself a delegate to execute a task,
                    //we may wind up returning before task completion, so we'll have to wait it
                    T result = await Task.Run(taskToExecute);

                    result.Wait();

                    // Return or break.
                    return result;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Operation Exception");

                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    if (currentRetry > this.retryCount || !IsTransient(ex))
                    {
                        // If this is not a transient error 
                        // or we should not retry re-throw the exception. 
                        throw;
                    }
                }

                // Wait to retry the operation.
                timeToWait *= 2;
                await Task.Delay(timeToWait);
            }

            //we shouldn't ever hit this but the compiler is complaining
            throw new TimeoutException();
        }
        public async Task<T> DoOperationWithRetry<T>(Func<T> taskToExecute) 
        {
            int currentRetry = 0;

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    //Run the task we were passed
                    T result = await Task.Run(taskToExecute);

                    // Return or break.
                    return result;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Operation Exception");

                    currentRetry++;

                    // Check if the exception thrown was a transient exception
                    // based on the logic in the error detection strategy.
                    if (currentRetry > this.retryCount || !IsTransient(ex))
                    {
                        // If this is not a transient error 
                        // or we should not retry re-throw the exception. 
                        throw;
                    }
                }

                // Wait to retry the operation.
                timeToWait *= 2;
                await Task.Delay(timeToWait);
            }

            //we shouldn't ever hit this but the compiler is complaining
            throw new TimeoutException();
        }
        private bool IsTransient(Exception ex)
        {
            // Determine if the exception is transient.
            // In some cases this may be as simple as checking the exception type, in other 
            // cases it may be necessary to inspect other properties of the exception.
            //if (ex is OperationTransientException)
            //    return true;

            //if we've got an aggregate exception from being passed around lets check what the root cause was
            if (ex is AggregateException)
            {
                ex = ex.GetBaseException();
            }
            var webException = ex as WebException;
            if (webException != null)
            {
                // If the web exception contains one of the following status values 
                // it may be transient.
                return new[] {WebExceptionStatus.ConnectionClosed,
                          WebExceptionStatus.Timeout,
                          WebExceptionStatus.RequestCanceled }.
                        Contains(webException.Status);
            }
            if (ex is TimeoutException)
            {
                return true;
            }

            // Additional exception checking logic goes here.
            return false;
        }

        //// Async method that wraps a call to a remote service (details not shown).
        //private async Task DoSomethingThatMighNotWork()
        //{
        //    var rando = new Random();
        //    await Task.Run(() => MaybeGetNumber());
        //}
        //private int MaybeGetNumber()
        //{
        //    var rando = new Random();
        //    var rdn = rando.Next(1, 3);
        //    if (rdn == 2)
        //    {
        //        return rdn;
        //    }
        //    else
        //    {
        //        throw new WebException("Too bad sucka", WebExceptionStatus.ConnectionClosed);
        //    }
        //}

    }
}
