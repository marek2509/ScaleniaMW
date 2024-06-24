using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Repositories.Interfaces
{
    public interface IRepository<TEntity, TKey>
        where TEntity : class, new ()
        where TKey : struct
    {
        TEntity GetOne(TKey key);
        IEnumerable<TEntity> GetAll();
        TEntity GetOne(Expression<Func<TEntity, bool>> where);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> where);
    }
}
