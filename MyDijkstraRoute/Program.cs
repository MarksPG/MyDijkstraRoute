using Newtonsoft.Json;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyDijkstraRoute
{

    class Program
    {
        public static List<Node> UserNodes;

        const string NodeFilePath = @"C:\Windows\Temp\myDAG.json";

        public static void Main()
        {

            while (true)
            {
                string option = ShowMenu("What do you want to do?", new[] {
                        "See all nodes and edges in existing DAG",
                        "Manually create a new DAG",
                        "Add new node in existing DAG",
                        "Redefine node in existing DAG",
                        "Save existing DAG to file",
                        "Import new DAG from file",
                        "Open map for view and calculation",
                        "Quit"
                    }); ;
                Console.Clear();

                if (option == "See all nodes and edges in existing DAG") SeeAll();
                else if (option == "Manually create a new DAG") ManuallyCreateDAG();
                else if (option == "Add new node in existing DAG") AddNewNode();
                else if (option == "Redefine node in existing DAG") AddOrRedefineNode();
                else if (option == "Save existing DAG to file") SaveExistingDAG();
                else if (option == "Import new DAG from file") ImportNewDAG();
                else if (option == "Open map for view and calculation") OpenGraphForCalc();
                else Environment.Exit(0);

                Console.WriteLine();
            }
        }

        private static void OpenGraphForCalc()
        {
            if (UserNodes == null || UserNodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG. You need to define a DAG or import a DAG from file!");
            }
            else
            {
                Graph graph = new Graph
                {
                    Nodes = UserNodes
                };

                Console.WriteLine("Please enter a startNode");
                string inputStartNode = Console.ReadLine();
                Console.WriteLine("Please enter an endNode");
                string inputEndNode = Console.ReadLine();

                graph.StartNode = graph.Nodes.FirstOrDefault(x => x.Name == inputStartNode);
                graph.EndNode = graph.Nodes.FirstOrDefault(x => x.Name == inputEndNode);

                Calculation calculation = new Calculation(graph);

                List<Node> shortestPath = calculation.GetShortestPathDijkstra();

                Console.WriteLine($"Number of nodes visited: {calculation.NodeVisits}");
                Console.WriteLine();
                foreach (var item in shortestPath)
                {
                    Console.WriteLine($"{item.Name}");
                }
            }
        }

        private static void SeeAll()
        {
            if (UserNodes == null || UserNodes.Count() == 0)
            {
                Console.WriteLine("There is currently no existing DAG.");
            }
            else
            {
                Console.WriteLine("Existing nodes in DAG:");
                foreach (var item in UserNodes)
                {
                    Console.WriteLine($"{item.Name} - Number of edges: {item.Destinations.Length}");
                }
            }
        }
        private static void ManuallyCreateDAG()
        {
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
            UserNodes = userNodes;


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
            UserNodes = userNodes;
        }
        private static void AddNewNode()
        {
            List<Node> userNodes = UserNodes;

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
                UserNodes = userNodes;

            }
            else if (AllDefinedNodes().Contains(nodeInput))
            {
                Console.WriteLine("A node with that name already exists. Please, check that node or define a node with a new name.");
            }
            else { };

        }

        private static void AddOrRedefineNode()
        {
            List<Node> userNodes = UserNodes;
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

        private static void AddNewEdge(Node selectedNode)
        {
            Edge[] destinations = selectedNode.Destinations;

            List<Node> userNodes = UserNodes;

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

        private static void ChangeEdgeValue(Node selectedNode)
        {
            Edge[] destinations = selectedNode.Destinations;
            List<Node> userNodes = UserNodes;

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

                    //destinations[Array.IndexOf(destinations,nodeInput)] = actualEdge;

                }
                else if (nodeInput != string.Empty && !AllDefinedNodes().Contains(nodeInput))
                {
                    Console.WriteLine("Specified Node does not exist or nodename misspelled. Please check again");
                }
                else
                {
                    break;
                }
                //selectedNode.Destinations = destinations;
            }

        }

        private static void SaveExistingDAG()
        {
            Dictionary<string, List<string>> userNodesToSave = new Dictionary<string, List<string>>();


            foreach (Node item in UserNodes)
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

            using (StreamWriter file = File.CreateText(NodeFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, userNodesToSave);
            }

            Console.WriteLine("Your DAG has been saved!");
            Console.WriteLine();
        }
        private static void ImportNewDAG()
        {
            if (UserNodes == null || UserNodes.Count() == 0)
            {
                GetImportedUserNodes();
            }
            else
            {
                UserNodes.Clear();
                GetImportedUserNodes();
            }
        }



        private static void GetImportedUserNodes()
        {
            List<Node> userNodes = new List<Node>();

            using StreamReader file = File.OpenText(NodeFilePath);
            JsonSerializer serializer = new JsonSerializer();
            Dictionary<string, List<string>> importedNodes = (Dictionary<string, List<string>>)serializer.Deserialize(file, typeof(Dictionary<string, List<string>>));

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
            UserNodes = userNodes;
        }

        private static List<string> AllDefinedNodes()
        {
            List<string> allDefinedNodes = new List<string>();

            try
            {
                foreach (Node node in UserNodes)
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

        public static string ShowMenu(string prompt, string[] options)
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

    }
}

