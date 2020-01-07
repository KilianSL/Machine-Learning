using System;
using System.Collections.Generic;
using System.Text;
using PNIS.Matrix;

namespace Perceptron
{
    class Perceptron  //perceptron class
    {
        private Matrix weights;
        private float bias;
        private float loss;

        public Perceptron(int input_dim)
        {
            var f = new Func<float>(GetRandomFloat);
            weights = new Matrix(input_dim, 1, f);
            bias = 1.0f;
            loss = 0.0f;
        }

        public void Train(Matrix x, Matrix y, int epochs = 50, float learningRate=0.001f) //updates weight matrix
        {
            for (int i = 1; i < epochs + 1; i++)
            {

                var y_hat = Forward(x);
                Backwards(y, y_hat, learningRate);
            }
            //Console.WriteLine(y_hat.ToString());
        }

        public float Evaluate(Matrix x, Matrix y) //returns a test score for the model
        {
            var y_hat = Forward(x);
            return CalculateError(y, y_hat);
        }

        private Matrix Forward(Matrix x) //returns a float array of y-hat values (predicted values for each instance)
        {
            Matrix y_hat = x * weights; 
            for (int i = 0; i < y_hat.GetLength(0); i++)
            {
                y_hat[i, 0] = Activate(y_hat[i,0] + bias);
            }
            return y_hat;
        }

        private float Activate(float x)
        {
            double r = 1.0 / (1.0 + Math.Exp(x));
            return Convert.ToSingle(r);
        }

        private void Backwards(Matrix y, Matrix y_hat, float lr)
        {
            loss = CalculateError(y, y_hat);
            UpdateWeights(loss, lr);
        }

        public float CalculateError(Matrix y, Matrix y_hat)   //root mean square error
        {
            float error = 0;
            for (int i = 0; i < y.GetLength(0); i++)
            {
                error += Convert.ToSingle(Math.Pow((y_hat[i] - y[i]), 2));
            }
            error = error / y.GetLength(0);
            error = Convert.ToSingle(Math.Sqrt(error));
            Console.WriteLine("Error: " + error);
            return error;
        }

        static public float GetRandomFloat()
        {
            var r = new Random();
            return Convert.ToSingle(r.NextDouble() * r.Next(-1,2));
        }

        private void UpdateWeights(float loss, float lr)
        {
            bias = bias + (loss * lr);
            for (int i = 0; i < weights.GetLength(0); i++)
            {
                weights[i,0] = weights[i,0] + (loss * lr);
            }
        }
    }
}
