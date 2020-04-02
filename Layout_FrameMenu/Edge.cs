using System;
using System.Collections.Generic;
using System.Text;

namespace Layout_FrameMenu
{
    public class Edge
    {
        public Node FromNode { get; set; }

        public Node Destination { get; set; }

        public List<Cost> AllCosts { get; set; }
    }
}
