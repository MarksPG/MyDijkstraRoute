using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Edge
    {
        public double Cost { get; set; }
        public Node Destination { get; set; }

        public override string ToString()
        {
            return "-> " + Destination.ToString();
        }
    }
}
