using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation
{
    public class Router
    {
        private readonly ILayoutFactory _layoutFactory;

        private readonly Graph Graph;

        //private readonly RoutingState _routingState;


        public int NodeVisits { get; private set; }

        public double ShortestPathCost { get; private set; }

        public List<CalcNode> CalcNodes { get; set; } = new List<CalcNode>();


        public Router(ILayoutFactory lf)
        {
            _layoutFactory = lf;
            Graph = lf.GetLayout();
        }


        public List<string> GetShortestPathDijkstra(string startNode, string endNode)
        {
            Node StartNode = Graph.Nodes.FirstOrDefault(x => x.Name == startNode);
            Node EndNode = Graph.Nodes.FirstOrDefault(x => x.Name == endNode);

            RoutingState routingState = new RoutingState(StartNode, EndNode);

            DijkstraSearch(routingState);
            List<string> result;
            var shortestPath = new List<CalcNode>();

            shortestPath.Add(routingState.End);
            BuildShortestPath(shortestPath, routingState.End);
            shortestPath.Reverse();
            result = shortestPath.Select(x => x.Name).ToList();

            return result;
        }


        private void DijkstraSearch(RoutingState routingstate)
        {
            NodeVisits = 0;
            routingstate.Start.MinCostToStart = 0;
            routingstate.PrioQueue = new List<CalcNode>();
            routingstate.Visited = new List<CalcNode>();

            routingstate.PrioQueue.Add(routingstate.Start);
            do
            {
                NodeVisits++;
                routingstate.PrioQueue = routingstate.PrioQueue.OrderBy(x => x.MinCostToStart.Value).ToList();
                CalcNode calcNode = routingstate.PrioQueue.First();
                routingstate.PrioQueue.Remove(calcNode);

                foreach (var cnn in calcNode.Destinations.OrderBy(x => x.Cost))
                {
                    var childNode = routingstate.GetNode(cnn.Destination.Name);
                    if (childNode == null) { childNode = new CalcNode(cnn.Destination); }
                    

                    if (routingstate.Visited.Contains(childNode))
                        continue;
                    if (childNode.MinCostToStart == null ||
                        calcNode.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = calcNode.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = calcNode;
                        if (!routingstate.PrioQueue.Contains(childNode))
                            routingstate.PrioQueue.Add(childNode);
                    }
                }
                routingstate.Visited.Add(calcNode);
                if (calcNode == routingstate.End)
                    return;
            } while (routingstate.PrioQueue.Any());
        }


        private void BuildShortestPath(List<CalcNode> list, CalcNode calcNode)
        {
            if (calcNode.NearestToStart == null)
                return;
            list.Add(calcNode.NearestToStart);
            //ShortestPathCost += node.Destinations.Single(x => x.Destination == node.NearestToStart).Cost;
            BuildShortestPath(list, calcNode.NearestToStart);
        }






    }
}
