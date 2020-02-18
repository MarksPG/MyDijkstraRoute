using System;

using System.Collections.Generic;
using System.Text;

namespace Layout
{
    public class Node
    {
        public string Name { get; set; }

        public Edge[] Destinations { get; set; } = new Edge[] { };

    }
}
