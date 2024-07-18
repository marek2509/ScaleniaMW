using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Repositories
{
    public abstract class RepositoryBase<TEntity, TKey>
        where TEntity : class, new()
        where TKey : struct
    {
        internal protected MainDbContext DbContext { get; set; }
        private readonly DbSet<TEntity> dbSet;

        protected RepositoryBase(MainDbContext mainDbContext)
        {
            DbContext = mainDbContext;
            dbSet = DbContext.Set<TEntity>();
        }

        public virtual TEntity GetOne(TKey key)
        {
            TEntity entity = dbSet.Find(key);
            return entity;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual TEntity GetOne(Expression<Func<TEntity, bool>> where)
        {
            TEntity entity = dbSet.Where(where).FirstOrDefault();
            return entity;
        }

        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public static long ToLong(object obj)
        {
            if (obj == null) { return 0; }

            long val;
            if (long.TryParse(obj.ToString(), out val))
            {
                return val;
            }
            return 0;
        }

        public virtual bool Save(TEntity entity)
        {
            //using (IDBEntities context = DBContext)
            //{
            try
            {
                dbSet.AddOrUpdate(entity);
                DbContext.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                //EntityValidationHelper.Log(dbEx);
                return false;
            }
            catch (Exception ex)
            {
                //LogHelper.Log.Error(ex);
                return false;
            }

            //}
        }
    }
}
