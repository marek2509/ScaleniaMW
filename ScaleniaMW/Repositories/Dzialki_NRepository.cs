using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Repositories
{
    public class Dzialki_NRepository : RepositoryBase<Dzialki_n, long>
    {
        public Dzialki_NRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {
            
        }
    }
}
