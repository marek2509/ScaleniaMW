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
      

         public DzialkaNkrZSQL(string obrDz, int nkr, decimal wartosc, string kw = "", int grp = 0, string fkt_dop ="")
        {
            ObrDzialka = obrDz;
            NKR = nkr;
            KW = kw;
            _grp = grp;
            WartoscDz = String.Format("{0:F0}", wartosc);
            Fkt_dop = fkt_dop;
        }

        public string ObrDzialka { get; set; } 
        public int NKR { get; set; }
        public string KW { get; set; }
        public string WartoscDz { get; set; }
        public string Fkt_dop { get; set; }
        public int _grp;
    }
}
