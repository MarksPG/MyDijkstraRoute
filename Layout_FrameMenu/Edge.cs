using System;
using System.Collections.Generic;
using System.Text;

namespace Layout_FrameMenu
{
    public class Edge
    {
        public double Cost { get; set; }

        public Node Destination { get; set; }

        //public override string ToString()
        //{
        //    return "-> " + Destination.ToString();
        //}
    }
}
