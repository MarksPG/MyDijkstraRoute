using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layout_FrameMenu
{
    public interface ICostCalculator
    {
        void UpdateCosts(Graph graph);
    }
}
