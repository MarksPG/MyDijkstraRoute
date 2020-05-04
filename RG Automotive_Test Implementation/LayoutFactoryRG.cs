using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Automotive_Test_Implementation
{
    public class LayoutFactoryRG : ILayoutFactory
    {
        public Graph Graph { get; }

        public LayoutFactoryRG()
        {
            Graph = new Graph
            {
                Name = "RGLayout",

                Nodes = MirrorDictionaryToList(ReadAllEdgesFromDatabase())
            };
        }
        public Dictionary<string, Dictionary<string, int>> ReadAllEdgesFromDatabase()
        {
            Dictionary<string, Dictionary<string, int>> edges = new Dictionary<string, Dictionary<string, int>>();

            using (var conn = new System.Data.SqlClient.SqlConnection("Server=DB2016\\SQL2016E;Database=RGAutomotive;User ID = elseware;Password = elseware;Connect Timeout = 30;Encrypt = False;TrustServerCertificate = False;ApplicationIntent = ReadWrite;MultiSubnetFailover = False"))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "select cm.FromPositionId, cm.ToPositionId, cm.Cost from PositionRoutes cm";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var keyString = $"{reader.GetString(0)},{reader.GetString(1)}";

                        if (edges.ContainsKey(keyString))
                        {
                            var x = edges[keyString];
                            if (x.ContainsKey("Distance"))
                            {
                                x["Cost"] = int.Parse(reader.GetString(2));
                            }
                            else
                            {
                                x.Add("Cost", int.Parse(reader.GetString(2)));
                            }
                        }
                        else
                        {
                            Dictionary<string, int> initialCost = new Dictionary<string, int>();
                            initialCost.Add("Cost", reader.GetInt32(2));
                            edges.Add(keyString, initialCost);
                        }
                    }
                }
            }

            return edges;
        }

        private List<Node> MirrorDictionaryToList(Dictionary<string, Dictionary<string, int>> connections)
        {
            List<Node> userNodes = new List<Node>();
            HashSet<string> nodeNamesBefore = new HashSet<string>();

            foreach (KeyValuePair<string, Dictionary<string, int>> connection in connections)
            {
                var positions = connection.Key.Split(',');

                if (!nodeNamesBefore.Contains(positions[0]))
                {
                    nodeNamesBefore.Add(positions[0]);
                    Node node = new Node()
                    {
                        Name = positions[0],
                    };
                    userNodes.Add(node);
                }
                if (!nodeNamesBefore.Contains(positions[1]))
                {
                    nodeNamesBefore.Add(positions[1]);
                    Node node = new Node()
                    {
                        Name = positions[1],
                    };
                    userNodes.Add(node);
                }


            }

            for (int i = 0; i < userNodes.Count; i++)
            {
                List<Edge> edges = new List<Edge>();
                foreach (KeyValuePair<string, Dictionary<string, int>> connection in connections)
                {
                    var positions = connection.Key.Split(',');
                    if (positions[0] == userNodes[i].Name)
                    {
                        edges.Add(new Edge
                        {
                            FromNode = userNodes.Where(x => x.Name == positions[0]).Select(x => x).FirstOrDefault(),
                            Destination = userNodes.Where(x => x.Name == positions[1]).Select(x => x).FirstOrDefault(),
                            AllCosts = new List<Cost> { new Cost { CostName = "Cost", Value = 1 } }
                        });
                        //edge.FromNode = userNodes.FirstOrDefault(x => x.Name == positions[0]);
                        //edge.Destination = userNodes.FirstOrDefault(x => x.Name == positions[1]);
                        //edge.AllCosts = new List<Cost>(new Cost[] { new Cost() { CostName = "Cost", Value = 1 } });
                        //edge.FromNode = userNodes.Where(x => x.Name == positions[0]).Select(x => x).FirstOrDefault();
                        //edge.Destination = userNodes.Where(x => x.Name == positions[1]).Select(x => x).FirstOrDefault();
                        ////edge.AllCosts = new List<Cost>(new Cost[] { new Cost() { CostName = "Cost", Value = 1 } });
                        //edge.AllCosts = new List<Cost> { new Cost { CostName = "Cost", Value = 1 } };
                        //edges.Add(edge);
                    }
                }
                userNodes[i].Destinations = edges.ToArray();

            }




            return userNodes;
        }




        public Graph GetLayout()
        {
            return Graph;
        }

    }

}