using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    class ZsumwaneWartosciZPorownania
    {
        public int NKR { get; set; }

        decimal _wartPrzed;
        public decimal WartPrzed
        {
            get => _wartPrzed;
            set
            {
                _wartPrzed = Decimal.Round(value,2);
                Roznice = WartPo - WartPrzed;
            }
        }

        decimal _wartPo;
        public decimal WartPo
        {
            get => _wartPo;          
            set
            {
                _wartPo = Decimal.Round(value,2);
                Roznice = WartPo - WartPrzed;
            }
        }

        public void wypiszWConsoli()
        {
            Console.WriteLine(NKR + " " + WartPrzed + " " + WartPo + " \t\t\tROZNICE" + Roznice + " idPo:" + IdPo);
        }


        public decimal Roznice { get; set; }

        public int IdPo;

        public ZsumwaneWartosciZPorownania()
        {

        }

        public ZsumwaneWartosciZPorownania(int nkr, decimal wartPrzed = 0, decimal wartPo = 0 )
        {
            NKR = nkr;
            WartPrzed = wartPrzed;
            WartPo = wartPo;
            Roznice = WartPo - WartPrzed;
        }
    }
}
