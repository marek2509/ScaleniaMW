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
        public static string DopasujNkrDoDziałkiGenerujtxtDoEWM(List<DzialkaEDZ> punkt, List<DzialkaNkrZSQL> dzialkaNkrZSQL, ref string logInfo, bool czyIgnorowacPrzecinkiIKropki)
        {

            StringBuilder sb = new StringBuilder();
            List<string[][]> ls = new List<string[][]>();

            if (punkt.Count.Equals(0))
            {
                logInfo = "Nie wczytano działek! ";
                Console.WriteLine("Nie wczytano działek!");
                return "";
            }
            else
            if (dzialkaNkrZSQL.Count.Equals(0))
            {
                logInfo += "Nie połączono z bazą FDB!";
                Console.WriteLine("Nie połączono z bazą FDB!");

                return "";
            }

            //List<DzialkaNkrZSQL> dzialkaNkrZSQLsBEZkropIprzec = new List<DzialkaNkrZSQL>(); ;
            //foreach (var item in dzialkaNkrZSQL)
            //{
            //    dzialkaNkrZSQLsBEZkropIprzec = 
            //}


            Console.WriteLine(dzialkaNkrZSQL.Count);
            Console.WriteLine(punkt.Count);
            DzialkaNkrZSQL nkrZSQL;
            int ileDopasowano = 0;
            int ileNieZnaleziono = 0;

            if (czyIgnorowacPrzecinkiIKropki)
            {


                foreach (var item in punkt)
                {

                    if (dzialkaNkrZSQL.Exists(x => x.Obr_Dzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", ""))))
                    {
                        nkrZSQL = dzialkaNkrZSQL.Find(x => x.Obr_Dzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", "")));
                        Console.WriteLine(nkrZSQL.Obr_Dzialka);
                        ileDopasowano++;
                        sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + nkrZSQL.NKR + "\" _");
                    }
                    else
                    {
                        ileNieZnaleziono++;
                        continue;
                    }
                }
            }
            else
            {
                foreach (var item in punkt)
                {

                    if (dzialkaNkrZSQL.Exists(x => x.Obr_Dzialka.Trim().Equals(item.Nr_Dz.Trim())))
                    {
                        nkrZSQL = dzialkaNkrZSQL.Find(x => x.Obr_Dzialka.Trim().Equals(item.Nr_Dz.Trim()));
                        ileDopasowano++;
                        sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + nkrZSQL.NKR + "\" _");
                    }
                    else
                    {
                        ileNieZnaleziono++;
                        continue;
                    }
                }
            }

            logInfo = "Dopasowano " + ileDopasowano + " z " + punkt.Count + " NKR dla działek z pliku. Niedopasowano: " + ileNieZnaleziono;
            return sb.ToString();
        }
    }
}
