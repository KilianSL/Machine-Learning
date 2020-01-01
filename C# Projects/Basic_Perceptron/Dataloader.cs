using System;
using System.Collections.Generic;
using System.Text;

namespace PNIS
{
    public static class DataLoader  //allows data to be read from a file to a .NET array. 
    {
        public static (float[,], float[]) LoadAllData(string path, bool shuffle = false)  //returns a tuple float arrays containing data and labels
        {

            if (shuffle)
            {
                (float[,] data, float[] labels) = LoadData(path);
                return ShuffleData(data, labels, 50);
            }
            else
            {
                return LoadData(path);
            }
        }

        private static (float[,], float[]) LoadData(string path)
        {
            int[] shape = GetDataShape(path);
            float[,] data = new float[shape[0], shape[1] - 1];
            float[] labels = new float[shape[0]];
            using (var reader = new System.IO.StreamReader(path))
            {
                int row = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    for (int i = 0; i < values.Length - 1; i++)
                    {
                        data[row, i] = Convert.ToSingle(values[i]);
                    }
                    labels[row] = Convert.ToSingle(values[shape[1] - 1]);
                    row += 1;
                }
                reader.Close();
                return (data, labels);
            }
        }

        private static (float[,], float[]) ShuffleData(float[,] data, float[] labels, int numIterations)
        {
            var rand = new Random();
            while (numIterations > 0)
            {
                int i1 = rand.Next(0, data.GetLength(0));
                int i2 = rand.Next(0, data.GetLength(0));
                for (int i = 0; i < data.GetLength(1); i++)
                {
                    var temp = data[i1, i];
                    data[i1, i] = data[i2, i];
                    data[i2, i] = temp;
                }
                var _ = labels[i1];
                labels[i1] = labels[i2];
                labels[i2] = _;
                numIterations -= 1;
            }
            return (data, labels);
        }

        private static int[] GetDataShape(string path) //returns the number of columns in the given dataset, to determine the shape of the array
        {
            using (var reader = new System.IO.StreamReader(path))
            {
                List<string[]> dataList = new List<string[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    dataList.Add(values);
                }
                reader.Close();
                Console.WriteLine("{0} rows , {1} columns", dataList.Count, dataList[0].Length);
                return new int[] { dataList.Count, dataList[0].Length };
            }
        }
    }
}
