using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace 异步流_AsyncStreameam_
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            const int count = 100;
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}-{DateTime.Now}-Index with yield starting.");
            IAsyncEnumerable<int> sequence = ProduceMessagesYield(count);
            var consumingTask=Task.Run(()=>ConsumeSequenceAsync(sequence));

            await consumingTask;
            Console.WriteLine("################################################");
            Console.WriteLine(Environment.NewLine);
        }

        static async Task ConsumeSequenceAsync(IAsyncEnumerable<int> sequence)
        {
            await foreach (var i in sequence)
            {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}-{DateTime.Now}-Yield index: {i}");
                await Task.Delay(TimeSpan.FromSeconds((new Random().Next(1,10))*0.1));
            }
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}-{DateTime.Now}-Index with yield completed.");
        }

        static async IAsyncEnumerable<int> ProduceMessagesYield(int count = 100)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}-{DateTime.Now}-ProduceMessagesYield called!");

            for (var i = 1; i <= count; i++)
            {
                // 模拟延迟!
                Task.Delay(TimeSpan.FromSeconds(0.1)).Wait();
                yield return i;
            }
        }
    }
}
