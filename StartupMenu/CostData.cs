using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupMenu
{
    public class CostData
    {
        public int CostDataId { get; set; } = 1;
        public int Occupancy { get; set; } = 1;

        public int EmergencyStop { get; set; } = 1;

        public int Speed { get; set; } = 1;

        public int Distance { get; set; } = 1;

        public int DownTime { get; set; } = 1;


        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
    }
}
