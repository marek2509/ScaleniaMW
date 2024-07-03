using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Services.Interfaces
{
    public interface IDzialki_NService
    {
        List<Dzialki_n> GetAll();
    }
}
