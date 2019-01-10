using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PLinqExamples
{
    class Program
    {
        static Random rand = new Random(0);
        static int minValue = 0;
        static int maxValue = 20;


        static int[] GenerateTestArray(int size)
        {
            int[] testArray = Enumerable
                .Repeat(0, size)
                .Select(i => rand.Next(minValue, maxValue))
                .ToArray();
            return testArray;
        }

        static void Benchmark(string message, int size, int iters, Action<int[]> linq, Action<int[]> plinq)
        {
            var sw = new Stopwatch();

            Console.WriteLine(message);
            Console.WriteLine($"Iterations: {iters}");
            for (int i = 0; i < iters; i++)
            {
                int[] array = GenerateTestArray(size);
                sw.Start();
                linq(array);
                sw.Stop();
            }
            var avgTimeLINQ = sw.Elapsed.Ticks / iters;
            Console.WriteLine($"\t LINQ: {new TimeSpan(avgTimeLINQ)}");
            sw.Reset();
            for (int i = 0; i < iters; i++)
            {
                int[] array = GenerateTestArray(size);
                sw.Start();
                plinq(array);
                sw.Stop();
            }
            var avgTimePLINQ = sw.Elapsed.Ticks / iters;
            Console.WriteLine($"\t PLINQ: {new TimeSpan(avgTimePLINQ)}");

            var accelaration = (double)avgTimeLINQ / avgTimePLINQ;
            Console.WriteLine($"Accelaration: {accelaration:0.####}\n");
        }

        static void Main(string[] args)
        {
            int size = 1000;
            int iters = 100;

            #region squares
            Benchmark(
                "Compute squares: ",
                size,
                iters,
                (arr) => arr.Select(x => x * x).ToArray(),
                (arr) => arr.AsParallel().Select(x => x * x).ToArray()
            );
            #endregion
            #region minimum
            Benchmark(
                "Find minimum: ",
                size,
                iters,
                (arr) => arr.Min(),
                (arr) => arr.AsParallel().Min()
            );
            #endregion
            #region sort
            Benchmark(
                "Sort array: ",
                size,
                iters,
                (arr) => arr.OrderBy(x => x).ToArray(),
                (arr) => arr.AsParallel().OrderBy(x => x).ToArray()
            );
            #endregion
            #region where
            Benchmark(
                "Where: ",
                size,
                iters,
                (arr) => arr.Where(x => x % 2 == 0).ToArray(),
                (arr) => arr.AsParallel().Where(x => x % 2 == 0).ToArray()
            );
            #endregion

            Console.ReadKey();
        }
    }
 }
