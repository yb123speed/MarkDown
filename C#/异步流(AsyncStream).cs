using System;
using System.Collections.Generic;
namespace Test
{
    public class Program
    {
        public void Main()
        {
            const int count = 5;
            ConsoleExt.WriteLine("Sum with yield starting.");
            foreach (var i in ProduceMessagesYield(count))
            {
                ConsoleExt.WriteLine($"Yield index: {i}");
            }
            ConsoleExt.WriteLine("Index with yield completed.");

            ConsoleExt.WriteLine("################################################");
            ConsoleExt.WriteLine(Environment.NewLine);
        }

        async IAsyncEnumerable<string> ProduceMessagesYield(int count = 100)
        {
            ConsoleExt.Writeline("ProduceMessagesYield called!");

            for (var i = 0; i <= count; i++)
            {
                yield return i;
            }
        }
    }
}