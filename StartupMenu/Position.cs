using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupMenu
{
    
        public class Position
        {
            public int PositionId { get; set; }
            public string FromPosition { get; set; }

            public string ToPosition { get; set; }

            public virtual List<CostData> Costs { get; set; }
        }
    
}
