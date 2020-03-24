using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation_Frame.Tests
{
    class LayoutFactoryMock : ILayoutFactory
    {
        public Graph GetLayout()
        {
            List<Node> testList = new List<Node>();
            Graph testgraph = new Graph();
            testgraph.Name = "testGraph";

            for (int i = 1; i < 11; i++)
            {
                Node node = new Node() { };
                string nameString = $"N{i}";
                node.Name = nameString;
                testList.Add(node);
            }

            testList[0].Destinations = new Edge[3];
            testList[0].Destinations[0] = new Edge { Cost = new Cost{ InitialValue = 3 }, Destination = testList.FirstOrDefault(x => x.Name == "N2") };
            testList[0].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 7 }, Destination = testList.FirstOrDefault(x => x.Name == "N3") };
            testList[0].Destinations[2] = new Edge { Cost = new Cost { InitialValue = 5 }, Destination = testList.FirstOrDefault(x => x.Name == "N4") };

            testList[1].Destinations = new Edge[2];
            testList[1].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 7 }, Destination = testList.FirstOrDefault(x => x.Name == "N5") };
            testList[1].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 1 }, Destination = testList.FirstOrDefault(x => x.Name == "N3") };

            testList[2].Destinations = new Edge[3];
            testList[2].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 2 }, Destination = testList.FirstOrDefault(x => x.Name == "N5") };
            testList[2].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 1 }, Destination = testList.FirstOrDefault(x => x.Name == "N7") };
            testList[2].Destinations[2] = new Edge { Cost = new Cost { InitialValue = 3 }, Destination = testList.FirstOrDefault(x => x.Name == "N6") };

            testList[3].Destinations = new Edge[2];
            testList[3].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 3 }, Destination = testList.FirstOrDefault(x => x.Name == "N3") };
            testList[3].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 2 }, Destination = testList.FirstOrDefault(x => x.Name == "N6") };

            testList[4].Destinations = new Edge[2];
            testList[4].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 1 }, Destination = testList.FirstOrDefault(x => x.Name == "N8") };
            testList[4].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 2 }, Destination = testList.FirstOrDefault(x => x.Name == "N7") };

            testList[5].Destinations = new Edge[2];
            testList[5].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 3 }, Destination = testList.FirstOrDefault(x => x.Name == "N7") };
            testList[5].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 4 }, Destination = testList.FirstOrDefault(x => x.Name == "N9") };

            testList[6].Destinations = new Edge[2];
            testList[6].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 3 }, Destination = testList.FirstOrDefault(x => x.Name == "N8") };
            testList[6].Destinations[1] = new Edge { Cost = new Cost { InitialValue = 2 }, Destination = testList.FirstOrDefault(x => x.Name == "N9") };

            testList[7].Destinations = new Edge[1];
            testList[7].Destinations[0] = new Edge { Cost = new Cost { InitialValue = 5 }, Destination = testList.FirstOrDefault(x => x.Name == "N9") };

            testList[8].Destinations = new Edge[0];

            testList[9].Destinations = new Edge[0]; //Simulated Island. No other Node connects to this Node.


            testgraph.Nodes = testList;
            return testgraph;
        }



    }


}
