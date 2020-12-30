using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AdaptivePredicate
{
    public static class Program
    {
        private static readonly Random Rnd = new Random(1);

        private static readonly FrequencyLimitPredicate Predicate = new FrequencyLimitPredicate(5);

        private static int TotalProcessed = 0;

        public static void Main(string[] args)
        {
            Console.Write("Init...");
            ThreadPool.SetMinThreads(1000, 1000);
            Console.WriteLine("OK");

            var stopwatch = Stopwatch.StartNew();

            Parallel.For(0, 1000, x => ProcessRequest(x).Wait());
            
            stopwatch.Stop();

            Console.WriteLine("DONE");
            Console.WriteLine($"Total seconds: {stopwatch.Elapsed.TotalSeconds}");
            Console.WriteLine($"Total processed: {TotalProcessed}");
            Console.WriteLine($"Avarage processed per second: {TotalProcessed / stopwatch.Elapsed.TotalSeconds}");
        }

        private static async Task ProcessRequest(int requestId)
        {
            await Task.Delay(Rnd.Next(50, 30_000));

            var time = DateTime.Now;
            var canProcess = Predicate.Evaluate();

            Console.WriteLine($"{time:HH:mm:ss.fff} Request#{requestId:0000} Thread#{Thread.CurrentThread.ManagedThreadId:0000} - {(canProcess ? "PROCESSED" : "BLOCKED")}");

            if (canProcess)
            {
                Interlocked.Increment(ref TotalProcessed);
            }
        }
    }
}
