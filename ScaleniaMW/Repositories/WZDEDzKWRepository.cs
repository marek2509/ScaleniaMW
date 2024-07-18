using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Repositories
{
    public class WZDEDzKWRepository : RepositoryBase<WZDEDzKW, long>
    {
        public WZDEDzKWRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {

        }
    }
}
