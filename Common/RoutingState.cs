using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation
{
    public class RoutingState
    {
        public CalcNode Start { get; set; }

        public CalcNode End { get; set; }

        public List<CalcNode> Visited { get; set; } = new List<CalcNode>();

        public List<CalcNode> PrioQueue { get; set; } = new List<CalcNode>();


        public RoutingState(Node startNode, Node endNode)
        {
            this.Start = new CalcNode(startNode);
            this.End = new CalcNode(endNode);
        }

        public CalcNode GetNode(string Name)
        {
            if (Start.Name == Name) return Start;
            if (End.Name == Name) return End;
            return Visited.FirstOrDefault(x => x.Name == Name) ?? PrioQueue.FirstOrDefault(x => x.Name == Name);
        }
    }
}
