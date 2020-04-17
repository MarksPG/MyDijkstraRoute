using Layout_FrameMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RG_Automotive_Test_Implementation
{
    class CostCalculatorRG : ICostCalculator
    {
        private Graph _graph;

        public void UpdateCosts(Graph graph)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection("Server=.\\SQLEXPRESS;Database=LIA2_RGAutomotiveTest;Integrated Security=True;"))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = "select cm.FromPosition, cm.ToPosition, cm.CostName, cm.Value from CostModels cm";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var keyString = $"{reader.GetString(0)},{reader.GetString(1)}";

                        if (_graph.Positions.ContainsKey(keyString))
                        {
                            var x = _graph.Positions[keyString];
                            if (x.ContainsKey(reader.GetString(2)))
                            {
                                x[reader.GetString(2)] = int.Parse(reader.GetString(3));
                            }
                            else
                            {
                                x.Add(reader.GetString(2), int.Parse(reader.GetString(3)));
                            }
                        }
                    }
                }
            }
        }
    }
}
