using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Yield
{
    static class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var evenNums = GetEvenNumbersDeferred();
            foreach (var number in evenNums)
            {
                if (number > 10)
                    break;

                Console.WriteLine(number);
            }
            sw.Stop();
            Console.WriteLine("Build List Method: " + sw.ElapsedMilliseconds + " ms");
            Console.ReadKey();
        }

        public static IEnumerable<int> GetEvenNumbers()
        {
            var evens = new List<int>();
            for (int i = 0; i <= 500000000; i++)
            {
                if (i % 2 == 0)
                {
                    evens.Add(i);
                }
            }
            return evens;
        }
        public static IEnumerable<int> GetEvenNumbersDeferred()
        {
            for (int i = 0; i <= 500000000; i++)
            {
                if (i % 2 == 0)
                {
                    yield return i;
                }
            }
        }
    }
}
