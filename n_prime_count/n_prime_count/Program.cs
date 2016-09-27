using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n_prime_count
{
    class Program
    {
        //const int L1D_CACHE_SIZE = 262144;
        static void Main(string[] args)
        {
            //Normal_Sieve(10000000);
            Bit_Sieve(10000000);
        }

        //benchMark: 10,000,000 - 7,000 + ms
        public static void Normal_Sieve(int nth)
        {
            int startTime = System.Environment.TickCount;
            int limit = (int)(nth * (Math.Log(nth) + Math.Log(Math.Log(nth))));
            int count = 0;

            // generate all primes which is less than limit
            List<bool> is_prime = new List<bool>(limit+1);

            for (int i = 0; i < limit+1; i++)
                is_prime.Add(true);

            for (int i = 2; i * i <= limit; i++)
                if (is_prime[i])
                    for (int j = i * i; j <= limit; j += i)
                        is_prime[j] = false;

            for(int i=2;i<is_prime.Count();i++)
            {
                if (is_prime[i])
                    count++;
                if(count == nth)
                {
                    Console.WriteLine("The nth_prime is:{0} SpentTime:{1}ms",i,Environment.TickCount- startTime);
                    break;
                }
            }
            Console.ReadKey();
        }

        public static void Bit_Sieve(int nth)
        {
            int startTime = Environment.TickCount;
            int limit = (int)(nth * (Math.Log(nth) + Math.Log(Math.Log(nth))));
            int count = 0;

            int total = limit + 1;
            int sqrt = (int)Math.Sqrt(limit) + 1;
            //use uint to replace bool.
            //[31 30 29 ... 0] every number maps to a bit in uint.
            List<uint> is_prime = new List<uint>((total >> 5) + 1);

            for (int i = 0; i < (total >> 5) + 1; i++)
                is_prime.Add(0x0);

            //Represent the prime from 0 ~ 31.

            for (int i = 2; i <= sqrt; i++)
                // is_prime[i>>5] bit i % 32 == 0 means it is a prime
                if ((is_prime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    for (int j = i * i; j <= total; j += i)
                        // is_prime[j>>5] bit j % 32 = 1;
                        is_prime[j >> 5] |= (uint)1 << (j & 31);
                }

            for (int i = 2; i < total; i++)
            {
                if ((is_prime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    count++;
                    if (count == nth)
                    {
                        Console.WriteLine("The nth_prime is:{0} SpentTime:{1}ms", i, Environment.TickCount - startTime);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
