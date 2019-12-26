using System;

namespace TensorOps
{
    class Program
    {
        static void Main(string[] args)
        {
            float[,] f = { { 1, 4}, { 7, 1 } };
            Tensor a = new Tensor(f);
            a.Display();
            Console.WriteLine();
            a.Transpose();
            a.Display();
        }
    }
}
