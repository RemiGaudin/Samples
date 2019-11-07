using System;
using System.Collections.Generic;
using System.Linq;

namespace DeferredExecution
{
    class Program
    {
        private static readonly Dictionary<string, Market> _markets = new Dictionary<string, Market>
        {
            { "CBOT", new Market { Name = "CBOT" } },
            { "EUREX", new Market { Name = "EUREX" } },
            { "CME", new Market { Name = "CME" } },
            { "ICE", new Market { Name = "ICE" } },
            { "COMEX", new Market { Name = "COMEX" } }
        };

        static void Main(string[] args)
        {
            try
            {
                var markets = GetMarkets();
                foreach (var market in markets)
                {
                    // This will raise the exception "System.InvalidOperationException: Collection was modified..." at the second iteration!
                    // This is due to the fact that IEnumerable<T> is deferring the execution of LINQ queries (except for functions
                    // such as .ToList() or .ToArray() that will force the execution of the query).
                    // To prevent the exception, GetMarkets() should do a .ToList() before returning the IEnumerable<Market> (so the query execution
                    // is not deferred anymore and an actual copy of the list is returned).
                    // See also: https://stackoverflow.com/q/1168944/4924754
                    var marketCopy = new Market { Name = market.Name + "_copy" };
                    _markets.Add(marketCopy.Name, marketCopy);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Number of markets in the dictionary: " + _markets.Count);
            Console.ReadKey();
        }
        private static IEnumerable<Market> GetMarkets()
        {
            // If you add .ToList() at the end of the return below, you won't have the exception anymore.
            return _markets.Values.Where(x => x.Name.StartsWith("C")).Select(x => x.Copy());
        }

        private class Market
        {
            public string Name { get; set; }

            public Market Copy()
            {
                return (Market)MemberwiseClone();
            }
        }
    }
}
