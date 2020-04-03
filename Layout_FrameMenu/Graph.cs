using System;
using System.Collections.Generic;
using System.Text;

namespace Layout_FrameMenu
{
    public class Graph : IGraph
    {
        public string Name { get; set; }

        public List<Node> Nodes { get; set; } = new List<Node>();

        public Dictionary<string, Dictionary<string, int>> Positions { get; set; }
    }
}