using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class DzialkaNkrZSQL
    {
        public DzialkaNkrZSQL(string obrDz, string nkr, string kw="")
        {
            Obr_Dzialka = obrDz;
            NKR = nkr;
            KW = kw;
        }
        
        public string Obr_Dzialka { get; set; } 
        public string NKR { get; set; }
        public string KW { get; set; }
    }
}
