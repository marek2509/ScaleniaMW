using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class DopasowanieJednostek
    {
        public int IdJednN { get; set; }
        public int IdJednS { get; set; }
        public int NrJednEwopis { get; set; }
        public int NowyNKR { get; set; }
        public string NrDzialki { get; set; }
        public int IdDz { get; set; }
        public int? PrzypisanyNrRej { get; set; }

        public DopasowanieJednostek(int idjednn, int idjedns, int nrjednewop, int nowynkr, string nrdz, int iddz)
        {
            IdJednN = idjednn;
            IdJednS = idjedns;
            NrJednEwopis = nrjednewop;
            NowyNKR = nowynkr;
            NrDzialki = nrdz;
            IdDz = iddz;
            PrzypisanyNrRej = null;
        }

        public void wypiszWConsoli(String wlasnytxt="")
        {
            Console.WriteLine("{6} {0} {1} {2} {3} {4} {5} {7}", IdJednN,IdJednS ,NrJednEwopis, NowyNKR, NrDzialki, IdDz, wlasnytxt, PrzypisanyNrRej.Equals(null) ? "pusto": PrzypisanyNrRej.ToString());
        }


    }
}
