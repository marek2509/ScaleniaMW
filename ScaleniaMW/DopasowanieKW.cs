using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
     public class DopasowanieKW
    {
        public int IdDzN { get; set; }
        public string NrDZ { get; set; }
        public string KWprzed { get; set; }
        public string KWPoDopasowane { get; set; }
        public int NKRn { get; set; }
        public int IdJednN { get; set; }
        public int IdJednS { get; set; }

        public DopasowanieKW()
        {

        }

        public DopasowanieKW(int idDzN, string nrDz, object kwPrzed, object kwPoDopas, int nkrN, int idJednN, int idJednS )
        {
            IdDzN = idDzN;
            NrDZ = nrDz;
            NKRn = nkrN;
            IdJednN = idJednN;
            IdJednS = idJednS;

            if (kwPrzed.Equals(System.DBNull.Value))
            {
                kwPrzed = null;
            }
            else
            {
                KWprzed = kwPrzed.ToString();
            }

            if (kwPoDopas.Equals(System.DBNull.Value))
            {
                KWPoDopasowane = null;
            }
            else
            {
                KWPoDopasowane = kwPoDopas.ToString();
            }

           
        }

        public void wypiszDaneDoKonsoli()
        {
            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", IdDzN, NrDZ, KWprzed, KWPoDopasowane, NKRn, IdJednN, IdJednS);
        }
    }
}
