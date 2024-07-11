﻿using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Repositories
{
    public class Dzialki_NRepository : RepositoryBase<Dzialki_n, long>
    {
        public Dzialki_NRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {
            
        }

        public override IEnumerable<Dzialki_n> GetAll(Expression<Func<Dzialki_n, bool>> where)
        {
            try
            {
                return base.GetAll(where).Where(x => x.ID_RD != 0);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}