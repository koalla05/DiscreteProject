using System.Collections;
using System.Diagnostics;
using System.Xml.XPath;
namespace MaximumFlow
{
    public class Graph(int nodes, double capability)
    {
        public int Nodes = nodes;
        public double Capability = capability;
        public int[,] AdjacencyMatrix()
        {
            var matrix = new int[Nodes, Nodes];
            var maxEdges = Nodes * (Nodes - 1);
            var edges = Convert.ToInt32(maxEdges * Capability / 100);
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
            return result;
        }
        
        private static List<Tuple<int,int>> ListOfIndexes(int amount, int range)
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


        private bool BfsForMatrix(int[,] rGraph, int start, int finish, int[] parent)
        {
            var visited = new bool[Nodes];
            for (var i = 0; i<Nodes; ++i)
                visited[i] = false;

            var queue = new Queue<int>();
            queue.Enqueue(start);
            visited[start] = true;
            parent[start] = -1;

            while (queue.Count != 0)
            {
                var u = queue.Dequeue();
                for (var v = 0; v < Nodes; v++)
                {
                    if (visited[v] == false &&  rGraph[u, v] > 0 && rGraph[u,v] < Int32.MaxValue)
                    {
                        if (v == finish)
                        {
                            parent[v] = u;
                            return true;
                        }
                        queue.Enqueue(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }
            return false;
        }

        private bool BfsForList(List<Dictionary<int, int>>[] graph, int start, int finish, out int[] parent)
        {
            parent = new int[graph.Length];
            Array.Fill(parent, -1);

            var visited = new bool[graph.Length];
            visited[start] = true;

            var queue = new Queue<int>();
            queue.Enqueue(start);

            while (queue.Count != 0)
            {
                var u = queue.Dequeue();
                foreach (var neighbor in graph[u])
                {
                    if (neighbor.Count == 0) continue; 
                    var v = neighbor.Keys.First();
                    if (visited[v] || GetCapacity(graph, u, v) <= 0) continue;
                    parent[v] = u;
                    visited[v] = true;
                    queue.Enqueue(v);
                }
            }

            return visited[finish];
        }

        public Tuple<int, double> FordFulkersonForMatrix(int[,] graph, int start, int finish)
        {
            long timer = Stopwatch.GetTimestamp();
            var capacityGraph = new int[Nodes, Nodes];
            for (var u = 0; u < Nodes ; u++)
            {
                for (var v = 0; v < Nodes; v++)
                {
                    capacityGraph[u, v] = graph[u, v];
                }
            }
            var parent = new int[Nodes];
            var maxFlow = 0;
            while (BfsForMatrix(capacityGraph, start, finish, parent))
            {
                var pathFlow = int.MaxValue;
                for (var v = finish; v != start; v = parent[v]) {
                    var u = parent[v];
                    pathFlow = Math.Min(pathFlow, capacityGraph[u, v]);
                    capacityGraph[u, v] -= pathFlow;
                    capacityGraph[v, u] += pathFlow;
                }
                maxFlow += pathFlow;
            }
            
            return new Tuple<int, double>(maxFlow, Stopwatch.GetElapsedTime(timer).TotalNanoseconds);
        }
        private static int GetCapacity(List<Dictionary<int, int>>[] graph, int u, int v)
        {
            foreach (var neighbor in graph[u])
            {
                if (neighbor.TryGetValue(v, out var capacity))
                {
                    return capacity;
                }
            }
            return 0;
        }

        private static void UpdateFlow(List<Dictionary<int, int>>[] graph, int u, int v, int flow)
        {
            for (var i = 0; i < graph[u].Count; i++)
            {
                var neighbor = graph[u][i];
                if (!neighbor.ContainsKey(v)) continue;
                graph[u][i][v] -= flow;
                if (graph[u][i][v] == 0)
                {
                    graph[u][i].Remove(v);
                }
                break;
            }

            var found = false;
            for (var i = 0; i < graph[v].Count; i++)
            {
                var neighbor = graph[v][i];
                if (neighbor.ContainsKey(u))
                {
                    graph[v][i][u] += flow;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                graph[v].Add(new Dictionary<int, int> { { u, flow } });
            }
        }
        public Tuple<int, double> FordFulkersonForList( List<Dictionary<int, int>>[] graph, int start, int finish)
        {
            long timer = Stopwatch.GetTimestamp();
            var maxFlow = 0;
            var newGraph = new List<Dictionary<int, int>>[graph.Length];
            for (var i = 0; i < graph.Length; i++)
            {
                newGraph[i] = new List<Dictionary<int, int>>();
                foreach (var neighbor in graph[i])
                {
                    newGraph[i].Add(new Dictionary<int, int>(neighbor));
                }
            }
            while (BfsForList(newGraph, start, finish, out var parent))
            {
                var pathFlow = int.MaxValue;
                
                for (var v = finish; v != start; v = parent[v])
                {
                    var u = parent[v];
                    var capacity = GetCapacity(newGraph, u, v);
                    pathFlow = Math.Min(pathFlow, capacity);
                    UpdateFlow(newGraph, u, v, pathFlow);
                }
                maxFlow += pathFlow;
            }
            return new Tuple<int, double>(maxFlow, Stopwatch.GetElapsedTime(timer).TotalNanoseconds);
        }
    }
}