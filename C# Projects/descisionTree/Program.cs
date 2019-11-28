using System;
using System.Collections.Generic;
using System.Linq;
using DataScience;

namespace descisionTree
{
    class Node  //represents a new node, with the attribute to split by the point to split from, and the two child nodes that it links to
    {
        public string type { get; set; } //stores the type of the node, defaults to node
        public int split_varible;  //column index of the array to split by
        public int layer { get; set; } //the layer of the tree that the node sits on (0 = root node) 
        public Node parent { get; set; }  //the parent node of the node (null for root node)
        public double splitpoint; //the point to split on (or value if it is a leaf)
        public Node left_child { get; set; }   //the node for data that falls below the split point
        public Node right_child { get; set; }  //the node for data that falls above the splitpoint
        public double[,] data;  //the training data for the node
        public double score = 0;  //the Gini score of the node
        public Node(double[,] d, int l, Node p=null, string t="node", int v=0)  //sets the data that the node stores, depending on the type of node. Sets value if it is a leaf
        {
            type = t;
            if (type == "node")
            {
                data = d;
                layer = l;
                parent = p;
                Console.WriteLine("New Node Created");
                findSplitAttribute();
                Console.WriteLine("Selected Split Attribute: {0}", split_varible);
                Console.WriteLine("Selected Split Point: {0}", splitpoint);
                Console.WriteLine("Node Score: {0}", score);
                Console.WriteLine();
                left_child = null;
                right_child = null;
            }
            else
            {
                splitpoint = v; //set to value that the leaf should output
                Console.WriteLine("Leaf created");
                Console.WriteLine("Output Value: {0}", splitpoint);
                Console.WriteLine();
                left_child = null;
                right_child = null;
            }
            
        }
        private void findSplitAttribute() //finds the best split for that node, and updates the node variables accordingly 
        {
            for (int i = 0; i < data.GetLength(1) - 1; i++)
            {
                double sPoint = findSplitPoint(i);
                (double[] L, double[] R) = predictSets(i, sPoint);
                double tScore = Gini(L, R);
                if (tScore > score)
                {
                    score = tScore;
                    split_varible = i;
                    splitpoint = sPoint;
                }
                //Console.WriteLine("Test Attribute: {0}  |  Score: {1}", i, tScore);
            }
            
        }
        private double findSplitPoint(int splitVar)  //finds the best point (out of all available data) at which to split the dataset
        {
            double testSplit;
            double bestSplit = new double();
            double bScore = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                testSplit = data[i, splitVar];
                (double[] L, double[] R) = predictSets(splitVar, testSplit);
                double tScore = Gini(L, R);
                
                //Console.WriteLine("Test Point: {0}  |  Score: {1}", testSplit, tScore);
                if (tScore > bScore)
                {
                    bScore = tScore;
                    bestSplit = testSplit;
                }
            }
            return bestSplit;
        }

        private (double[], double[]) predictSets(int sVar, double sPoint)   //splits the array based on the split point
        {
            List<double> listL = new List<double>();
            List<double> listR = new List<double>();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, sVar] >= sPoint)
                {
                    listL.Add(data[i, data.GetLength(1) - 1]);
                }
                else
                {
                    listR.Add(data[i, data.GetLength(1) - 1]);
                }
            }
            double[] L = listL.ToArray();
            double[] R = listR.ToArray();
            return (L, R);
        }

        private double Gini(double[] L, double[] R)  //calculates the Gini score for a given two sets
        {
            double L0 = 0;
            double L1 = 1;
            foreach (var item in L)
            {
                if (item == 0)
                {
                    L0 += 1;
                }
                else
                {
                    L1 += 1;
                }
            }
            double R0 = 0;
            double R1 = 1;
            foreach (var item in R)
            {
                if (item == 0)
                {
                    R0 += 1;
                }
                else
                {
                    R1 += 1;
                }
            }
            double gR = Math.Pow(R0 / (R0 + R1), 2) + Math.Pow(R1 / (R0 + R1), 2);
            double gL = Math.Pow(L0 / (L0 + L1), 2) + Math.Pow(L1 / (L0 + L1), 2);
            double g = gR * (R0+R1)/(R0 + R1 + L0 + L1) + gL * (L0+L1)/(R0 + R1 + L0 + L1);
            return g;
        }

        
        
    }

    class Tree //represents the entire tree, made up of connected nodes and leaves
    {
        double[,] dataset { get; set; }
        int maxDepth { get; set; }
        int depth = 1;
        double minError;
        private Node[] tree { get; }
        public Tree(double[,] dset, int mDepth, double minE=1)
        {
            minError = minE;
            dataset = dset;
            maxDepth = mDepth;
            Console.WriteLine("Building tree...");
            tree = buildTree();
        }

        private Node[] buildTree()
        {
            List<Node> nodes = new List<Node>(); //list of every node in the tree
            Console.WriteLine("Root Node:");
            nodes.Add(new Node(dataset, 0)); // creates a root node, with the entire dataset
            (double[,] L, double[,] R) = Split(dataset, nodes[0]);
            while (depth <= maxDepth)  //iterates until maxdepth is reached
            {
                Console.WriteLine("Depth: {0}", depth);
                List<Node> newNodes = new List<Node>(); //temporary list to store each newly created node
                foreach (var n in nodes)  //iterates through each node in the tree
                {
                    if (n.type == "node")  //checks if n is a node or a leaf - leaves don't need any extra nodes
                    {
                        int layer = n.layer + 1;
                        (L, R) = Split(n.data, n);
                        if (n.left_child == null)  //if node is missing a child node, it creates a new node using the data passed from the last one
                        {
                            if (n.score >= minError)  //creates a leaf if the node score is above a given threshold
                            {
                                int class0 = 0;
                                int class1 = 0;
                                for (int i = 0; i < L.GetLength(0); i++)
                                {
                                    if (L[i, L.GetLength(1) - 1] == 0)
                                    {
                                        class0 += 1;
                                    }
                                    else
                                    {
                                        class1 += 1;
                                    }
                                }
                                if (class0 > class1)
                                {
                                    Node n1 = new Node(L, layer, n, "leaf");
                                    n.left_child = n1;
                                    newNodes.Add(n1);
                                }
                                else
                                {
                                    Node n1 = new Node(L, layer, n, "leaf", 1);
                                    n.left_child = n1;
                                    newNodes.Add(n1);
                                }
                            }
                            else
                            {
                                Node n1 = new Node(L, layer, n);
                                n.left_child = n1;
                                newNodes.Add(n1);
                            }
                        }

                        if (n.right_child == null)
                        {
                            if (n.score >= minError)  //checks whether to create a leaf or a node, based on the score of the previous node
                            {
                                int class0 = 0;
                                int class1 = 0;
                                for (int i = 0; i < R.GetLength(0); i++)
                                {
                                    if (R[i, R.GetLength(1) - 1] == 0)
                                    {
                                        class0 += 1;
                                    }
                                    else
                                    {
                                        class1 += 1;
                                    }
                                }
                                if (class0 > class1)
                                {
                                    Node n1 = new Node(R, layer, n, "leaf");
                                    n.right_child = n1;
                                    newNodes.Add(n1);
                                }
                                else
                                {
                                    Node n1 = new Node(L, layer, n, "leaf", 1);
                                    n.left_child = n1;
                                    newNodes.Add(n1);
                                }
                            }
                            else
                            {
                                Node n1 = new Node(R, layer, n);
                                n.right_child = n1;
                                newNodes.Add(n1);
                            }

                        }
                    }
                    
                }
                foreach (var n in newNodes)
                {
                    nodes.Add(n);
                }
                depth += 1;
            }
            List<Node> leaves = new List<Node>();
            foreach (var n in nodes) //creates a leaf node at the end of every loose node, as max depth has been reached
            {
                if (n.type == "node")
                {
                    int layer = n.layer + 1;
                    (L, R) = Split(n.data, n);
                    int class0 = 0;
                    int class1 = 0;
                    for (int i = 0; i < L.GetLength(0); i++)
                    {
                        if (L[i, L.GetLength(1) - 1] == 0)
                        {
                            class0 += 1;
                        }
                        else
                        {
                            class1 += 1;
                        }
                    }
                    if (class0 > class1)
                    {
                        Node n1 = new Node(L, layer, n, "leaf");
                        n.left_child = n1;
                        leaves.Add(n1);
                    }
                    else
                    {
                        Node n1 = new Node(L, layer, n, "leaf", 1);
                        n.left_child = n1;
                        leaves.Add(n1);
                    }
                }
                
            }
            Node[] tree = nodes.ToArray();
            return tree;
        }

        private (double[,], double[,]) Split(double[,] arr, Node n)   //splits the array based on the split point of the node
        {
            List<double[]> listL = new List<double[]>();
            List<double[]> listR = new List<double[]>();
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                double[] x = new double[arr.GetLength(1)];
                for (int j = 0; j < x.Length; j++)
                {
                    x[j] = arr[i, j];
                }
                if (x[n.split_varible] >= n.splitpoint)
                {
                    listR.Add(x);
                }
                else
                {
                    listL.Add(x);
                }
            }
            double[,] R = new double[listR.Count, arr.GetLength(1)];
            for (int i = 0; i < listR.Count; i++)
            {
                double[] x = listR.ElementAt(i);
                for (int j = 0; j < x.Length; j++)
                {
                    R[i, j] = x[j];
                }
            }
            double[,] L = new double[listL.Count, arr.GetLength(1)];
            for (int i = 0; i < listL.Count; i++)
            {
                double[] x = listL.ElementAt(i);
                for (int j = 0; j < x.Length; j++)
                {
                    L[i, j] = x[j];
                }
            }
            return (L, R);
        }

        public void makePrediction(double[] data)  //makes a prediction for the given data points
        {
            Node n = tree[0];
            while (n.type == "node")
            {
                if (data[n.split_varible] < n.splitpoint)
                {
                    if (n.left_child != null)
                    {
                        n = n.left_child;
                    }
                    else
                    {
                        n = n.right_child;
                    }   
                }
                else
                {
                    if (n.right_child != null)
                    {
                        n = n.right_child;
                    }
                    else
                    {
                        n = n.left_child;
                    }
                }
            }
            Console.WriteLine("Predicated Value: {0}  |  Actual Value: {1}", n.splitpoint, data[data.Length - 1]);
            
        }

    }





    class Program
    {

        static double[,] loadData(string path)
        {
            var csv = new DataScience.CSVReader();
            double[] x = csv.ReadDouble(path, 0);
            double[,] data = new double[x.Length, 5];
            for (int i = 0; i < data.GetLength(1); i++)
            {
                x = csv.ReadDouble(path, i);
                for (int j = 0; j < x.Length; j++)
                {
                    data[j, i] = x[j];
                }
            }
            return data;
        }

        static void testTree(double[,] data, Tree tree)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                double[] x = new double[data.GetLength(1)];
                for (int j = 0; j < x.Length; j++)
                {
                    x[j] = data[i, j];
                }
                tree.makePrediction(x);
            }
        }

        static void Main(string[] args)
        {
            double[,] data = loadData(@"C:\Users\kilian\source\Git Repos\Machine-Learning\C# Projects\descisionTree\banknote.csv");
            double[,] testData = loadData(@"C:\Users\kilian\source\Git Repos\Machine-Learning\C# Projects\descisionTree\testData.csv");
            var tree = new Tree(data, 5);
            testTree(testData, tree);

        }
    }
}
