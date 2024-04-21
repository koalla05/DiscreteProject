using System.Net.NetworkInformation;
using MaximumFlow;
using ScottPlot;
using SkiaSharp;

static void CreatePlotMatrix(Dictionary<int, Color> nodes)
{
    
    double[] capabilities = [20, 40, 60, 80, 100];
    var plt = new ScottPlot.Plot();

    foreach (var node in nodes.Keys)
    {
        var y = new List<double> { };

        foreach (var capability in capabilities)
        {
            var number = new List<double>();

            for (var k = 0; k < 15; k++)
            {
                var graph = new Graph(node, capability);
                var adjacencyMatrix = graph.AdjacencyMatrix();

                var result = graph.FordFulkersonForMatrix(adjacencyMatrix, 0, node - 1);
                plt.Add.Scatter(capability, result.Item2, color: nodes[node]);
                number.Add(result.Item2);
            }

            number.Sort();
            var point = number[6];
            y.Add(point);
        }

        var scatter = plt.Add.Scatter(capabilities, y.ToArray(), color: nodes[node]);
        scatter.Label = $"{node} вершин";
        scatter.LineWidth = 3;

        plt.Title("Матриця суміжності");
        plt.XLabel("Щільність");
        plt.YLabel("Час у наносекундах");
        plt.ShowLegend();

        plt.SavePng($"matrix_{nodes.Keys.ToString()}.png", 600, 1400);
        Console.WriteLine($"Ready with {node} nodes");
    }
}

static void CreatePlotList(Dictionary<int, Color> nodes)
{
    
    double[] capabilities = [20, 40, 60, 80, 100];
    var plt1 = new ScottPlot.Plot();

    foreach (var node in nodes.Keys)
    {
        var y = new List<double> { };

        foreach (var capability in capabilities)
        {
            var number = new List<double>();

            for (var k = 0; k < 10; k++)
            {
                var graph = new Graph(node, capability);
                var adjacencyMatrix = graph.AdjacencyMatrix();
                var adjacencyLists = graph.AdjacencyList(adjacencyMatrix);

                var result = graph.FordFulkersonForList(adjacencyLists, 0, node - 1);
                plt1.Add.Scatter(capability, result.Item2, color: nodes[node]);
                number.Add(result.Item2);
            }

            number.Sort();
            var point = number[4];
            y.Add(point);
        }

        var scatter = plt1.Add.Scatter(capabilities, y.ToArray(), color: nodes[node]);
        scatter.Label = $"{node} вершин";
        scatter.LineWidth = 3;

        plt1.Title("Списки суміжності");
        plt1.XLabel("Щільність");
        plt1.YLabel("Час у наносекундах");
        plt1.ShowLegend();

        plt1.SavePng($"list_{nodes.Keys.ToString()}.png", 400, 1600);
        Console.WriteLine($"Ready with {node} nodes");
    }
}


var nodes = new Dictionary<int, Color>
    {
        {20, Color.FromSKColor(SKColors.Aqua) },
        {40, Color.FromSKColor(SKColors.Coral) },
        {60, Color.FromSKColor(SKColors.LawnGreen) },
        {80 , Color.FromSKColor(SKColors.Brown)},
        {100 , Color.FromSKColor(SKColors.Pink)},
        {120 , Color.FromSKColor(SKColors.Chartreuse)},
        {140 , Color.FromSKColor(SKColors.Fuchsia)},
        {160 , Color.FromSKColor(SKColors.DarkBlue)},
        {180 , Color.FromSKColor(SKColors.Orchid)},
        {200 , Color.FromSKColor(SKColors.MediumVioletRed)},
    };

CreatePlotMatrix(nodes);
CreatePlotList(nodes);

