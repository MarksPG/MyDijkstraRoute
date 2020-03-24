using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabaseApplication.DataProvider
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Textile> Textile { get; set; }
        public DbSet<Colour> Colour { get; set; }
        public DbSet<Width> Width { get; set; }
        public DbSet<Order> Order { get; set; }

        public DbSet<PriceList> PriceList { get; set; }

        public DbSet<SewingPrice> SewingPrice { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=(local)\SQLEXPRESS;Initial Catalog=WintersTextilSource;Integrated Security=True");
        }

    }
}
