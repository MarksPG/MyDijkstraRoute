using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupMenu
{
    class DatabaseGraphImportFactory : ILayoutFactory
    {
        private Graph _graph;

        public DatabaseGraphImportFactory(Graph graph)
        {
            _graph = graph;
        }

        public Graph GetLayout()
        {
            return _graph;
        }

    }
}
