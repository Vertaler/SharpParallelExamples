using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;

namespace MatrixMultiplicationsExamples
{
    class MatrixMultiplication
    {
        public static void MultiplyStep(float[,] A, float[,] B, float[,] res, int i, int j)
        {
            res[i, j] = 0;
            for(int k=0; k < A.GetLength(1); k++)
            {
                res[i, j] += A[i, k] * B[k, j];
            }
            

        }

        public static void MultiplyStepSIMD(float[,] A, float[,] B, float[,] res, int i, int j)
        {
            res[i, j] = 0;
            int bound = A.GetLength(1);

            for (int k = 0; k < bound; k += Vector<float>.Count)
            {
                var subRow = new float[Vector<float>.Count];
                var subColumn = new float[Vector<float>.Count];
                for(int c=0; c<Vector<float>.Count && k+c < bound; c++)
                {
                    subRow[c] = A[i, k + c];
                    subColumn[c] = B[k + c, j];
                }
                var subRowSimd = new Vector<float>(subRow);
                var subColumnSimd = new Vector<float>(subColumn);
                res[i, j] += Vector.Dot(subRowSimd,subColumnSimd);
            }
        }

        public static void SequentialMultiply(float[,] A, float[,] B, float[,] res)
        {
            for(int i=0; i< res.GetLength(0); i++)
            {
                for(int j=0; j < res.GetLength(1); j++)
                {
                    MultiplyStep(A, B, res, i, j);
                }
            }
        }

        public static void ThreadMultiply(float[,] A, float[,] B, float[,] res)
        {
            int threadNum = 4;
            Thread[] threads = new Thread[threadNum];
            int rowsPerThread = res.GetLength(0) / threadNum;
            for (int threadIndex = 0; threadIndex < threadNum; threadIndex++)
            {
                int tmp = threadIndex;
                threads[threadIndex] = new Thread(() =>
                    {
                        for (int i = 0; i < rowsPerThread; i++)
                        {
                            for (int j = 0; j < res.GetLength(1); j++)
                            {
                                MultiplyStep(A, B, res, i + tmp * rowsPerThread, j);
                            }
                        }
                    }
                );
                threads[threadIndex].Start();
            }

            for (int threadIndex = 0; threadIndex < threadNum; threadIndex++)
            {
                threads[threadIndex].Join();
            }
        }

        public static void ThreadPoolMultiply(float[,] A, float[,] B, float[,] res)
        {
            using (var finished = new CountdownEvent(1)){
                for (int i = 0; i < res.GetLength(0); i++){
                    
                    var tmpI = i;
                    finished.AddCount();
                    ThreadPool.QueueUserWorkItem((state) =>
                        {
                            try{
                                for (int j = 0; j < res.GetLength(1); j++){
                                    MultiplyStep(A, B, res, tmpI, j);
                                }
                            }
                            finally{
                                finished.Signal();
                            }
                        }
                    ); 
                }
                finished.Signal();
                finished.Wait();
            }
        }

        public static void TaskMultiply(float[,] A, float[,] B, float[,] res)
        {
            int rows = res.GetLength(0);
            int columns = res.GetLength(1);
            var tasks = new Task[rows];
            for (int i = 0; i < rows; i++)
            {
                var tmpI = i;
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < columns; j++)
                    {
                        MultiplyStep(A, B, res, tmpI, j);
                    }
                });
            }
            Task.WaitAll(tasks);
        }

        public static void ParallelInvokeMultiply(float[,] A, float[,] B, float[,] res)
        {
            int rows = res.GetLength(0);
            int columns = res.GetLength(1);
            var actions = new Action[rows];
            for (int i = 0; i < rows; i++)
            {
                var tmpI = i;
                actions[i] = () =>
                {
                    for (int j = 0; j < columns; j++)
                    {
                        MultiplyStep(A, B, res, tmpI, j);
                    }
                };
            }
            Parallel.Invoke(actions);
        }

        public static void VectorizedMultiply(float[,] A, float[,] B, float[,] res)
        {
            for (int i = 0; i < res.GetLength(0); i++)
            {
                for (int j = 0; j < res.GetLength(1); j++)
                {
                    MultiplyStepSIMD(A, B, res, i, j);
                }
            }
        }

        public static void ParallelForMultiply(float[,] A, float[,] B, float[,] res)
        {
            int rows = res.GetLength(0);
            int columns = res.GetLength(1);
            Parallel.For(0, rows, (i) =>
                {
                    for (int j = 0; j < columns; j++)
                    {
                        MultiplyStep(A, B, res, i, j);
                    }
                }
            );          
        }
    }
}
