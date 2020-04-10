using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class DzialkaNkrZSQL
    {
        public DzialkaNkrZSQL(string obrDz, string nkr)
        {
            Obr_Dzialka = obrDz;
            NKR = nkr;
        }
        public string Obr_Dzialka;
        public string NKR;
    }
}
