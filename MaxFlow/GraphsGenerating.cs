using System.Collections;
using System.Xml.XPath;

namespace MaximumFlow
{
    public class Generation(int nodes, int capability)
    {
        public int Nodes = nodes;
        public int Capability = capability;
        

        public int[,] AdjacencyMatrix()
        {
            var matrix = new int[Nodes, Nodes];
            var maxEdges = Nodes * (Nodes - 1);
            var edges = maxEdges * Capability / 100;
            var list = ListOfIndexes(edges, Nodes);
            for (var i = 0; i < Nodes; i++)
            {
                for (var j = 0; j < Nodes; j++)
                {
                    var rand = new Random();
                    if (list.Contains(new Tuple<int, int>(i, j))) matrix[i, j] = rand.Next(10);
                    else matrix[i, j] = Int32.MaxValue;
                }
            }
            return matrix;
        }

        public List<Dictionary<int,int>>[] AdjacencyList(int[,] matrix)
        {
            var result = new List<Dictionary<int,int>>[Nodes];
            var maxEdges = Nodes * (Nodes - 1);
            var edges = maxEdges * Capability / 100;
            var list = ListOfIndexes(edges, Nodes);
            for (var i = 0; i < Nodes; i++)
            {
                for (var j = 0; j < Nodes; j++)
                {
                    if (result[i] == null)
                        result[i] = new List<Dictionary<int,int>>();
                    if (matrix[i, j] != Int32.MaxValue)
                        result[i].Add(new Dictionary<int, int>{{j, matrix[i,j]}});
                    
                }
            }
            /*foreach (var tuple in list)
            {
                var rand = new Random();
                if (result[tuple.Item1] == null)
                    result[tuple.Item1] = new List<Dictionary<int,int>>();
                result[tuple.Item1].Add(new Dictionary<int, int>(tuple.Item2, rand.Next(10)));
            }*/
            return result;
        }

        public static List<Tuple<int,int>> ListOfIndexes(int amount, int range)
        {
            var result = new List<Tuple<int, int>>();
            while (result.Count != amount)
            {
                var rand = new Random();
                var rand1 = rand.Next(range);
                var rand2 = rand.Next(range);
                while (rand1==rand2)
                    rand2 = rand.Next(range);
                

                var tuple = new Tuple<int, int>(rand1, rand2);
                if (!result.Contains(tuple))
                    result.Add(tuple);
            }
            return result;
        }
    }
    
    
}

