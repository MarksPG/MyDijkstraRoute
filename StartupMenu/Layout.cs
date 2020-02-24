using Calculation;
using Layout_FrameMenu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        const string NodeFilePathIn = @"C:\Windows\Temp\01_Start.json";
        const string NodeFilePathOut = @"C:\Windows\Temp\DAG_200_3_10000.json";

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
                        "Import new DAG from file",
                        "Check best route through calculation",
                        "Run iterative calculations through DAG",
                        "Run performancetest",
                        "Quit"
                    }); ;
                Console.Clear();

                if (option == "See all nodes and edges in existing DAG") SeeAll();
                else if (option == "Automatically create a new DAG") AutomaticallyCreateDAG();
                else if (option == "Manually create a new DAG") ManuallyCreateDAG();
                else if (option == "Add new node in existing DAG") AddNewNode();
                else if (option == "Redefine node in existing DAG") AddOrRedefineNode();
                else if (option == "Save existing DAG to file") SaveExistingDAG();
                else if (option == "Import new DAG from file") ImportNewDAG();
                else if (option == "Check best route through calculation") CheckBestRoute();
                else if (option == "Run iterative calculations through DAG") RunIterative();
                else if (option == "Run performancetest") RunPerformanceTest();
                else Environment.Exit(0);

                Console.WriteLine();
            }
        }

        private void RunPerformanceTest()
        {
            AutomaticallyCreateDAG();
            int numberOfNodes = Graph.Nodes.Count();
            int numberOfEdges = Graph.Nodes[0].Destinations.Length;

            Console.WriteLine("Please enter number of iterations");
            int inputIterationNumber = int.Parse(Console.ReadLine());

            var factory = new LayoutFactory(Graph);
            var router = new Router(factory);

            //FileStream filestream = new FileStream($@"C:\Windows\Temp\{inputIterationNumber}_iterations.csv", FileMode.Create);
            //var streamwriter = new StreamWriter(filestream);
            //streamwriter.AutoFlush = true;

            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < inputIterationNumber; i++)
            {
                string inputStartNode;
                string inputEndNode;
                List<string> shortestPath = new List<string>();
                string routeString = "";

                while (true)
                {
                    inputStartNode = $"N{GetRandomNumber(1, numberOfNodes / 4)}";
                    inputEndNode = $"N{GetRandomNumber(numberOfNodes / 4, numberOfNodes + 1)}";
                    if (inputStartNode != inputEndNode)
                        break;
                }

                //Console.SetOut(streamwriter);
                //Console.SetError(streamwriter);
                Stopwatch swInner = Stopwatch.StartNew();
                shortestPath = router.GetShortestPathDijkstra(inputStartNode, inputEndNode);

                //Console.WriteLine("Time taken for Routecalculation #{0} iteration: {1}ms", i + 1, sw.ElapsedMilliseconds);


                //Console.WriteLine();
                //Console.WriteLine($"Result from calculation #{i + 1}");
                //Console.WriteLine();
                //Console.WriteLine($"Number of nodes visited: {router.NodeVisits}");
                //Console.WriteLine();
                //Console.WriteLine($"Shortest path Cost: {router.ShortestPathCost}");
                //Console.WriteLine();


                foreach (var item in shortestPath)
                {
                    routeString += item + "-";
                    //Console.WriteLine($"{item}");
                }
                string choppedRouteString = routeString.Remove(routeString.Length - 1, 1);
                log.Debug($"Routecalculation #{i + 1}\nTime taken for iteration: {swInner.ElapsedMilliseconds}ms\nShortest path: {choppedRouteString}\nNumber of visited Nodes: {router.NodeVisits}\nShortestPathCost: {router.ShortestPathCost}\n");
                swInner.Stop();

            }
            //Console.WriteLine(routeString);
            log.Info($"Number of Nodes in Graph: {numberOfNodes}\nNumber of edges per Node: {numberOfEdges}\nTotal time for all {inputIterationNumber} iterations(ms): {sw.ElapsedMilliseconds}\n");

            //Console.WriteLine("Time taken until all iterations done saved: {0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine();
            Console.WriteLine("Test done!");
            Console.WriteLine();

            SaveExistingDAG();
            //streamwriter.AutoFlush = false;
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

                //FileStream filestream = new FileStream($@"C:\Windows\Temp\{inputIterationNumber}_iterations.csv", FileMode.Create);
                //var streamwriter = new StreamWriter(filestream);
                //streamwriter.AutoFlush = true;


                for (int i = 0; i < inputIterationNumber; i++)
                {

                    //Console.SetOut(streamwriter);
                    //Console.SetError(streamwriter);

                    List<string> shortestPath = router.GetShortestPathDijkstra(inputStartNode, inputEndNode);
                    Console.WriteLine();
                    Console.WriteLine($"Result from calculation #{i + 1}");
                    Console.WriteLine();
                    Console.WriteLine($"Number of nodes visited: {router.NodeVisits}");
                    Console.WriteLine();
                    Console.WriteLine($"Shortest path Cost: {router.ShortestPathCost}");
                    Console.WriteLine();

                    foreach (var item in shortestPath)
                    {
                        Console.WriteLine($"{item}");
                    }

                    IncreaseEdgeByIncrement(shortestPath);
                }
                //streamwriter.AutoFlush = false;
                SaveExistingDAG();
                //streamwriter.Dispose();
                //filestream.Dispose();

            }
        }

        private void IncreaseEdgeByIncrement(List<string> shortestPath)
        {
            List<Node> resultRoute = new List<Node>();

            foreach (string nodeName in shortestPath)
            {
                Node node = Graph.Nodes.First(x => x.Name == nodeName);
                resultRoute.Add(node);
            }

            for (int i = 0; i < resultRoute.Count - 1; i++)
            {
                Edge actualEdge = resultRoute[i].Destinations.FirstOrDefault(x => x.Destination.Name == resultRoute[i + 1].Name);
                actualEdge.Cost++;
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

            Console.WriteLine("Enter how many edges/node: ");
            string edgeInput = Console.ReadLine();
            int numberOfEdges = Int32.Parse(edgeInput);

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

                //int numberOfEdges = GetRandomNumber(2, 5);
                //int numberOfEdges = 3;

                for (int i = 0; i < numberOfEdges; i++)
                {
                    Edge edge = new Edge() { };

                    while (true)
                    {
                        int connectedNodeNumber = GetRandomNumber(1, numberOfNodes + 1);
                        edge.Destination = userNodes.FirstOrDefault(x => x.Name == $"N{connectedNodeNumber}");
                        if (item != edge.Destination && destinations.All(x => x.Destination != edge.Destination))
                            break;
                    }

                    double costInput = GetRandomNumber(1, 11);
                    edge.Cost = costInput;
                    destinations.Add(edge);
                }
                item.Destinations = destinations.ToArray();
            }

        }

        private void CheckBestRoute()
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



                var factory = new LayoutFactory(Graph);
                var router = new Router(factory);
                List<string> shortestPath = router.GetShortestPathDijkstra(inputStartNode, inputEndNode);

                Console.WriteLine($"Number of nodes visited: {router.NodeVisits}");
                Console.WriteLine();
                Console.WriteLine($"Shortest path Cost: {router.ShortestPathCost}");
                Console.WriteLine();
                foreach (var item in shortestPath)
                {
                    Console.WriteLine($"{item}");
                }
            }
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
                        edge.Destination = userNodes.FirstOrDefault(x => x.Name == nodeInput);
                        Console.WriteLine($"Specify cost for the connection between {item.Name} and {nodeInput}: ");
                        costInput = Console.ReadLine();
                        edge.Cost = Int32.Parse(costInput);

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
                            edge.Cost = Int32.Parse(costInput);

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
                Console.WriteLine($"Connection to {edge.Destination.Name} at Cost of {edge.Cost.ToString()}");

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
                    edge.Cost = Int32.Parse(costInput);

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
                Console.WriteLine($"Connection to {edge.Destination.Name} at Cost of {edge.Cost.ToString()}");
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
                    actualEdge.Cost = Int32.Parse(costInput);
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
                    string edgeCost = edge.Cost.ToString();
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
                    edge.Cost = Int32.Parse(parts[0]);
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

