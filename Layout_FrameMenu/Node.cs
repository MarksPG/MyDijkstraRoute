using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layout_FrameMenu
{
    public class Node
    {
        public string Name { get; set; }

        public Edge[] Destinations { get; set; } = new Edge[] { };

    }
}
