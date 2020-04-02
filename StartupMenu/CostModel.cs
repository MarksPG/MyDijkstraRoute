using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupMenu
{

    public class CostModel
    {
        [Key]
        [Column(Order = 1)]
        public string FromPosition { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ToPosition { get; set; }

        [Key]
        [Column(Order = 3)]
        public string CostName { get; set; }

        public string Value { get; set; }
    }

}
