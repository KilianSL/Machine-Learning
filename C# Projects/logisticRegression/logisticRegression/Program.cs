using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataScience;

namespace logisticRegression
{
    class Program
    {
        static void Main(string[] args)
        {
            // arrays are notated name[row, column]. i will denote columns and j will denote rows (i = x, j = y)
            string path = @"C:\Users\kilian\source\repos\logisticRegression\logisticRegression\banknote.csv";
            initData(path, 4);
            
        }

        static void initData(string path, int columns)
        {
            
            CSVReader csv = new CSVReader();
            double[] x = csv.ReadDouble(path, 0);
            double[,] xList = new double[x.Length, columns];
            double[] coef = new double[columns + 1];
            for (int i = 0; i < coef.Length; i++)
            {
                coef[i] = 0.5;
            }
            for (int i = 0; i < 4; i++)
            {
                x = csv.ReadDouble(path, i + 1);
                for (int j = 0; j < x.Length; j++)
                {
                    xList[j, i] = x[j];
                }
            }
            double[] yList = csv.ReadDouble(path, columns + 1);
            coef = FitModel(xList, yList, coef, 1e-5, 12240); //12240, 1e-5
            Console.WriteLine("z = {0} + {1}x1 + {2}x2 + {3}x3 + {4}x4", coef[0], coef[1], coef[2], coef[3], coef[4]);
            Console.ReadLine();
            outputTest(coef, yList, xList);
            Console.ReadLine();
        } //reads data from the dataset and initialises variables, passes on to FitModel()

        static double[] FitModel(double[,] xList, double[] yList, double[] coef, double trainingRate, int epochs)  //finds the model that fits the dataset, returns coeficients a0...an
        {
            for (int i = 0; i < epochs; i++)
            {
                double[] pred = makePredictions(xList, coef);
                coef = updateCoef(xList, yList, coef, pred, trainingRate);
            }
            return coef;
            
        }

        static double[] makePredictions(double[,] xList, double[] coef)  //generates a predicted probability for each set of x values given the weighs for each value
        {
            double[] pred = new double[xList.GetLength(0)];
            for (int j = 0; j < pred.Length; j++)
            {
                pred[j] = coef[0];
                for (int i = 1; i < coef.Length; i++)
                {
                    pred[j] = pred[j] + coef[i] * xList[j, i - 1];
                }
                pred[j] = 1 / (1 + Math.Exp(-pred[j]));
            }
            return pred;
        }

        static double[] updateCoef(double[,] xList, double[] yList, double[] coef, double[] pred, double learningRate)  //updates each coefficient using gradient descent and cross-entropy loss function
        {
            double cost = 0;
            double[] xError = new double[coef.Length - 1];
            for (int i = 0; i < yList.Length; i++)
            {
                cost = cost + yList[i] * Math.Log(pred[i]) + (1 - yList[i]) * Math.Log(1 - pred[i]);
            }
            Console.WriteLine(cost);
            cost = (-1 * cost) / yList.Length;
            for (int j = 0; j < yList.Length; j++)
            {
                for (int i = 0; i < xError.Length; i++)
                {
                    xError[i] = xError[i] + xList[j, i] * cost;
                }
            }
            coef[0] = coef[0] - (learningRate * cost) / yList.Length;
            for (int i = 1; i < coef.Length; i++)
            {
                coef[i] = coef[i] - (learningRate * xError[i-1]) / yList.Length;
            }
            testAccuracy(pred, yList);
            return coef;
        }

        static void testAccuracy(double[] pred, double[] yList)
        {
            double boundary = 0.5;
            double[] cMatrix = new double[4]; //cMatrix in form TP, FP, TN, FN
            for (int i = 0; i < yList.Length; i++)
            {
                int c = 0;
                if (pred[i] > boundary)
                {
                    c = 1;
                    if (c == yList[i])
                    {
                        cMatrix[0] += 1;
                    }
                    else
                    {
                        cMatrix[1] += 1;
                    }
                }
                else
                {
                    if (c == yList[i])
                    {
                        cMatrix[2] += 1;
                    }
                    else
                    {
                        cMatrix[3] += 1;
                    }
                }
            }
            double a = (cMatrix[0] + cMatrix[2]) / (cMatrix.Sum());
            double r = cMatrix[0] / (cMatrix[0] + cMatrix[3]);
            double p = cMatrix[0] / (cMatrix[0] + cMatrix[1]);
            double f1 = 2 * ((p * r) / (p + r));
            Console.WriteLine("Accuracy: {0}", a);
            Console.WriteLine("f1-Score: {0}", f1);
        }

        static void outputTest(double[] coef, double[] yList, double[,] xList)
        {
            double boundary = 0.5;
            double[] pred = makePredictions(xList, coef);
            for (int i = 0; i < pred.Length; i++)
            {
                if (pred[i] > boundary)
                {
                    pred[i] = 1;
                    Console.WriteLine("Predicted class: {0}  |  Actual class: {1}", pred[i], yList[i]);
                }
                else
                {
                    pred[i] = 0;
                    Console.WriteLine("Predicted class: {0}  |  Actual class: {1}", pred[i], yList[i]);
                }
            }
        }
    }
        
}
