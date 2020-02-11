using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    class Calculation
    {
        public Graph Graph { get; set; }

        public Node Start { get; set; }

        public Node End { get; set; }

        public int NodeVisits { get; private set; }

        public List<Node> ShortestPath;
        
        public double ShortestPathCost { get; private set; }


        public Calculation(Graph graph)
        {
            Graph = graph;

            End = graph.StartNode;

            Start = graph.EndNode;
        }


        public List<Node> GetShortestPathDijkstra()
        {
            DijkstraSearch();
            var shortestPath = new List<Node>();
            shortestPath.Add(End);
            BuildShortestPath(shortestPath, End);
            shortestPath.Reverse();
            return shortestPath;
        }

        private void DijkstraSearch()
        {
            NodeVisits = 0;
            Start.MinCostToStart = 0;
            var prioQueue = new List<Node>();
            prioQueue.Add(Start);
            do
            {
                NodeVisits++;
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                foreach (var cnn in node.Destinations.OrderBy(x => x.Cost))
                {
                    var childNode = cnn.Destination;
                    if (childNode.Visited)
                        continue;
                    if(childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }

            } while (prioQueue.Any());
        }


        private void BuildShortestPath(List<Node> list, Node node)
        {
            if (node.NearestToStart == null)
                return;
            list.Add(node.NearestToStart);
            ShortestPathCost += node.Destinations.Single(x => x.Destination == node.NearestToStart).Cost;
            BuildShortestPath(list, node.NearestToStart);
        }

        
    }

}
