using ScaleniaMW.Entities;
using ScaleniaMW.Repositories;
using ScaleniaMW.Repositories.Interfaces;
using ScaleniaMW.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Services
{
    public class Dzialki_NService
    {
        private readonly Dzialki_NRepository _dzialki_NRepository;

        public Dzialki_NService(MainDbContext dbContext)
        {
            _dzialki_NRepository = new Dzialki_NRepository(dbContext);
        }

        public List<Dzialki_n> GetAll()
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
    }
}
