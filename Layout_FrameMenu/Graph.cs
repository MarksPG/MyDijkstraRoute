using System;
using System.Collections.Generic;
using System.Text;

namespace Layout_FrameMenu
{
    public class Graph
    {
        public string Name { get; set; }

        public List<Node> Nodes { get; set; } = new List<Node>();

        public Dictionary<string, Dictionary<string, int>> Positions { get; set; } = new Dictionary<string, Dictionary<string, int>>();


        public void UpdateCostsFromPositionsDictionary()
        {
            foreach (Node node in Nodes)
            {
                for (int i = 0; i < node.Destinations.Length; i++)
                {
                    string keyValue = $"{node.Name},{node.Destinations[i].Destination.Name}";
                    node.Destinations[i].AllCosts.Clear();
                    foreach (KeyValuePair<string, int> costs in Positions[keyValue])
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