using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Repositories
{
    public class DzialkaRepository : RepositoryBase<Dzialka, long>
    {
        public DzialkaRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {

        }

        public override IEnumerable<Dzialka> GetAll(Expression<Func<Dzialka, bool>> where)
        {
            try
            {
                return base.GetAll(where).Where(x => x.ID_STI != 1);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
