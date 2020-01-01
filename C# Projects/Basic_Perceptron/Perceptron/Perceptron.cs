using System;
using System.Collections.Generic;
using System.Text;
using PNIS.Matrix;

namespace Perceptron
{
    class Perceptron  //perceptron class
    {
        private Matrix weights;
        private float loss;

        public Perceptron(int input_dim)
        {
            var f = new Func<float>(GetRandomFloat);
            weights = new Matrix(input_dim, 1, f);
            loss = 0;
        }

        public void Train(Matrix x, Matrix y, int epochs = 50) //updates weight matrix
        {
            var y_hat = Forward(x);
            Backwards(y, y_hat);
            Console.WriteLine(y_hat.ToString());
        }

        public double Evaluate(Matrix x, Matrix y) //returns a test score for the model
        {
            return default;
        }

        private Matrix Forward(Matrix x) //returns a float array of y-hat values (predicted values for each instance)
        {
            Matrix y_hat = x * weights; 
            for (int i = 0; i < y_hat.GetLength(0); i++)
            {
                y_hat[i, 0] = TanH(y_hat[i,0]);
            }
            return y_hat;
        }

        private float TanH(float x)
        {
            x = Convert.ToSingle(Math.Tanh(x));
            return x;
        }

        private void Backwards(Matrix y, Matrix y_hat)
        {
            loss = CalculateError(y, y_hat);
        }

        public float CalculateError(Matrix y, Matrix y_hat)
        {
            return default;
        }

        static public float GetRandomFloat()
        {
            var r = new Random();
            return Convert.ToSingle(r.NextDouble() * r.Next(-1,2));
        }
    }
}
