using Layout_FrameMenu;
using System;

namespace Calculation
{
    public class CalcNode
    {
        public string Name { get; set; }

        public Edge[] Destinations {
            get { return LayoutNode.Destinations; } }

        public Node LayoutNode { get; }

        public CalcNode(Node node)
        {
            
            this.Name = node.Name;
            
            this.LayoutNode = node;
        }

        public double? MinCostToStart { get; set; }
        
        public CalcNode NearestToStart { get; set; }

        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}
