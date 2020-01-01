using System;
using PNIS.Matrix;
using PNIS;
namespace Basic_Perceptron
{
    class Program
    {
        static void Main(string[] args)  //perceptron to predicted if diabetes is present based on 8 predicator variables
        {
            string path = @"C:\Users\kilian\source\Git Repos\Machine-Learning\C# Projects\Basic_Perceptron\indians.csv";
            (var data, var labels) = PNIS.DataLoader.LoadAllData(path);
            var x = new Matrix(data);
            var y = new Matrix(labels);
            var model = new Perceptron.Perceptron(8);
            model.Train(x, y, 1);
        }
    }
}
