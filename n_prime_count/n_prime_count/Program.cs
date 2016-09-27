using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n_prime_count
{
    class Program
    {
        const int L1D_CACHE_SIZE = 262144;
        static void Main(string[] args)
        {
            Segmented_Sieve(10000000);
        }

        public static void Segmented_Sieve(Int64 limit)
        {
            int sqrt = (int)Math.Sqrt(limit);
            int segment_size = Math.Max(sqrt, L1D_CACHE_SIZE);

            // generate all small primes <= sqrt.
            List<bool> is_prime = new List<bool>(sqrt + 1);
            for(int i = 0; i < sqrt + 1; i++)
                is_prime.Add(true);

        
            for(int i= 2; i * i <= sqrt; i++)
                if (is_prime[i])
                    for (int j = i * i; j <= sqrt; j += i)
                        is_prime[j] = false;
        }
    }
}
