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

        public bool Delete(int id)
        {
            var entity = base.GetOne(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                return base.Save(entity);
            }
            return false;
        }


        public override IEnumerable<WZDEDzKW> GetAll(Expression<Func<WZDEDzKW, bool>> where)
        {
            try
            {
                return base.GetAll(where).Where(x => x.IsDeleted == false);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override IEnumerable<WZDEDzKW> GetAll()
        {
            try
            {
                return base.GetAll(x => x.IsDeleted == false);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
