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

        public int? ShortestPathCost { get; private set; }

        public List<CalcNode> CalcNodes { get; set; } = new List<CalcNode>();


        public Router(ILayoutFactory lf)
        {
            _layoutFactory = lf;

            Graph = lf.GetLayout();

            if (Graph.Nodes.Count == 0)
            {
                throw new ArgumentException("A graph does not exist, or has not been properly instantiated");
            }
        }


        public List<RouterResult> GetShortestPathDijkstra(string startNode, string endNode)
        {
            UpdateAllEdgeCostsFromPositionsDictionary();

            if (startNode == endNode)
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
                    ShortestPathValues = shortestPath[i].Destinations.FirstOrDefault(x => x.Destination.Name == shortestPath[i + 1].Name).AllCosts
                };
                result.Add(routerResult);
                i++;
            }
            RouterResult routerResult2 = new RouterResult()
            {
                NodeName = shortestPath[i].Name,
                ShortestPathValues = new List<Cost>(new Cost[] { new Cost() { CostName = "Endpoint", Value = 0 } })
            };
            result.Add(routerResult2);


            return result;
        }

        private int? SetShortestPathCost(RoutingState routingState)
        {
            try
            {
                int? value = routingState.End.MinCostToStart;
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
                routingstate.PrioQueue = routingstate.PrioQueue.OrderBy(x => x.MinCostToStart).ToList();
                CalcNode calcNode = routingstate.PrioQueue.First();
                routingstate.PrioQueue.Remove(calcNode);

                foreach(var cnn in calcNode.Destinations.OrderBy(x => x.AllCosts.Sum(v => v.Value)))
                //foreach (var cnn in calcNode.Destinations.OrderBy(x => x.AllCosts[0].Value))
                {
                    var childNode = routingstate.GetNode(cnn.Destination.Name);
                    if (childNode == null) { childNode = new CalcNode(cnn.Destination); }
                    

                    if (routingstate.Visited.Contains(childNode))
                        continue;
                    if (childNode.MinCostToStart == null ||
                        calcNode.MinCostToStart + cnn.AllCosts[0].Value < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = calcNode.MinCostToStart + cnn.AllCosts.Sum(v => v.Value);
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

        private void UpdateAllEdgeCostsFromPositionsDictionary()
        {
            foreach (Node node in Graph.Nodes)
            {
                for (int i = 0; i < node.Destinations.Length; i++)
                {
                    string keyValue = $"{node.Name},{node.Destinations[i].Destination.Name}";
                    node.Destinations[i].AllCosts.Clear();
                    foreach (KeyValuePair<string, int> costs in Graph.Positions[keyValue])
                    {
                        Cost cost = new Cost()
                        {
                            CostName = costs.Key,
                            Value = costs.Value
                        };
                        node.Destinations[i].AllCosts.Add(cost);
                    }
                }
            }
        }

    }
}
