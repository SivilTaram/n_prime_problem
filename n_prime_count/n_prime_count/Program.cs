using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (args != null && args.Length == 1)
            {
                string input = args[0];

                try
                {
                    int i = Convert.ToInt32(input);
                    long nthPrime = 0;
                    if (i >= 1 && i <= 50000)
                        nthPrime = Bit_Sieve(i);
                    else if (i > 50000)
                        nthPrime = Local_Bit_Sieve_Long(i, 256 * 256);
                    else
                        throw new Exception();
                    string output = nthPrime.ToString();
                    int init = output.Length % 3 == 0 ? 3 : output.Length % 3;
                    for(int k = 0; k < init; k++)
                    {
                        Console.Write(output[k]);
                    }
                    for(int k = init; k < output.Length; k += 3)
                    {
                        Console.Write(" ");
                        for (int j = 0; j < 3; j++)
                        {
                            Console.Write(output[k+j]);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sorry, I can't support the number.");
                }
            }
            else
            {
                Console.WriteLine("Please input the number nth.");
            }
        }

        //benchMark: 10,000,000 - 7,000 + ms
        public static void Normal_Sieve(int nth)
        {
            int startTime = System.Environment.TickCount;
            int limit = nth < 6 ? 25 : (int)(nth * (Math.Log(nth) + Math.Log(Math.Log(nth))));
            int count = 0;

            // generate all primes which is less than limit
            List<bool> isPrime = new List<bool>(limit+1);

            for (int i = 0; i < limit+1; i++)
                isPrime.Add(true);

            for (int i = 2; i * i <= limit; i++)
                if (isPrime[i])
                    for (int j = i * i; j <= limit; j += i)
                        isPrime[j] = false;

            for(int i=2;i<isPrime.Count();i++)
            {
                if (isPrime[i])
                    count++;
                if(count == nth)
                {
                    Console.WriteLine("The {0}th_prime is:{1} SpentTime:{2}ms", nth, i,
                        Environment.TickCount - startTime);
                    break;
                }
            }
        }

        public static int Bit_Sieve(int nth)
        {
            int startTime = Environment.TickCount;
            int limit = nth < 6 ? 25 : (int)(nth * (Math.Log(nth) + Math.Log(Math.Log(nth))));
            int count = 0;

            int total = limit + 1;
            int sqrt = (int)Math.Sqrt(limit) + 1;
            //use uint to replace bool.
            //[31 30 29 ... 0] every number maps to a bit in uint.
            List<uint> isPrime = new List<uint>((total >> 5) + 1);

            for (int i = 0; i < (total >> 5) + 1; i++)
                isPrime.Add(0x0);

            //Represent the prime from 0 ~ 31.

            for (int i = 2; i <= sqrt; i++)
                // is_prime[i>>5] bit i % 32 == 0 means it is a prime
                if ((isPrime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    for (int j = i * i; j <= total; j += i)
                        // is_prime[j>>5] bit j % 32 = 1;
                        isPrime[j >> 5] |= (uint)1 << (j & 31);
                }

            for (int i = 2; i < total; i++)
            {
                if ((isPrime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    count++;
                    if (count == nth)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static void Local_Bit_Sieve(int nth,int segSize)
        {
            Stopwatch Watch = new Stopwatch();
            Watch.Start();
            //要测试的代码  


            int limit = nth < 6 ? 25 :(int)(nth * (Math.Log(nth) + Math.Log(Math.Log(nth))));
            int sqrt = (int) Math.Sqrt(limit) + 1;
            
            //Get all primes which are less than \sqrt{limit}
            List<uint> isPrime = new List<uint>((sqrt >> 5) +1);
            for (int i = 0; i < (sqrt >> 5) + 1; i++)
                isPrime.Add(0x0);
            for (int i = 2; i * i <= sqrt; i++)
                // is_prime[i>>5] bit i % 32 == 0 means it is a prime
                if ((isPrime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    for (int j = i * i; j <= sqrt; j += i)
                        // is_prime[j>>5] bit j % 32 = 1;
                        isPrime[j >> 5] |= (uint)1 << (j & 31);
                }

            //smallPrimes store the primes
            List<int> smallPrimes = new List<int>();
            for (int i = 2; i < sqrt; i++)
            {
                if ((isPrime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    smallPrimes.Add(i);
                }
            }

            uint[] primeSeg = new uint[segSize];
            
            //allPrimes store all primes which are found.
            List<int> allPrimes = new List<int>();
            allPrimes.AddRange(smallPrimes);

            int high = segSize << 5;
            //chunk [2,limit] into different segments
            for (int low = sqrt; low <= limit; low += segSize << 5)
            {
                Array.Clear(primeSeg,0,segSize);
                //for each prime, use them to mark the [low,low + segSize]
                foreach (var sPrime in smallPrimes)
                {
                    int initValue = low % sPrime;
                    for (int i = (initValue == 0 ? initValue : sPrime - initValue); i < high; i += sPrime)
                    {
                        primeSeg[i >> 5] |= (uint) 1 << (i & 31);
                    }
                }

                for (int i = 0; i < high; i++)
                {
                    if ((primeSeg[i >> 5] & (1 << (i & 31))) == 0)
                        allPrimes.Add(i + low);
                }
                
                if (allPrimes.Count() > nth)
                {
                    break;
                }
            }
            Watch.Stop();
           
            long watchTime = Watch.ElapsedMilliseconds;//花费时间  
            Console.WriteLine("The {0}th prime is :{1}, Spent time :{2}", nth, allPrimes[nth - 1], watchTime);
        }

        public static long Local_Bit_Sieve_Long(int nth, int segSize)
        {
            Stopwatch Watch = new Stopwatch();
            Watch.Start();

            long nthPrime = 0;

            long limit = nth < 6 ? 25 : (long)(nth * (Math.Log(nth) + Math.Log(Math.Log(nth))));
            int sqrt = (int)Math.Sqrt(limit) + 1;

            segSize = Math.Max(sqrt, segSize);

            List<uint> isPrime = new List<uint>((sqrt >> 5) + 1);
            for (int i = 0; i < (sqrt >> 5) + 1; i++)
                isPrime.Add(0x0);
            for (int i = 2; i * i <= sqrt; i++)
                // is_prime[i>>5] bit i % 32 == 0 means it is a prime
                if ((isPrime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    for (int j = i * i; j <= sqrt; j += i)
                        // is_prime[j>>5] bit j % 32 = 1;
                        isPrime[j >> 5] |= (uint)1 << (j & 31);
                }

            //smallPrimes store the primes
            List<int> smallPrimes = new List<int>();
            for (int i = 2; i < sqrt; i++)
            {
                if ((isPrime[i >> 5] & (1 << (i & 31))) == 0)
                {
                    smallPrimes.Add(i);
                }
            }

            //allPrimes store the local segment primes
            List<long> segPrimes = new List<long>();
            uint[] primeSeg = new uint[segSize];

            long high = segSize << 5;
            long count = 0;
            segPrimes.AddRange(smallPrimes.Select(o=> (long)o));
            //chunk [2,limit] into different segments
            for (long low = sqrt; low <= limit; low += high)
            {
                Array.Clear(primeSeg, 0, segSize);
                //for each prime, use them to mark the [low,low + segSize]
                foreach (var sPrime in smallPrimes)
                {
                    int initValue =(int)(low % sPrime);
                    for (int i = (initValue == 0 ? initValue : sPrime - initValue); i < high; i += sPrime)
                    {
                        primeSeg[i >> 5] |= (uint)1 << (i & 31);
                    }
                }

                for (int i = 0; i < high; i++)
                {
                    if ((primeSeg[i >> 5] & (1 << (i & 31))) == 0)
                        segPrimes.Add(i + low);
                }

                count += segPrimes.Count();

                if (count > nth)
                {
                    nthPrime = segPrimes[(int)(segPrimes.Count() - 1 - count + nth)];
                    break;
                }
                segPrimes.Clear();
            }
            return nthPrime;
        }
    }
}
