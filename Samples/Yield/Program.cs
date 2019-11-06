﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Yield
{
    // Code sample from https://filteredcode.wordpress.com/2015/01/07/tolist-ing-everything-and-who-is-yield-part-2-of-2/
    static class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
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
