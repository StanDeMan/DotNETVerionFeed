using Polly;
using Polly.Retry;

namespace VersionsFeedService.VersionParser.Transient
{
    public class Retry<T> where T : Exception
    {
        public int RetryCount { get; set; }

        public TimeSpan SleepDuration { get; set; } = TimeSpan.FromMilliseconds(200);

        public AsyncRetryPolicy Policy { get; }

        public T? ExceptionToCatch { get; } = new Exception() as T;

        public Retry(int retryCount = 3)
        {
            RetryCount = retryCount;

            Policy = Polly.Policy
                .Handle<T>()
                .WaitAndRetryAsync(RetryCount, sleepDurationProvider: _ => SleepDuration);
        }
    }
}
