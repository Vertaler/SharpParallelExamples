using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;

namespace SimpleSimdExample
{
    class Program
    {
        static void Main(string[] args)
        {
            int SIZE = 128000000;
            var sw = new Stopwatch();

            var a = new float[SIZE];
            var b = new float[SIZE];
            var res = new float[SIZE];

            sw.Start();
            for (int i = 0; i < SIZE; i++)
            {
                res[i] = a[i] + b[i];
            }
            sw.Stop();
            Console.WriteLine($"Seq sum time elapsed {sw.Elapsed}");

            sw.Reset();
            sw.Start();
            for (int i = 0; i < SIZE; i += Vector<float>.Count)
            {
                var a_s = new Vector<float>(a, i);
                var b_s = new Vector<float>(b, i);
                var r_s = Vector.Add(a_s, b_s);
                r_s.CopyTo(res, i);
            }
            sw.Stop();
            Console.WriteLine($"Vectorized sum time elapsed {sw.Elapsed}");


            Console.ReadKey();
        }
    }
}
