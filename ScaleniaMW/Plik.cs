using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public static class Plik
    {

        static List<char> cyfry = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };



        public static string UsuniecieInnychZnakowNizCyfry(string tekstDoPoprawki)
        {
            char jakaLiterkeUsunac = ' ';
        wroc:;
            tekstDoPoprawki = tekstDoPoprawki.Replace(jakaLiterkeUsunac.ToString(), "");
            foreach (var item in tekstDoPoprawki)
            {
                if (!cyfry.Exists(x => x == item))
                {
                    jakaLiterkeUsunac = item;
                    goto wroc;
                }
                else
                {
                    continue;
                }
            }
            return tekstDoPoprawki;
        }
    }
}
