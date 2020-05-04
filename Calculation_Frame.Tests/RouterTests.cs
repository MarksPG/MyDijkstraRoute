using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculation;
using Layout_FrameMenu;
using Xunit;
using Moq;

namespace Calculation_Frame.Tests
{
    
    public class RouterTests
    {
        private readonly Graph Graph;
        private readonly Router _sut;
        ILayoutFactory _layoutFactoryMock;


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
            
            List<string> nodesAsStrings = new List<string>();

            // Act
            var listOfNodes = _sut.GetShortestPathDijkstra(startNode, endNode);
            foreach (RouterResult item in listOfNodes)
            {
                nodesAsStrings.Add(item.NodeName);
            }

            // Assert
            Assert.Contains(expected, nodesAsStrings);
        }

        //[Fact]
        //public void ShortestPathDijkstra_ErrorMessageShouldBeTrownIfEndpointIsIsland()
        //{
        //    //Arrange
            
        //    string startNode = "N1";
        //    string endNode = "N10";
        //    //Act

        //    //Assert
        //    Assert.Throws<ArgumentException>(() => _sut.GetShortestPathDijkstra(startNode, endNode));
        //}

        //[Fact]
        //public void RouterConstructor_ErrorMessageShouldbeThrownIfGraphIsNull()
        //{
        //    //Arrange
        //    Graph graph = new Graph();
            

        //    var nullMock = new Mock<ILayoutFactory>();
        //    nullMock.Setup(nm => nm.GetLayout()).Returns(graph);
        //    _layoutFactoryMock = nullMock.Object;




        //    //Act
        //    var vm = new Router(_layoutFactoryMock);
        //    //Assert
        //    Assert.Throws<ArgumentException>(() => new Router(_layoutFactoryMock));
        //    //
        //}




    }
}
