using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XliffTasks
{
    public class ExponentialRetry
    {
        public static IEnumerable<TimeSpan> Intervals
        {
            get
            {
                var seconds = 5;
                while (true)
                {
                    yield return TimeSpan.FromSeconds(seconds);
                    seconds *= 2;
                }
            }
        }

        public static async Task ExecuteWithRetry<T>(Func<T> action,
            Func<T, bool> isSuccess,
            int maxRetryCount,
            Func<IEnumerable<Task>> timer,
            string taskDescription = "")
        {
            var count = 0;
            foreach (var t in timer())
            {
                await t;
                var result = action();
                if (isSuccess(result))
                    return;
                count++;
                if (count == maxRetryCount)
                    throw new RetryFailedException(
                        $"Retry failed for {taskDescription} after {count} times with result: {result}");
            }
            throw new Exception("Timer should not be exhausted");
        }

        public static IEnumerable<Task> Timer(IEnumerable<TimeSpan> interval)
        {
            return interval.Select(Task.Delay);
        }
    }
}
