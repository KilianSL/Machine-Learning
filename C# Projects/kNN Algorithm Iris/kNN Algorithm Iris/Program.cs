using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataScience;

namespace kNN_Algorithm_Iris
{
    class Program
    {

        public static void Main(string[] args)
        {
            (double[,] xList, double[,] xTest, int[] yList, int[] yTest) = importData(4);   //class 0 is Iris-setosa, class 1 is Iris-versicolor, class 2 is Iris-virginica
            int k = 5;
            makePrediction(xTest, yTest, xList, yList, k, 3);
            Console.ReadLine();
        }

        static (double[,] xList, double[,] xTest, int[] yList, int[] yTest) importData(int c)
        {
            CSVReader csv = new CSVReader();
            string path1 = @"C:\Users\kilian\source\repos\kNN Algorithm Iris\kNN Algorithm Iris\Iris.csv";
            string path2 = @"C:\Users\kilian\source\repos\kNN Algorithm Iris\kNN Algorithm Iris\test.csv";
            int[] yList = csv.ReadInteger(path1, 4);
            int[] yTest = csv.ReadInteger(path2, 4);
            double[,] xList = new double[yList.Length, c];
            double[,] xTest = new double[yTest.Length, c];
            for (int i = 0; i < 3; i++)
            {
                double[] x = csv.ReadDouble(path1, i);
                double[] xt = csv.ReadDouble(path2, i);
                for (int j = 0; j < yList.Length; j++)
                {
                    xList[j, i] = x[j];
                }
                for (int j = 0; j < xt.Length; j++)
                {
                    xTest[j, i] = xt[j];
                }
            }
            return (xList, xTest, yList, yTest);

        }

        static double euclideanDistance(double[] x, double[] y)
        {
            double distance = 0;
            for (int i = 0; i < x.Length; i++)
            {
                distance += Math.Pow((x[i] - y[i]), 2);
            }
            distance = Math.Sqrt(distance);
            return distance;
        }

        static int[] getkNeighbours(double[,] xList, int[] yList, double[] testInstance, int k)   //returns an array of the k closest neighbours
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
        }

        static int[] sortNeighbours(double[,] distances)    //returns an array of all points sorted by distances
        {
            int n = distances.GetLength(0);
            int[] neighbours = new int[n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (distances[j,0] > distances[j+1,0])
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
        }

        static int getModalNeighbour(int[] neighbours, int classes, int k)
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

        static void makePrediction(double[,] xtest, int[] yTest, double[,] xList, int[] yList, int k, int classes)   //returns a score for the accuracy of the model, and prints out the test data.
        {
            Dictionary<int, string> dict = new Dictionary<int, string> { { 0, "Iris-setosa" }, { 1, "Iris-versicolor" }, { 2, "Iris-virginica" } };
            int correct = 0;
            int total = 0;
            for (int i = 0; i < yTest.Length; i++)
            {
                double[] xInstance = new double[xList.GetLength(1)];
                for (int j = 0; j < xInstance.Length; j++)
                {
                    xInstance[j] = xtest[i, j];
                }
                int[] neighbours = getkNeighbours(xList, yList, xInstance, k);
                int predicted = getModalNeighbour(neighbours, classes, k);
                if (predicted == yTest[i])
                {
                    correct += 1;
                }
                total += 1;
                Console.WriteLine("Predicted Class: {0}   |   Actual Class: {1}", dict[predicted], dict[yTest[i]]);
            }
        }
    }
}
