using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multipleLinearRegression
{
    class Program
    {
        static double[] readFile(string path, int column)
        {
            using (var reader = new System.IO.StreamReader(path))
            {
                List<double> listX = new List<double>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    listX.Add(double.Parse(values[column]));
                }
                return listX.ToArray();
            }
        }

        static void Main(string[] args)
        {
            double[] trCarat = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\train.csv", 0);
            double[] trColour = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\train.csv", 1);
            double[] trClarity = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\train.csv", 2);
            double[] trPrice = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\train.csv", 4);
            double[] teCarat = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\test.csv", 0);
            double[] teColour = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\test.csv", 1);
            double[] teClarity = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\test.csv", 2);
            double[] tePrice = readFile(@"c:\users\kilian\source\repos\multipleLinearRegression\multipleLinearRegression\test.csv", 4);
            Console.Write("Enter Learning Rate: ");
            double alpha = double.Parse(Console.ReadLine());
            Console.Write("Enter Epochs: ");
            int epochs = int.Parse(Console.ReadLine());
            double[] coef = findCoeffcient(trCarat, trClarity, trColour, trPrice, alpha, epochs);
            Console.WriteLine("Model is: Y = {0}x1 + {1}x2 + {2}x3 + {3}", coef[0], coef[1], coef[2], coef[3]);
            double rmse = RMSE(teCarat, teColour, teClarity, tePrice, coef[0], coef[1], coef[2], coef[3]);
            Console.ReadLine();
            Console.WriteLine("RMSE: {0}", rmse);
            Console.ReadLine();
        }

        static double[] findCoeffcient(double[] x1_list, double[] x2_list, double[] x3_list, double[] y_list, double learnRate, int epochs)
        {
            int n = y_list.Length;
            double x1 = 0;
            double x2 = 0;
            double x3 = 0;
            double c = 0;
            while (epochs > 0)
            {
                //Console.WriteLine(a);
                //Console.WriteLine(b);
                //Console.ReadKey();
                double[] pred = new double[n];
                for (int i = 0; i < n; i++)
                {
                    pred[i] = c + x1 * x1_list[i] + x2 * x2_list[i] + x3 * x3_list[i];
                }
                double sumError = 0;
                double sqSumError = 0;
                double x1SumError = 0;
                double x2SumError = 0;
                double x3SumError = 0;
                for (int i = 0; i < n; i++)
                {
                    double error = pred[i] - y_list[i];
                    sumError = sumError + (error);
                    sqSumError = sqSumError + Math.Pow(error, 2);
                    x1SumError = x1SumError + (error * x1_list[i]);
                    x2SumError = x2SumError + (error * x2_list[i]);
                    x3SumError = x3SumError + (error * x3_list[i]);
                }
                x1 = x1 - (learnRate * 2 * x1SumError) / n;
                x2 = x2 - (learnRate * 2 * x2SumError) / n;
                x3 = x3 - (learnRate * 2 * x3SumError) / n;
                c = c - (learnRate * 2 * sumError) / n;
                epochs = epochs - 1;
                if (epochs % 10 == 0)
                {
                    Console.WriteLine("MSE is: {0}", sqSumError / n);
                }
            }
            double[] coef = new double[] {x1,x2,x3,c};
            return coef;
        }

        static double RMSE(double[] x1_list,double[] x2_list, double[] x3_list, double[] y_list, double x1, double x2, double x3, double c)
        {
            double error = 0;
            double error2 = 0;
            for (int i = 0; i < y_list.Length; i++)
            {
                error = error + ((c + x1 * x1_list[i] + x2 * x2_list[i] + x3 * x3_list[i]) - y_list[i]);
                error2 = error2 + Math.Pow((c + x1 * x1_list[i] + x2 * x2_list[i] + x3 * x3_list[i]) - y_list[i], 2);
                if (i % 5 == 3)
                {
                    Console.WriteLine("Predicted value: {0}  || Actual Value: {1}", c + x1 * x1_list[i] + x2 * x2_list[i] + x3 * x3_list[i], y_list[i]);
                    Console.WriteLine("Error: {0}%", Math.Round(((c + x1 * x1_list[i] + x2 * x2_list[i] + x3 * x3_list[i]) - y_list[i])/y_list[i]*100),4);
                }
            }
            return Math.Sqrt(error2 / y_list.Length);
        }
    }
}
