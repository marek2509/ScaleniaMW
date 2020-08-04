using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    class StanPrzedWartosci
    {
        public decimal Wartosc { get; set; }
        public int IJR { get; set; }
        public int? NKR { get; set; }
        public int? GRP { get; set; }
        public int? IDGRP { get; set; }
        public int id_id;
        public double udzialPrzed = 1;

        public StanPrzedWartosci()
        {

        }

        public StanPrzedWartosci(decimal wart, int ijr, int nkr, int grp, int idgrp, int idJednNowej, double udzial)
        {
            Wartosc = Decimal.Round(wart,2);
            IJR = ijr;
            NKR = nkr;
            GRP = grp;
            IDGRP = idgrp;
            id_id = idJednNowej;
            udzialPrzed = udzial;
        }                  

        public void wypiszwConsoli()
        {
            Console.WriteLine(Wartosc + " "+ IJR + " "+ NKR + " " + GRP + " " + IDGRP);
        }
    }                            
   
}
