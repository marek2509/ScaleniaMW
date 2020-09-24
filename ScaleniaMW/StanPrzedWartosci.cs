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
      
        public int? IDGRP { get; set; }
        public int id_id;
        public double udzialPrzed = 1;

        public StanPrzedWartosci()
        {

        }

        public StanPrzedWartosci(decimal wart, int ijr, int nkr, int idgrp, int idJednNowej, double udzial)
        {
            Console.WriteLine("1");
            Wartosc = Decimal.Round(wart,2);
            Console.WriteLine("2");
            IJR = ijr;
            Console.WriteLine("3");
            NKR = nkr;
            Console.WriteLine("4");
            if (idgrp == 0)
            {
                IDGRP = null;
            }
            else
            {
                IDGRP =  idgrp;
            }
            Console.WriteLine("5");
            id_id = idJednNowej;
            Console.WriteLine("6");
            udzialPrzed = udzial;
            Console.WriteLine("7");
        }                  

        public void wypiszwConsoli()
        {
            Console.WriteLine(Wartosc + " "+ IJR + " "+ NKR + " " + " " + IDGRP);
        }
    }                            
   
}
