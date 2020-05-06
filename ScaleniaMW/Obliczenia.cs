using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScaleniaMW
{
    public static class Obliczenia
    {
        public static string DopasujNkrDoDziałkiGenerujtxtDoEWM(List<DzialkaEDZ> punkt, List<DzialkaNkrZSQL> dzialkaNkrZSQL, ref string logInfo, bool czyIgnorowacPrzecinkiIKropki, int intKodExpo0NKR1KW, bool czyDopisacBrakKw, bool czyDopisacBlad)
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

            Console.WriteLine("czy dopisac brak kw " + czyDopisacBrakKw + " czy błąd: " + czyDopisacBlad + " ignor przecinek " + czyIgnorowacPrzecinkiIKropki);
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
                        string KW = nkrZSQL.KW;
                        ileDopasowano++;
                        switch (intKodExpo0NKR1KW)
                        {
                            case 0:
                                { 
                                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + nkrZSQL.NKR + "\" _");
                                break;
                                }
                            case 1:
                                { 
                                if (nkrZSQL.KW.Equals(""))
                                {
                                    if (czyDopisacBrakKw) {
                                        KW = "Brak KW";

                                            sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                            break;
                                        }
                                        else
                                    {
                                        Console.WriteLine("break KW" );
                                        break;
                                    }

                                }

                                if (czyDopisacBlad)
                                {


                                    if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolnaBool(nkrZSQL.KW))
                                    {
                                        KW = nkrZSQL.KW + "@Błąd";
                                        Console.WriteLine(KW);
                                    }
                                }
                              
                                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");

                                break;
                                }
                            default:
                                Console.WriteLine("Default case");
                                break;
                        } 
                        
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
                        switch (intKodExpo0NKR1KW)
                        {
                            case 0:
                                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + nkrZSQL.NKR + "\" _");
                                break;
                            case 1:
                                if (nkrZSQL.KW.Equals(""))
                                {
                                    nkrZSQL.KW = "Brak KW";
                                }
                                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + nkrZSQL.KW + "\" _");
                                break;
                            default:
                                Console.WriteLine("Default case");
                                break;
                        }
                       
                    }
                    else
                    {
                        ileNieZnaleziono++;
                        continue;
                    }
                }
            }

            switch (intKodExpo0NKR1KW)
            {
                case 0:
                    logInfo = "Dopasowano " + ileDopasowano + " z " + punkt.Count + " NKR dla działek z pliku. Niedopasowano: " + ileNieZnaleziono;
                    break;
                case 1:
                    logInfo = "Dopasowano " + ileDopasowano + " z " + punkt.Count + "KW";
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
           
            return sb.ToString();
        }


        public static void DopasujNrRejDoNowychDzialek(ref List<DopasowanieJednostek>  listaJednostekZSQL, ListBox listBoxNkr, ListBox listBoxNoweDzialki, ListBox listBoxNrRej)
        {
            for (int i = listaJednostekZSQL[0].IdJednN; i <= listaJednostekZSQL[listaJednostekZSQL.Count - 1].IdJednN; i++)
            {
                List<DopasowanieJednostek> tmpListaJednostek = listaJednostekZSQL.FindAll(x => x.IdJednN.Equals(i));


                if (tmpListaJednostek.Count == tmpListaJednostek.FindAll(x => x.IdJednS.Equals(tmpListaJednostek[0].IdJednS)).Count)
                {
                    foreach (var item in tmpListaJednostek)
                    {
                        item.PrzypisanyNrRej = item.NrJednEwopis;
                    }
                }
            }


            List<int> NowyNkr = new List<int>();
            List<DopasowanieJednostek> tmpListNKRbezJednRej = listaJednostekZSQL.FindAll(x => x.PrzypisanyNrRej.Equals(null));

            foreach (var item in tmpListNKRbezJednRej.GroupBy(x=> x.NowyNKR))
            {
                NowyNkr.Add(item.Key);
            }
            listBoxNkr.ItemsSource = NowyNkr;


            List<string> NowyNrDz = new List<string>();
           // List<DopasowanieJednostek> tmpListNKRbezJednRejNRDZ = tmpListNKRbezJednRej.FindAll(x => x.NowyNKR.Equals(NowyNkr[listBoxNkr.SelectedIndex]));
            List<DopasowanieJednostek> tmpListNKRbezJednRejNRDZ = tmpListNKRbezJednRej.FindAll(x => x.NowyNKR.Equals(NowyNkr[listBoxNkr.SelectedIndex]));
            foreach (var item in tmpListNKRbezJednRejNRDZ.GroupBy(a => a.NrDzialki))
            {
                NowyNrDz.Add(item.Key);
            }
            listBoxNoweDzialki.ItemsSource = NowyNrDz;


            List<int> NrRejGr = new List<int>();
            List<DopasowanieJednostek> tmpListGrRej = listaJednostekZSQL.FindAll(x => x.NowyNKR.Equals(NowyNkr[listBoxNkr.SelectedIndex]));
            //foreach (var item in tmpListGrRej.GroupBy(x => x.NrJednEwopis))
            //{
            //    NrRejGr.Add(item.Key);

            //}
          //  listBoxNrRej.ItemsSource = NrRejGr;
            listBoxNrRej.ItemsSource = tmpListGrRej.GroupBy(x => x.NrJednEwopis).Select(x => x.Key);
            foreach (var item in tmpListGrRej.Select(x => x.NrJednEwopis))
            {
                Console.WriteLine(item);
            }
          

        }
    }
}
