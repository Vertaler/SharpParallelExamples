using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
