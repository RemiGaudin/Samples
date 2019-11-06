using System;
using System.Collections.Generic;
using System.Linq;

namespace DeferredExecution
{
    class Program
    {
        private static readonly Dictionary<string, Market> _markets = new Dictionary<string, Market>
        {
            { "CME", new Market { Name = "CME" } },
            { "EUREX", new Market { Name = "EUREX" } },
            { "ICE", new Market { Name = "ICE" } }
        };

        static void Main(string[] args)
        {
            try
            {
                var markets = GetMarkets();
                foreach (var mar in markets)
                {
                    // This will raise the exception "System.InvalidOperationException: Collection was modified..." at the second iteration!
                    // This is due to the fact that IEnumerable<T> is deferring the execution of LINQ queries (except for functions
                    // such as .ToList() or .ToArray() that will force the execution of the query).
                    // To prevent the exception, GetMarkets() should do a .ToList() before returning the IEnumerable<Market>.
                    // See also: https://stackoverflow.com/q/1168944/4924754
                    _markets.Remove(mar.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Number of markets remaining in the dictionary: " + _markets.Count);
            Console.ReadKey();
        }
        private static IEnumerable<Market> GetMarkets()
        {
            return _markets.Values.Select(x => x.Copy());
        }

        public class Market
        {
            public string Name { get; set; }

            public Market Copy()
            {
                return (Market)MemberwiseClone();
            }
        }
    }
}
