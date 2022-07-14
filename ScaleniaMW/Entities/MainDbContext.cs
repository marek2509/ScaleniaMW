using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Entities
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<Jedn_rej_n> Jednostki_rej_n { get; set; }
        public DbSet<Jedn_sn> Jednostki_sn { get; set; }
        public DbSet<Jedn_rej> Jednostki_rej { get; set; }
        public DbSet<Dzialki_n> Dzialki_nowe { get; set; }

    }
}
