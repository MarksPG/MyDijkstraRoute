using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculation;
using Layout_FrameMenu;
using Xunit;

namespace Calculation_Frame.Tests
{
    public class RouterTests
    {
        private readonly Router _sut;

        private readonly ILayoutFactory _layoutFactoryMock;

        private readonly Graph Graph;

        public RouterTests()
        {
            _layoutFactoryMock = new LayoutFactoryMock();
            _sut = new Router(_layoutFactoryMock);
            Graph = _layoutFactoryMock.GetLayout();
        }


        [Theory]
        [InlineData("N1", "N1", "N9")]
        [InlineData("N9", "N1", "N9")]
        [InlineData("N7", "N1", "N9")]
        public void ShortestPathDijkstra_ListOfStringShouldContainSomeGivenNodes_Theory(string expected, string startNode, string endNode)
        {
            // Arrange
            // See [Theory]

            // Act
            var listOfNodes = _sut.GetShortestPathDijkstra(startNode, endNode);

            // Assert
            Assert.Contains(expected, listOfNodes);
        }

        public void ShortestPathDijkstra_Should 

        
    }
}
