using System.Collections.Generic;

namespace Layout_FrameMenu
{
    public interface IGraph
    {
        string Name { get; set; }
        List<Node> Nodes { get; set; }
    }
}