using Calculation;
using Layout_FrameMenu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;



[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace StartupMenu
{

    public class Layout
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Graph Graph { get; set; }

        public static DateTime Today { get; }

        private static readonly Random getrandom = new Random();

        const string NodeFilePathIn = @"C:\Windows\Temp\GrundGraf.json";
        const string NodeFilePathOut = @"C:\Windows\Temp\RGLayout_out.json";

        public void Run()
        {
            while (true)
            {
                string option = ShowMenu("What do you want to do?", new[] {
                        "See all nodes and edges in existing DAG",
                        "Automatically create a new DAG",
                        "Manually create a new DAG",
                        "Add new node in existing DAG",
                        "Redefine node in existing DAG",
                        "Save existing DAG to file",
                        "Save existing DAG to database",
                        "Import new DAG from file",
                        "Find cost for a specific Edge in graph",
                        "Automatic DAG with variable edge Cost",
                        "Check best route through calculation",
                        "Check best route with extended Cost",
                        "Run iterative calculations through DAG",
                        "Run performancetest",
                        
                        "Find all Nodes that connect to a specific Node in DAG",
                        "Quit"
                    }); ;
                Console.Clear();

                if (option == "See all nodes and edges in existing DAG") SeeAll();
                else if (option == "Automatically create a new DAG") AutomaticallyCreateDAG();
                else if (option == "Manually create a new DAG") ManuallyCreateDAG();
                else if (option == "Add new node in existing DAG") AddNewNode();
                else if (option == "Redefine node in existing DAG") AddOrRedefineNode();
                else if (option == "Save existing DAG to file") SaveExistingDAG();
                else if (option == "Save existing DAG to database") SaveToDatabase();
                else if (option == "Import new DAG from file") ImportNewDAG();
                else if (option == "Find cost for a specific Edge in graph") FindCostforEdge();
                else if (option == "Automatic DAG with variable edge Cost") CreateDAGWithVariableEdge();
                else if (option == "Check best route through calculation") CheckBestRoute();
                else if (option == "Check best route with extended Cost") CheckWithVariableCost();
                else if (option == "Run iterative calculations through DAG") RunIterative();
                else if (option == "Run performancetest") RunPerformanceTest();
                else if (option == "Find all Nodes that connect to a specific Node in DAG") CheckIncomingNodes();
                else Environment.Exit(0);

                Console.WriteLine();
            }
        }

        private void CheckWithVariableCost()
        {
            if (Graph.Nodes == null || Graph.Nodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }
            else
            {
                UpdateAllEdgeCosts();

            }
        }
        
        private void UpdateAllEdgeCosts()
        {
            List<Node> userNodes = Graph.Nodes;

            foreach (var item in userNodes)
            {
                var c = GetCostsInDatabase(item.Name, item.Destinations.Select(d => d.Destination.Name).ToArray());
                var costList = c.ToList();

                for (int i = 0; i < c.Count(); i++)
                {
                    int costSum = costList[i].Distance + costList[i].DownTime + costList[i].EmergencyStop + costList[i].Occupancy + costList[i].Speed;

                    item.Destinations[i].Cost.InitialValue = costSum;
                }
            }
        }

        private void UpdateAllEdgeCostsTotalLoading()
        {
            List<Node> userNodes = Graph.Nodes;
            var edges = GetCostsInDatabaseTotal();


            foreach (Node node in userNodes)
            {
                foreach (var item in node.Destinations)
                {
                    List<CostData> hupps = edges.FirstOrDefault(p => p.FromPosition == node.Name && p.ToPosition == item.Destination.Name).Costs.ToList();

                    int sum = hupps[0].Distance + hupps[0].DownTime + hupps[0].EmergencyStop + hupps[0].Occupancy + hupps[0].Speed;
                    item.Cost.InitialValue = sum;
                }
                
            }
            

        }

        private void UpdateAllEdgeCostsTotalLoading2()
        {
            List<Node> userNodes = Graph.Nodes;
            using (var conn = new System.Data.SqlClient.SqlConnection("Server=.\\SQLEXPRESS;Database=LIA2Routing_1;Integrated Security=True;"))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "select p.fromposition, p.toposition, c.Occupancy, c.EmergencyStop, c.Speed, c.Distance, c.DownTime from positions p join costdatas c on p.positionid = c.positionid";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //var edge = userNodes.FirstOrDefault(n => n.Name == reader.GetString(0))?.Destinations.FirstOrDefault(d => d.Destination.Name == reader.GetString(1));
                        //int sum = reader.GetInt32(2) + reader.GetInt32(3) + reader.GetInt32(4) + reader.GetInt32(5) + reader.GetInt32(6);
                        //edge.Cost.InitialValue = sum;

                        foreach (Node node in userNodes)
                        {
                            if(node.Name == reader.GetString(0))
                            {
                                for (int i = 0; i < node.Destinations.Length; i++)
                                {
                                    if(node.Destinations[i].Destination.Name == reader.GetString(1))
                                    {
                                        int sum = reader.GetInt32(2) + reader.GetInt32(3) + reader.GetInt32(4) + reader.GetInt32(5) + reader.GetInt32(6);
                                        node.Destinations[i].Cost.InitialValue = sum;
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }
        }

        private void FindCostforEdge()
        {


            if (Graph.Nodes == null || Graph.Nodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }
            else
            {
                List<Node> userNodes = Graph.Nodes;
                Console.Clear();
                Console.WriteLine("Please, specify Node for calculation");
                string nodeInput = Console.ReadLine();
                Node selectedNode = userNodes.FirstOrDefault(x => x.Name == nodeInput);

                Console.WriteLine($"The selected node {selectedNode.Name} has {selectedNode.Destinations.Length} connection/s");
                Console.WriteLine();

                var c = GetCostsInDatabase(nodeInput, selectedNode.Destinations.Select(d => d.Destination.Name).ToArray());
                var costList = c.ToList();

                for (int i = 0; i < c.Count(); i++)
                {
                    string destination = selectedNode.Destinations[i].Destination.Name;
                    int costSum = costList[i].Distance + costList[i].DownTime + costList[i].EmergencyStop + costList[i].Occupancy + costList[i].Speed;



                    Console.WriteLine($"Connection to Node {destination} at a total cost of {costSum}");
                }
                
                
                

            }

        }

        private IEnumerable<CostData> GetCostsInDatabase(string nodeInput, string[] destination)
        {
            CostData[] costs;

            using (var db = new LayoutContext())
            {
                costs = db.CostData.Where(cd => cd.Position.FromPosition == nodeInput && destination.Contains(cd.Position.ToPosition)).AsNoTracking().ToArray();
                //db.Positions.FirstOrDefault(x => x.FromPosition == nodeInput && x.ToPosition == destination).Costs;
            }
            return costs;
        }

        private IEnumerable<Position> GetCostsInDatabaseTotal()
        {
            Position[] all;

            using (var db = new LayoutContext())
            {
                all = db.Positions.Include(p => p.Costs).AsNoTracking().ToArray();
                //db.Positions.FirstOrDefault(x => x.FromPosition == nodeInput && x.ToPosition == destination).Costs;
            }
            return all;
        }

        private void SaveToDatabase()
        {
            if (Graph.Nodes == null || Graph.Nodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }
            else
            {
                int counter = 0;
                List<Node> userNodes = Graph.Nodes;

                using (var db = new LayoutContext())
                {
                    for (int i = 0; i < userNodes.Count; i++)
                    {
                        for (int k = 0; k < userNodes[i].Destinations.Length; k++)
                        {
                            List<CostData> costDatas = new List<CostData>();
                            CostData costData = new CostData();
                            costDatas.Add(costData);


                            Position position = new Position()
                            {
                                FromPosition = userNodes[i].Name,
                                ToPosition = userNodes[i].Destinations[k].Destination.Name,
                                Costs = costDatas
                            };
                            //position.Costs[0].Distance = (userNodes[i].Destinations[k].Cost.InitialValue).GetValueOrDefault();
                            db.Positions.Add(position);
                            counter++;
                        }
                    }
                    db.SaveChanges();
                    Console.WriteLine($"Number of saved positions to database: {counter}");
                }
                    
            }

        }

        private void CreateDAGWithVariableEdge()
        {
            Graph graph = new Graph()
            {
                Nodes = new List<Node>()
            };

            List<Node> userNodes = new List<Node>();

            Console.WriteLine("Enter how many nodes: ");
            string nodeInput = Console.ReadLine();
            int numberOfNodes = Int32.Parse(nodeInput);

            for (int i = 1; i < numberOfNodes + 1; i++)
            {
                Node node = new Node() { };
                string nameString = $"N{i}";
                node.Name = nameString;
                userNodes.Add(node);
            }
            graph.Nodes = userNodes;
            Graph = graph;

            foreach (Node item in userNodes)
            {
                List<Edge> destinations = new List<Edge>();

                int numberOfEdges = GetRandomNumber(2, 5);
                

                for (int i = 0; i < numberOfEdges; i++)
                {
                    Edge edge = new Edge() { };
                    Cost cost = new Cost() { };

                    while (true)
                    {
                        int connectedNodeNumber = GetRandomNumber(1, numberOfNodes + 1);
                        edge.Destination = userNodes.FirstOrDefault(x => x.Name == $"N{connectedNodeNumber}");
                        if (item != edge.Destination && destinations.All(x => x.Destination != edge.Destination))
                            break;
                    }

                    edge.Cost = cost;
                    destinations.Add(edge);
                }
                item.Destinations = destinations.ToArray();
            }

        }

        private void CheckIncomingNodes()
        {
            if (Graph.Nodes == null || Graph.Nodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }
            else

            {
                Console.WriteLine("Please enter what Node to check");
                string inputDestinationNode = Console.ReadLine();

                foreach (Node node in Graph.Nodes)
                {
                    if(node.Destinations.Any(x => x.Destination.Name == inputDestinationNode))
                    {
                        Console.WriteLine(node.Name);
                    }
                    
                }
                

                
            }

        }

        private void RunPerformanceTest()
        {
            //AutomaticallyCreateDAG();
            int numberOfNodes = Graph.Nodes.Count();
            //int numberOfEdges = Graph.Nodes[0].Destinations.Length;

            Console.WriteLine("Please enter number of iterations");
            int inputIterationNumber = int.Parse(Console.ReadLine());

            var factory = new LayoutFactory(Graph);
            var router = new Router(factory);
                        
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < inputIterationNumber; i++)
            {
                string inputStartNode;
                string inputEndNode;
                List<RouterResult> shortestPath = new List<RouterResult>();
                string routeString = "";
                string pathCostString = "";
                string inner = "";

                while (true)
                {
                    inputStartNode = $"N{GetRandomNumber(1, numberOfNodes / 4)}";
                    inputEndNode = $"N{GetRandomNumber(numberOfNodes / 4, numberOfNodes + 1)}";
                    if (inputStartNode != inputEndNode)
                        break;
                }

                Stopwatch swInner = Stopwatch.StartNew();

                try
                {
                    UpdateAllEdgeCostsTotalLoading2();
                    shortestPath = router.GetShortestPathDijkstra(inputStartNode, inputEndNode);
                }
                catch (Exception ex)
                {
                    inner = ex.Message;
                    Console.WriteLine(inner);
                    Console.WriteLine(ex.StackTrace);
                }

                if (shortestPath.Count > 0)
                {
                    foreach (var item in shortestPath)
                    {
                        routeString += item.NodeName + "-";
                        pathCostString += item.ShortestPathValue + "-";
                    }
                    string choppedRouteString = routeString.Remove(routeString.Length - 1, 1);
                    string choppedpathCostString = pathCostString.Remove(pathCostString.Length - 1, 1);
                    log.Debug($"Routecalculation #{i + 1}\nTime taken for iteration: {swInner.ElapsedMilliseconds}ms\n" +
                        $"StartNode = {inputStartNode}\nEndNode = {inputEndNode}\nShortest path: {choppedRouteString}\n" +
                        $"PathCost: {choppedpathCostString}\nNumber of visited Nodes: {router.NodeVisits}\n" +
                        $"ShortestPathCost: {router.ShortestPathCost}\n");
                }
                else
                {
                    log.Debug($"{inner}");
                }
                swInner.Stop();
            }
            
            log.Info($"Number of Nodes in Graph: {numberOfNodes}\nTotal time for all {inputIterationNumber} iterations(ms): {sw.ElapsedMilliseconds}\n");

            Console.WriteLine();
            Console.WriteLine("Test done!");
            Console.WriteLine();

            SaveExistingDAG();
            
            sw.Stop();
        }

        private void RunIterative()
        {
            if (Graph.Nodes == null || Graph.Nodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }
            else
            {
                Console.WriteLine("Please enter a startNode");
                string inputStartNode = Console.ReadLine();
                Console.WriteLine("Please enter an endNode");
                string inputEndNode = Console.ReadLine();
                Console.WriteLine("Please enter number of iterations");
                int inputIterationNumber = int.Parse(Console.ReadLine());

                var factory = new LayoutFactory(Graph);
                var router = new Router(factory);


                for (int i = 0; i < inputIterationNumber; i++)
                {
                    List<RouterResult> shortestPath = router.GetShortestPathDijkstra(inputStartNode, inputEndNode);
                    Console.WriteLine();
                    Console.WriteLine($"Result from calculation #{i + 1}");
                    Console.WriteLine();
                    Console.WriteLine($"Number of nodes visited: {router.NodeVisits}");
                    Console.WriteLine();
                    Console.WriteLine($"Shortest path Cost: {router.ShortestPathCost}");
                    Console.WriteLine();

                    foreach (var item in shortestPath)
                    {
                        Console.WriteLine($"{item.NodeName}");
                    }

                    IncreaseEdgeByIncrement(shortestPath);
                }
                SaveExistingDAG();
            }
        }

        private void IncreaseEdgeByIncrement(List<RouterResult> shortestPath)
        {
            List<Node> resultRoute = new List<Node>();

            foreach (var item in shortestPath)
            {
                Node node = Graph.Nodes.First(x => x.Name == item.NodeName);
                resultRoute.Add(node);
            }

            for (int i = 0; i < resultRoute.Count - 1; i++)
            {
                Edge actualEdge = resultRoute[i].Destinations.FirstOrDefault(x => x.Destination.Name == resultRoute[i + 1].Name);
                actualEdge.Cost.InitialValue++;
            }
        }

        private void AutomaticallyCreateDAG()
        {
            //Console.WriteLine("Please, define a name for your new DAG:");
            //string dagNameInput = Console.ReadLine();

            Graph graph = new Graph()
            {
                Nodes = new List<Node>()
            };

            //graph.Name = dagNameInput;

            List<Node> userNodes = new List<Node>();

            Console.WriteLine("Enter how many nodes: ");
            string nodeInput = Console.ReadLine();
            int numberOfNodes = Int32.Parse(nodeInput);

            //Console.WriteLine("Enter how many edges/node: ");
            //string edgeInput = Console.ReadLine();
            //int numberOfEdges = Int32.Parse(edgeInput);

            for (int i = 1; i < numberOfNodes + 1; i++)
            {
                Node node = new Node() { };
                string nameString = $"N{i}";
                node.Name = nameString;
                userNodes.Add(node);
            }
            graph.Nodes = userNodes;
            Graph = graph;

            foreach (Node item in userNodes)
            {
                List<Edge> destinations = new List<Edge>();

                int numberOfEdges = GetRandomNumber(2, 5);
                //int numberOfEdges = 3;

                for (int i = 0; i < numberOfEdges; i++)
                {
                    Edge edge = new Edge() { };
                    Cost cost = new Cost() { };

                    while (true)
                    {
                        int connectedNodeNumber = GetRandomNumber(1, numberOfNodes + 1);
                        edge.Destination = userNodes.FirstOrDefault(x => x.Name == $"N{connectedNodeNumber}");
                        if (item != edge.Destination && destinations.All(x => x.Destination != edge.Destination))
                            break;
                    }

                    int costInput = GetRandomNumber(1, 11);
                    cost.InitialValue = costInput;
                    edge.Cost = cost;
                    destinations.Add(edge);
                }
                item.Destinations = destinations.ToArray();
            }

        }

        private void CheckBestRoute()
        {
            int numberOfNodes = Graph.Nodes.Count();

            string inputStartNode;
            string inputEndNode;
            List<RouterResult> shortestPath = new List<RouterResult>();
            string routeString = "";
            string pathCostString = "";
            string inner = "";

            Stopwatch sw = Stopwatch.StartNew();

            if (Graph.Nodes != null || Graph.Nodes.Count() != 0)
            {
                
                Console.WriteLine("Please enter a startNode");
                inputStartNode = Console.ReadLine();
                Console.WriteLine("Please enter an endNode");
                inputEndNode = Console.ReadLine();
                UpdateAllEdgeCosts();

                var factory = new LayoutFactory(Graph);
                var router = new Router(factory);

                Stopwatch swInner = Stopwatch.StartNew();

                try
                {
                    shortestPath = router.GetShortestPathDijkstra(inputStartNode, inputEndNode);
                }
                catch (Exception ex)
                {
                    inner = ex.Message;
                    Console.WriteLine(inner);
                    Console.WriteLine(ex.StackTrace);
                }

                if (shortestPath.Count > 0)
                {
                    foreach (var item in shortestPath)
                    {
                        routeString += item.NodeName + "-";
                        pathCostString += item.ShortestPathValue + "-";
                    }
                    string choppedRouteString = routeString.Remove(routeString.Length - 1, 1);
                    string choppedpathCostString = pathCostString.Remove(pathCostString.Length - 1, 1);
                    Console.WriteLine($"Routecalculation result\nTime taken for iteration: {swInner.ElapsedMilliseconds}ms\n" +
                        $"StartNode = {inputStartNode}\nEndNode = {inputEndNode}\nShortest path: {choppedRouteString}\n" +
                        $"PathCost: {choppedpathCostString}\nNumber of visited Nodes: {router.NodeVisits}\n" +
                        $"ShortestPathCost: {router.ShortestPathCost}\n");
                }
                else
                {
                    log.Debug($"{inner}");
                }
                swInner.Stop();

                Console.WriteLine($"Number of Nodes in Graph: {numberOfNodes}\nTotal time for iteration(ms): {sw.ElapsedMilliseconds}\n");

                Console.WriteLine();
                Console.WriteLine("Test done!");
                Console.WriteLine();




                Console.WriteLine($"Number of nodes visited: {router.NodeVisits}");
                Console.WriteLine();
                Console.WriteLine($"Shortest path Cost: {router.ShortestPathCost}");
                Console.WriteLine();
                foreach (var item in shortestPath)
                {
                    Console.WriteLine($"{item.NodeName}");
                }
            }

            else
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }

            sw.Stop();
        }

        private void SeeAll()
        {
            if (Graph == null || Graph.Nodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG.");
            }
            else
            {
                Console.WriteLine("Existing nodes in DAG:");
                foreach (var item in Graph.Nodes)
                {
                    Console.WriteLine($"{item.Name} - Number of edges: {item.Destinations.Length}");
                }
            }
        }

        private void ManuallyCreateDAG()
        {
            Graph graph = new Graph()
            {
                Nodes = new List<Node>()
            };

            Console.WriteLine("Please, define a name for your new DAG:");
            string dagNameInput = Console.ReadLine();
            graph.Name = dagNameInput;

            List<Node> userNodes = new List<Node>();

            Console.WriteLine("Enter how many nodes: ");
            string input = Console.ReadLine();
            int numberOfNodes = Int32.Parse(input);

            for (int i = 0; i < numberOfNodes; i++)
            {
                Node node = new Node() { };
                Console.Write($"Name your node #{i + 1}: ");

                string inputString = Console.ReadLine();
                node.Name = inputString;
                userNodes.Add(node);
            }
            graph.Nodes = userNodes;
            Graph = graph;


            foreach (Node item in userNodes)
            {
                Edge[] destinations = new Edge[0];

                while (true)
                {
                    string nodeInput;
                    string costInput;
                    Console.WriteLine($"Specify what node(s) {item.Name} connects to: ");
                    nodeInput = Console.ReadLine();

                    if (nodeInput != string.Empty && AllDefinedNodes().Contains(nodeInput))
                    {
                        Edge edge = new Edge() { };
                        Cost cost = new Cost() { };
                        edge.Destination = userNodes.FirstOrDefault(x => x.Name == nodeInput);
                        Console.WriteLine($"Specify cost for the connection between {item.Name} and {nodeInput}: ");
                        costInput = Console.ReadLine();
                        cost.InitialValue = Int32.Parse(costInput);
                        edge.Cost = cost;

                        destinations = destinations.Concat(new Edge[] { edge }).ToArray();
                    }
                    else if (nodeInput != string.Empty && !AllDefinedNodes().Contains(nodeInput))
                    {
                        Console.WriteLine("Specified Node does not exist or nodename misspelled. Please check again");
                    }
                    else
                    {
                        break;
                    }
                }
                item.Destinations = destinations;
            }
            graph.Nodes = userNodes;
            Graph = graph;
        }

        private void AddNewNode()
        {
            try
            {
                List<Node> userNodes = Graph.Nodes;
                Console.Write("Name your node: ");
                string nodeInput = Console.ReadLine();

                if (nodeInput != string.Empty && !AllDefinedNodes().Contains(nodeInput))
                {
                    Node node = new Node() { };
                    Edge[] destinations = new Edge[0];
                    node.Name = nodeInput;
                    userNodes.Add(node);

                    while (true)
                    {
                        string costInput;
                        Console.WriteLine($"Specify what node(s) {node.Name} connects to: ");
                        string nodeToInput = Console.ReadLine();

                        if (nodeToInput != string.Empty && AllDefinedNodes().Contains(nodeToInput))
                        {
                            Edge edge = new Edge() { };
                            edge.Destination = userNodes.FirstOrDefault(x => x.Name == nodeToInput);
                            Console.WriteLine($"Specify cost for the connection between {node.Name} and {nodeToInput}: ");
                            costInput = Console.ReadLine();
                            edge.Cost.InitialValue = Int32.Parse(costInput);

                            destinations = destinations.Concat(new Edge[] { edge }).ToArray();

                        }
                        else if (nodeToInput != string.Empty && !AllDefinedNodes().Contains(nodeToInput))
                        {
                            Console.WriteLine("Specified Node does not exist or nodename misspelled. Please check again");
                        }
                        else
                        {
                            break;
                        }
                        node.Destinations = destinations;
                    }
                    Graph.Nodes = userNodes;
                }
                else if (AllDefinedNodes().Contains(nodeInput))
                {
                    Console.WriteLine("A node with that name already exists. Please, check that node or define a node with a new name.");
                }
                else { };
            }
            catch
            {
                Console.WriteLine("There is currently no existing DAG. Please, create a new DAG manually or import one from file");
            }

        }

        private void AddOrRedefineNode()
        {
            List<Node> userNodes = Graph.Nodes;
            string nodeRedefinement = ShowMenu("Select actual node", AllDefinedNodes().ToArray());

            Node selectedNode = userNodes.FirstOrDefault(x => x.Name == nodeRedefinement);

            Console.WriteLine();
            Console.WriteLine($"The selected node {selectedNode.Name} has {selectedNode.Destinations.Length} connection/s");
            Console.WriteLine();
            foreach (Edge edge in selectedNode.Destinations)
            {
                Console.WriteLine($"Connection to {edge.Destination.Name} at Cost of {edge.Cost.InitialValue.ToString()}");

            }
            string selectedAction = ShowMenu("Select what you want to do.", new[] {
                        "Change value for cost of Edge",
                        $"Add new edge for {selectedNode.Name}",
                        "Return to main menu"});
            if (selectedAction == "Change value for cost of Edge") ChangeEdgeValue(selectedNode);
            else if (selectedAction == $"Add new edge for {selectedNode.Name}") AddNewEdge(selectedNode);
            else { };

            Console.Clear();
        }

        private void AddNewEdge(Node selectedNode)
        {
            Edge[] destinations = selectedNode.Destinations;

            List<Node> userNodes = Graph.Nodes;

            while (true)
            {
                string nodeInput;
                string costInput;
                Console.WriteLine($"Specify what node(s) {selectedNode.Name} connects to: ");
                nodeInput = Console.ReadLine();

                if (nodeInput != string.Empty && AllDefinedNodes().Contains(nodeInput))
                {
                    Edge edge = new Edge() { };
                    edge.Destination = userNodes.FirstOrDefault(x => x.Name == nodeInput);
                    Console.WriteLine($"Specify cost for the connection between {selectedNode.Name} and {nodeInput}: ");
                    costInput = Console.ReadLine();
                    edge.Cost.InitialValue = Int32.Parse(costInput);

                    destinations = destinations.Concat(new Edge[] { edge }).ToArray();
                }
                else if (nodeInput != string.Empty && !AllDefinedNodes().Contains(nodeInput))
                {
                    Console.WriteLine("Specified Node does not exist or nodename misspelled. Please check again");
                }
                else
                {
                    break;
                }

            }
            selectedNode.Destinations = destinations;
        }

        private void ChangeEdgeValue(Node selectedNode)
        {
            Edge[] destinations = selectedNode.Destinations;
            List<Node> userNodes = Graph.Nodes;

            foreach (Edge edge in destinations)
            {
                Console.WriteLine($"Connection to {edge.Destination.Name} at Cost of {edge.Cost.InitialValue.ToString()}");
            }

            while (true)
            {
                Console.WriteLine("Type what destination to affect:");
                string nodeInput = Console.ReadLine();

                if (nodeInput != string.Empty && AllDefinedNodes().Contains(nodeInput))
                {
                    Edge actualEdge = destinations.FirstOrDefault(x => x.Destination.Name == nodeInput);

                    Console.WriteLine($"Specify new cost for the connection between {selectedNode.Name} and {nodeInput}: ");
                    string costInput = Console.ReadLine();
                    actualEdge.Cost.InitialValue = Int32.Parse(costInput);
                }
                else if (nodeInput != string.Empty && !AllDefinedNodes().Contains(nodeInput))
                {
                    Console.WriteLine("Specified Node does not exist or nodename misspelled. Please check again");
                }
                else
                {
                    break;
                }

            }

        }

        private void SaveExistingDAG()
        {
            Dictionary<string, List<string>> userNodesToSave = new Dictionary<string, List<string>>();
            Dictionary<string, Dictionary<string, List<string>>> userGraphToSave = new Dictionary<string, Dictionary<string, List<string>>>();

            Console.WriteLine("Please, set a name to the existing DAG: ");
            Graph.Name = Console.ReadLine();

            foreach (Node item in Graph.Nodes)
            {
                string nodeName = item.Name.ToString();
                List<string> destinations = new List<string>();
                foreach (Edge edge in item.Destinations)
                {
                    string edgeCost = edge.Cost.InitialValue.ToString();
                    string edgeDestination = edge.Destination.Name.ToString();
                    destinations.Add($"{edgeCost},{edgeDestination}");
                }
                userNodesToSave.Add(nodeName, destinations);
            }
            userGraphToSave.Add(Graph.Name, userNodesToSave);

            using (StreamWriter file = File.CreateText(NodeFilePathOut))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, userGraphToSave);
            }
        }

        private void ImportNewDAG()
        {
            if (Graph == null || Graph.Nodes.Count() == 0)
            {
                GetImportedUserNodes();
            }
            else
            {
                Graph.Nodes.Clear();
                GetImportedUserNodes();
            }
        }

        private void GetImportedUserNodes()
        {
            Graph graph = new Graph()
            {
                Nodes = new List<Node>()
            };

            List<Node> userNodes = new List<Node>();
            Dictionary<string, List<string>> importedNodes = new Dictionary<string, List<string>>();
            Dictionary<string, Dictionary<string, List<string>>> importedGraph = new Dictionary<string, Dictionary<string, List<string>>>();

            using (StreamReader file = File.OpenText(NodeFilePathIn))
            {
                JsonSerializer serializer = new JsonSerializer();
                importedGraph = (Dictionary<string, Dictionary<string, List<string>>>)serializer.Deserialize(file, typeof(Dictionary<string, Dictionary<string, List<string>>>));
            }

            foreach (KeyValuePair<string, Dictionary<string, List<string>>> pair in importedGraph)
            {
                graph.Name = pair.Key;
                importedNodes = pair.Value;
            }

            foreach (KeyValuePair<string, List<string>> pair in importedNodes)
            {
                Node node = new Node
                {
                    Name = pair.Key
                };
                userNodes.Add(node);
            }

            foreach (Node node in userNodes)
            {
                Edge[] destinations = new Edge[0];

                var edgeDestinations = importedNodes[node.Name];
                foreach (var item in edgeDestinations)
                {
                    string[] parts = item.Split(',');
                    Edge edge = new Edge() { };
                    Cost cost = new Cost() { };
                    cost.InitialValue = Int32.Parse(parts[0]);
                    edge.Cost = cost;
                    edge.Destination = userNodes.FirstOrDefault(x => x.Name == parts[1]);
                    destinations = destinations.Concat(new Edge[] { edge }).ToArray();
                }
                node.Destinations = destinations;
            }
            graph.Nodes = userNodes;
            Graph = graph;
        }

        private List<string> AllDefinedNodes()
        {
            List<string> allDefinedNodes = new List<string>();

            try
            {
                foreach (Node node in Graph.Nodes)
                {
                    allDefinedNodes.Add(node.Name);
                }
            }
            catch
            {
                Console.WriteLine("There is currently no existing DAG.");
            }

            return allDefinedNodes;
        }

        public string ShowMenu(string prompt, string[] options)
        {
            Console.WriteLine(prompt);

            int selected = 0;

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                // If this is not the first iteration, move the cursor to the first line of the menu.
                if (key != null)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.CursorTop - options.Length;
                }

                // Print all the options, highlighting the selected one.
                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("- " + option);
                    Console.ResetColor();
                }

                // Read another key and adjust the selected value before looping to repeat all of this.
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Length - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }
            }
            // Reset the cursor and return the selected option.
            Console.CursorVisible = true;
            return options[selected];
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }
    }
}

