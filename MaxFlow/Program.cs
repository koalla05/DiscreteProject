using MaximumFlow;
var graph = new Generation(5, 50);
var adjacencyMatrix = graph.AdjacencyMatrix();
var adjacencyList = graph.AdjacencyList(adjacencyMatrix);
Console.WriteLine();