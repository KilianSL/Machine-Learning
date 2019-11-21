using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScience
{
    public class CSVReader
    {
        
        public double[] ReadDouble(string path, int column)
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

        public string[] ReadString(string path, int column)
        {
            using (var reader = new System.IO.StreamReader(path))
            {
                List<string> listX = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    listX.Add(values[column]);
                }
                return listX.ToArray();
            }
        }

        public int[] ReadInteger(string path, int column)
        {
            using (var reader = new System.IO.StreamReader(path))
            {
                List<int> listX = new List<int>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    listX.Add(int.Parse(values[column]));
                }
                return listX.ToArray();
            }
        }
    }

    public class BivariateLinearRegression   //subroutines that fit and test the model y = ax + b in the form [a,b]
    {
        public double[] FindModel(double[] xList, double[] yList, double learningRate, int epochs) //takes the input list, output list, learning rate and number of training epochs 
        {
            int n = xList.Length;
            double a = 0;
            double b = 0;
            while (epochs > 0)
            {
                double[] pred = new double[n];
                for (int i = 0; i < n; i++)
                {
                    pred[i] = b + a * xList[i];
                }
                double sumError = 0;
                double sqSumError = 0;
                double xSumError = 0;
                for (int i = 0; i < n; i++)
                {
                    double error = pred[i] - yList[i];
                    sumError = sumError + (error);
                    sqSumError = sqSumError + Math.Pow(error, 2);
                    xSumError = xSumError + (error * xList[i]);
                }
                a = a - (learningRate * 2 * xSumError) / n;
                b = b - (learningRate * 2 * sumError) / n;
                epochs = epochs - 1;
            }
            double[] ab = new double[] { a, b };
            return ab;
        }
        public double TestRMSE(double[] xList, double[] yList, double a, double b) //tests the model y = ax + b against a set of test data, and returns the root mean square error
        {
            double error = 0;  //sum of the difference between the actual and the predicated values
            double error2 = 0; //sum of the square of the error
            for (int i = 0; i < xList.Length; i++)
            {
                error = error + (a * xList[i] + b) - yList[i];
                error2 = error2 + Math.Pow((a * xList[i] + b) - yList[i], 2);
                if (i % 10 == 0)
                {
                    Console.WriteLine("Predicted value: {0}  || Actual Value: {1}", a * xList[i] + b, yList[i]);
                    Console.WriteLine("Error: {0}%", Math.Round(((a * xList[i] + b) - yList[i]) / yList[i] * 100, 2));
                }
            }
            return Math.Sqrt(error2 / xList.Length);
        }
    }

    public class kNearestNeighbours
    {
        public int MakePrediction(double[,] xList, int[] yList, int classes, int k, double[] xInstance)  //returns the predicted class for the given xInstance. 
        {                                                                                                //assumes classes are integers starting from 0 - treat data
            int prediction;                                                                              //xList:training predicators, yList:training classes, classes:no. of classes
            int[] kNeighbours = getKNeighbours(xList, yList, xInstance, k);
            prediction = getModalNeighbour(kNeighbours, classes, k);
            return prediction;
        }

        private double euclideanDistance(double[] x, double[] y)   //gets the euclidean distances between 2 n-dimensional vectors x and y
        {
            double distance = 0;
            for (int i = 0; i < x.Length; i++)
            {
                distance += Math.Pow((x[i] - y[i]), 2);
            }
            distance = Math.Sqrt(distance);
            return distance;
        }

        private int[] getKNeighbours(double[,] xList, int[] yList, double[] testInstance, int k)
        {
            double[,] distances = new double[yList.Length, 2]; //every object in the train data, in the format [distance from test point, recorded class] 
            for (int i = 0; i < yList.Length; i++)
            {
                double[] trainInstance = new double[xList.GetLength(1)];
                for (int j = 0; j < trainInstance.Length; j++)
                {
                    trainInstance[j] = xList[i, j];
                }
                distances[i, 0] = euclideanDistance(testInstance, trainInstance);
                distances[i, 1] = yList[i];
            }
            int[] sNeighbours = sortNeighbours(distances);
            int[] kNeighbours = new int[k];
            for (int i = 0; i < k; i++)
            {
                kNeighbours[i] = sNeighbours[i];
            }
            return kNeighbours;
        }   //returns the closest k classes to the test instance

        private int[] sortNeighbours(double[,] distances)
        {
            int n = distances.GetLength(0);
            int[] neighbours = new int[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (distances[j, 0] > distances[j + 1, 0])
                    {
                        double tempDist = distances[j, 0];
                        double tempClass = distances[j, 1];
                        distances[j, 0] = distances[j + 1, 0];
                        distances[j, 1] = distances[j + 1, 1];
                        distances[j + 1, 0] = tempDist;
                        distances[j + 1, 1] = tempClass;
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                neighbours[i] = Convert.ToInt32(distances[i, 1]);
            }
            return neighbours;
        }   //uses a bubble sort to sort the training classes by distance from the test instance

        private int getModalNeighbour(int[] neighbours, int classes, int k)  //gets the most common neighbour out of the k closest neighbours 
        {
            int mClass = 0;
            int[] classVotes = new int[classes];
            for (int i = 0; i < k; i++)
            {
                int c = neighbours[i];
                classVotes[c] += 1;
            }
            int maxVote = 0;
            for (int i = 0; i < classes; i++)
            {
                if (classVotes[i] > maxVote)
                {
                    maxVote = classVotes[i];
                    mClass = i;
                }
            }
            return mClass;
        }
    }

}
