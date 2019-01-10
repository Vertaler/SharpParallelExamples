using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Numerics;

namespace MatrixMultiplicationsExamples
{
    class Program
    {
        delegate void MatrixMultiply(float[,] A, float[,] B, float[,] res);
      
        static void BenchmarkMatrixMultiply(int n, int m, int k, int iters, IDictionary<string, MatrixMultiply> methods)
        // n - first matrix rows
        // m - first matrix columns and second matrix rows
        // k - second matrix columns
        {
            var sw = new Stopwatch();
            var random = new Random(42);

            foreach (var item in methods)
            {
                sw.Reset();
                for (int count = 0; count< iters; count++)
                {
                    #region initialize matrices

                    float[,] A = new float[n, m];
                    float[,] B = new float[m, k];
                    float[,] res = new float[n, k];

                    for (int i = 0; i < n; ++i)
                        for (int j = 0; j < m; ++j)
                            A[i, j] = (float)random.NextDouble();

                    for (int i = 0; i < m; ++i)
                        for (int j = 0; j < k; ++j)
                            B[i, j] = (float)random.NextDouble();
                    #endregion

                    sw.Start();
                    item.Value(A, B, res);
                    sw.Stop();
                }
                var avgTime = new TimeSpan(sw.Elapsed.Ticks / iters);
                Console.WriteLine($"{item.Key} method average time on {iters} iterations: {avgTime}");
            }

        }

        static void Main(string[] args)
        {

            int n = 500;
            int m = 500;
            int k = 500;
            int iters = 10;

            Console.WriteLine($"Vector<T> hardware accelerated: {Vector.IsHardwareAccelerated}");
            Console.WriteLine($"Vector<float>.Count: {Vector<float>.Count}");

            var methods =new Dictionary<string, MatrixMultiply>();
            methods["Sequential"] = MatrixMultiplication.SequentialMultiply;
            methods["Vectorized"] = MatrixMultiplication.VectorizedMultiply;
            methods["Thread manually"] = MatrixMultiplication.ThreadMultiply;
            methods["ThreadPool"] = MatrixMultiplication.ThreadPoolMultiply;
            methods["Tasks"] = MatrixMultiplication.TaskMultiply;
            methods["ParallelInvoke"] = MatrixMultiplication.ParallelInvokeMultiply;
            methods["ParallelFor"] = MatrixMultiplication.ParallelForMultiply;


            BenchmarkMatrixMultiply(n, m, k, iters, methods);
            Console.ReadKey();
        }
    }
}
