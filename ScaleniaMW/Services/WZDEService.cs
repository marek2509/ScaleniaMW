using ScaleniaMW.Entities;
using ScaleniaMW.Repositories;
using ScaleniaMW.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Services
{
    public class WZDEService
    {
        private readonly Dzialki_NRepository _dzialki_NRepository;
        private readonly Jedn_rejRepository _jedn_RejRepository;

        public WZDEService(MainDbContext dbContext)
        {
            _dzialki_NRepository = new Dzialki_NRepository(dbContext);
            _jedn_RejRepository = new Jedn_rejRepository(dbContext);
        }

        public List<Dzialki_n> GetAllParcel()
        {
            try
            {
                return _dzialki_NRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Jedn_rej> GetUnitsWithParcel()
        {
            try
            {
                return _jedn_RejRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
