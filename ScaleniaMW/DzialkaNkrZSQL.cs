using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class DzialkaNkrZSQL
    {

        public DzialkaNkrZSQL()
        {

        }
        public DzialkaNkrZSQL(string obrDz, int nkr, string kw="", int grp=0)
        {
            ObrDzialka = obrDz;
            NKR = nkr;
            KW = kw;
            _grp = grp ;
        }
        
        public string ObrDzialka { get; set; } 
        public int NKR { get; set; }
        public string KW { get; set; }
        public int _grp;
    }
}
