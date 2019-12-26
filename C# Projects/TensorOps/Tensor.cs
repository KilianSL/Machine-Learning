using System;
using System.Collections.Generic;
using System.Text;

namespace TensorOps
{
    class Tensor //creates a standard float tensor
    {
        private int[] shape;  //stores MxN size of tensor
        private float[,] values;

        public Tensor(int m, int n)
        {
            shape = new int[] { m, n };
            values = new float[shape[0], shape[1]];
        }
        public Tensor(int m, int n, float fillValue)  //fills the tensor with a single value
        {
            shape = new int[] { m, n };
            values = new float[shape[0], shape[1]];
            for (int i = 0; i < shape[0]; i++)
            {
                for (int j = 0; j < shape[1]; j++)
                {
                    values[i, j] = fillValue;
                }
            }
        }

        public Tensor(float[,] input)  //creates a tensor based off the input 2d array
        {
            shape = new int[] { input.GetLength(0), input.GetLength(1)};
            values = new float[shape[0], shape[1]];
            for (int i = 0; i < shape[0]; i++)
            {
                for (int j = 0; j < shape[1]; j++)
                {
                    values[i, j] = input[i, j];

                }
            }
        }

        public float GetValue(int i, int j)
        {
            return values[i, j];
        }

        public void SetValue(int i, int j, float v)
        {
            try
            {
                values[i, j] = v;
            }
            catch (Exception)
            {
                Console.WriteLine("Index [{0}, {1}] was outside of the bounds of the tensor", i, j);
            }
        }
        public int[] GetShape()
        {
            return shape;
        }

        public void Add(Tensor t)
        {
            if (this.shape[0] == t.GetShape()[0] && this.shape[1] == t.GetShape()[1])
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    for (int j = 0; j < shape[1]; j++)
                    {
                        this.values[i, j] += t.GetValue(i, j);
                    }
                }
            }
            else
            {
                Console.WriteLine("Dimensions not right for addition");
            }
        }

        public void Add(float a)
        {
            for (int i = 0; i < shape[0]; i++)
            {
                for (int j = 0; j < shape[1]; j++)
                {
                    values[i, j] += a;
                }
            }
        }
        public void Display()
        {
            for (int i = 0; i < shape[0]; i++)
            {
                for (int j = 0; j < shape[1]; j++)
                {
                    Console.Write(values[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public void Transform(float a) //Transforms by the scalar a
        {
            for (int i = 0; i < shape[0]; i++)
            {
                for (int j = 0; j < shape[1]; j++)
                {
                    values[i, j] = values[i, j] * a;
                }
            }
        }

        public void Transform(Tensor t)  //Transforms by the tensor t, such that the final tensor is t * this_tensor
        {
            
            if (t.GetShape()[1] == this.shape[0])
            {
                float[,] results = new float[t.GetShape()[0], this.shape[1]];
                for (int i = 0; i < results.GetLength(0); i++)
                {
                    for (int j = 0; j < results.GetLength(1); j++)
                    {
                        float v = 0;
                        for (int k = 0; k < shape[0]; k++)
                        {
                            v += t.GetValue(i, k) * this.values[k, j];
                        }
                        results[i, j] = v;
                    }
                }
                this.values = results;
                this.shape = new int[] { results.GetLength(0), results.GetLength(1) };
            }
            else
            {
                Console.WriteLine("Wrong dimensions for transformation");
            }
        }

        public void Transpose() //transposes this tensor
        {
            float[,] t = new float[shape[1], shape[0]];
            for (int i = 0; i < shape[0]; i++)
            {
                for (int j = 0; j < shape[1]; j++)
                {
                    t[j, i] = values[i, j];
                }
            }
            values = t;
        }
        public float[,] ToArray()  //returns the values as an array
        {
            return values;
        }
    }

}
