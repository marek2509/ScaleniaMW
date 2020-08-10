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
        public int Nkr_Przed;
        public decimal WartPrzed
        {
            get => _wartPrzed;
            set
            {
                _wartPrzed = Decimal.Round(value, 2);
                Roznice = WartPo - WartPrzed;
                Odch_3_Proc = Decimal.Round(WartPrzed * 0.03M, 2);
            }
        }
        public decimal WartPo
        {
            get => _wartPo;
            set
            {
                _wartPo = Decimal.Round(value, 2);
                Roznice = WartPo - WartPrzed;
            }
        }
        public decimal Roznice { get; set; }
        public decimal Odch_3_Proc
        {
            get => _odch_3_proc;
            set
            {
                _odch_3_proc = value;
                if (Math.Abs(Roznice) > Odch_3_Proc)
                {
                    CzyDopOdch__3__proc = "NIE";
                }
            }
        }
        public string CzyDopOdch__3__proc { get; set; }
        decimal _odch_3_proc;
        decimal _wartPo;
        decimal _wartPrzed;
        public int IdPo;

        public ZsumwaneWartosciZPorownania()
        {
        }

        public ZsumwaneWartosciZPorownania(int nkr, decimal wartPrzed = 0, decimal wartPo = 0)
        {
            NKR = nkr;
            WartPrzed = wartPrzed;
            WartPo = wartPo;
            Roznice = WartPo - WartPrzed;
        }

        public void wypiszWConsoli()
        {
            Console.WriteLine(NKR + "<NKR WartPrzed>" + WartPrzed + " " + WartPo + "<wart po ROZNICE>" + Roznice + " idPo:>" + IdPo + "nkrprzed>" + Nkr_Przed);
        }
    }
}
