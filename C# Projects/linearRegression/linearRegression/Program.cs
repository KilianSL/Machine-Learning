using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace linearRegression
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
            double[] x_test = readFile(@"C:\Users\kilian\source\repos\linearRegression\linearRegression\lr_test.csv", 0);
            double[] y_test = readFile(@"C:\Users\kilian\source\repos\linearRegression\linearRegression\lr_test.csv", 1);
            double[] x_train = readFile(@"C:\Users\kilian\source\repos\linearRegression\linearRegression\lr_train.csv", 0);
            double[] y_train = readFile(@"C:\Users\kilian\source\repos\linearRegression\linearRegression\lr_train.csv", 1);
            Console.Write("Enter Learning Rate: ");
            double alpha = double.Parse(Console.ReadLine());
            Console.Write("Enter Epochs: ");
            int epochs = int.Parse(Console.ReadLine());
            double[] ab = findCoeffcient(x_train, y_train, alpha, epochs);
            Console.WriteLine("The relationship is modelled by y = {0}x + {1}", ab[0], ab[1]);
            double rmse = RMSE(x_test, y_test, ab[0], ab[1]);
            
            Console.WriteLine();
            Console.WriteLine("Root Mean Squared Error: {0}", rmse);
            Console.ReadLine();

        }

        static double RMSE(double [] x_list, double[] y_list, double a, double b)
        {
            double error = 0;
            double error2 = 0;
            for (int i = 0; i < x_list.Length; i++)
            {
                error = error + (a * x_list[i] + b) - y_list[i];
                error2 = error2 + Math.Pow((a * x_list[i] + b) - y_list[i], 2);
                if (i % 10 == 0)
                {
                    Console.WriteLine("Predicted value: {0}  || Actual Value: {1}", a * x_list[i] + b, y_list[i]);
                    Console.WriteLine("Error: {0}%", Math.Round(((a * x_list[i] + b) - y_list[i])/y_list[i] * 100, 2));
                }
            }
            return Math.Sqrt(error2 / x_list.Length);
        }
        static double[] findCoeffcient(double[] x_list, double[] y_list, double alpha, int epochs)
        {
            int f = epochs/10;
            Console.WriteLine("Learning Model - Please Wait");
            Console.Write("<");
            double n = 700;
            double a = 0;
            double b = 0;
            while (epochs > 0)
            {
                //Console.WriteLine(a);
                //Console.WriteLine(b);
                //Console.ReadKey();
                double[] pred = new double[700];
                for (int i = 0; i < 700; i++)
                {
                    pred[i] = b + a * x_list[i];
                }
                double sumError = 0;
                double sqSumError = 0;
                double xSumError = 0;
                for (int i = 0; i < 700; i++)
                {
                    double error = pred[i] - y_list[i];
                    sumError = sumError + (error);
                    sqSumError = sqSumError + Math.Pow(error, 2);
                    xSumError = xSumError + (error * x_list[i]);
                }
                a = a - (alpha * 2 * xSumError) / n;
                b = b - (alpha * 2 * sumError) / n;
                epochs = epochs - 1;
                if (epochs % f == 0)
                {
                    Console.Write("=");
                }
            }
            Console.Write(">");
            Console.WriteLine();
            Console.WriteLine("Training Done! - Press RTN to continue");
            Console.ReadLine();
            double[] ab = new double[] { a, b };
            return ab;
        }
    }
}
