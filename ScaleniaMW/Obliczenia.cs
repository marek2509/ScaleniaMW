using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScaleniaMW
{
    public static class Obliczenia
    {
        public static string DopasujNkrDoDziałkiGenerujtxtDoEWM(List<Punkt> punkt, List<DzialkaNkrZSQL> dzialkaNkrZSQL, ref string logInfo)
        { 
            
            StringBuilder sb = new StringBuilder();
            List<string[][]> ls = new List<string[][]>();
           if(punkt.Count.Equals(0))
            {
                logInfo = "Nie wczytano działek! ";
                Console.WriteLine("Nie wczytano działek!");
                return "";
            }else
            if (dzialkaNkrZSQL.Count.Equals(0)) 
            {
                logInfo += "Nie połączono z bazą FDB!";
                Console.WriteLine("Nie połączono z bazą FDB!");

                return "";
            }

            Console.WriteLine(dzialkaNkrZSQL.Count);
            Console.WriteLine(punkt.Count);
            DzialkaNkrZSQL nkrZSQL;
            foreach (var item in punkt)
            {
                nkrZSQL = dzialkaNkrZSQL.Find(x => x.Obr_Dzialka.Trim().Equals(item.NazwaDz.Trim()));
                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" +dzialkaNkrZSQL.Find(x => x.Obr_Dzialka.Trim().Equals(item.NazwaDz.Trim())).NKR + "\" _");
            }

           



            //  parts.Find(x => x.PartName.Contains("seat")));

            // Check if an item with Id 1444 exists.

            //    parts.Exists(x => x.PartId == 1444));



            return sb.ToString();
        }
    }
}
