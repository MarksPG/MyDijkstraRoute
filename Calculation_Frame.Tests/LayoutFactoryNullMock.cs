using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation_Frame.Tests
{
    class LayoutFactoryNullMock : ILayoutFactory
    {
        public Graph GetLayout()
        {
            return null;
        }
    }
}
