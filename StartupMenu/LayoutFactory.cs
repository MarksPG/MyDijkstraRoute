using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layout_FrameMenu
{
    public class LayoutFactory : ILayoutFactory
    {

        private Graph _graph;
        public Graph GetLayout()
        {
            return _graph;
        }

        public LayoutFactory(Graph graph)
        {
            _graph = graph;
        }
    }
}
