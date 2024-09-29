using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ScaleniaMW.Repositories
{

    public class Jedn_rejNRepository : RepositoryBase<Jedn_rej_n, long>
    {
        public Jedn_rejNRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {

        }

        public new List<Jedn_rej_n> GetAll(Expression<Func<Jedn_rej_n, bool>> where)
        {
            return base.GetAll(where).ToList();
        }

        //public List<GetOwnersForJRResult> GetOwnersForJR(int id)
        //{
        //    return base.DbContext.Database.SqlQuery<GetOwnersForJRResult>(string.Format(Constants.SQL_WlascicielAdresyUdzialyIdNkrStaryFormat0WHERE, $"js.id_id = {id}")).ToList();

        //}
    }
}
