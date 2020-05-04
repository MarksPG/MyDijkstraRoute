using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupMenu
{
    public class LayoutContext : DbContext
    {
        public DbSet<CostModel> Costs { get; set; }

    }
}
