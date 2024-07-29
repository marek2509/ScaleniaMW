using ScaleniaMW.Entities;
using ScaleniaMW.Repositories.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace ScaleniaMW.Repositories
{
    public class Jedn_rejRepository : RepositoryBase<Jedn_rej, long>
    {
        public Jedn_rejRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {

        }

        public new List<Jedn_rej> GetAll(Expression<Func<Jedn_rej, bool>> where)
        {
           return base.GetAll(where).Where(x => x.ID_STI != 1).ToList();
        }

        public List<GetOwnersForJRResult> GetOwnersForJR(int id)
        {
           return base.DbContext.Database.SqlQuery<GetOwnersForJRResult>(string.Format(Constants.SQL_WlascicielAdresyUdzialyIdNkrStaryFormat0WHERE, $"js.id_id = {id}")).ToList();

        }
    }
}