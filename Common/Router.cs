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

        public int NodeVisits { get; private set; }

        public double? ShortestPathCost { get; private set; }

        public List<CalcNode> CalcNodes { get; set; } = new List<CalcNode>();


        public Router(ILayoutFactory lf)
        {
            _layoutFactory = lf;

            Graph = lf.GetLayout();

            if(Graph == null)
            {
                throw new NullReferenceException("A graph does not exist, or has not been properly instantiated");
            }
        }


        public List<RouterResult> GetShortestPathDijkstra(string startNode, string endNode)
        {
            if(startNode == endNode)
            {
                throw new ArgumentException("Router can not process route calculation for endNode similar to startNode!");
            }
            else if (startNode == null || endNode == null)
            {
                throw new NullReferenceException("Either startNode or endNode is null!");
            }

            Node StartNode = Graph.Nodes.FirstOrDefault(x => x.Name == startNode);
            Node EndNode = Graph.Nodes.FirstOrDefault(x => x.Name == endNode);

            RoutingState routingState = new RoutingState(StartNode, EndNode);

            DijkstraSearch(routingState);
            List<RouterResult> result = new List<RouterResult>();
            var shortestPath = new List<CalcNode>();

            shortestPath.Add(routingState.End);
            ShortestPathCost = SetShortestPathCost(routingState);
            BuildShortestPath(shortestPath, routingState.End);
            
            shortestPath.Reverse();
            if (!shortestPath.Any(x => x.Name == endNode) || !shortestPath.Any(x => x.Name == startNode))
            {
                throw new ArgumentException("Router can not process route calculation for Nodes that aren't connected");
            }
            int i = 0;
            while (i < shortestPath.Count - 1)
            {
                RouterResult routerResult = new RouterResult()
                {
                    NodeName = shortestPath[i].Name,
                    ShortestPathValue = shortestPath[i].Destinations.FirstOrDefault(x => x.Destination.Name == shortestPath[i + 1].Name).Cost
                };
                result.Add(routerResult);
                i++;
            }
            RouterResult routerResult2 = new RouterResult()
            {
                NodeName = shortestPath[i].Name,
                ShortestPathValue = 0
            };
            result.Add(routerResult2);


            return result;
        }

        private double? SetShortestPathCost(RoutingState routingState)
        {
            try
            {
                double? value = routingState.End.MinCostToStart;
                return value;
            }
            catch
            {
                throw new NullReferenceException("ShortestPath is only one Node. StartPoint and EndPoint cannot be identical!");
            }
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
            
            //ShortestPathCost += calcNode.Destinations.Single(x => x.Destination.Name == calcNode.NearestToStart.Name).Cost;
            BuildShortestPath(list, calcNode.NearestToStart);
        }

    }
}
