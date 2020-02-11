using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Node
    {
        public string Name { get; set; }

        public Edge[] Destinations { get; set; }

        public double? MinCostToStart { get; set; }

        public Node NearestToStart { get; set; }

        public bool Visited { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
