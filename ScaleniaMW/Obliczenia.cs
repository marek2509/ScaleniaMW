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

                                        if (czyDopisacBrakKw)
                                        {
                                            KW = "Brak KW";

                                            sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("break KW");
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
                        string KW = nkrZSQL.KW;
                        switch (intKodExpo0NKR1KW)
                        {
                            case 0:
                                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + nkrZSQL.NKR + "\" _");
                                break;
                            case 1:
                                {
                                    if (nkrZSQL.KW.Equals(""))
                                    {

                                        if (czyDopisacBrakKw)
                                        {
                                            KW = "Brak KW";
                                            sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                            break;
                                        }
                                        else
                                        {
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

        public static string DopasujNkrDoDziałkiGenerujtxtDoEWM(List<DzialkaEDZ> punkt, List<DzNkrKWzSQLProponow> dzialkaNkrZSQL, ref string logInfo, bool czyIgnorowacPrzecinkiIKropki, int intKodExpo0NKR1KW, bool czyDopisacBrakKw, bool czyDopisacBlad)
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

            Console.WriteLine("czy dopisac brak kw " + czyDopisacBrakKw + " czy błąd: " + czyDopisacBlad + " ignor przecinek " + czyIgnorowacPrzecinkiIKropki);
            Console.WriteLine(dzialkaNkrZSQL.Count);
            Console.WriteLine(punkt.Count);
           List<DzNkrKWzSQLProponow> nkrZSQL;
            int ileDopasowano = 0;
            int ileNieZnaleziono = 0;

            if (czyIgnorowacPrzecinkiIKropki)
            {


                foreach (var item in punkt)
                {

                    if (dzialkaNkrZSQL.Exists(x => x.Nrdz.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", ""))))
                    {
                        nkrZSQL = dzialkaNkrZSQL.FindAll(x => x.Nrdz.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", "")));
                        foreach (var item2 in nkrZSQL)
                        {

                        
                        string KW = item2.ProponowKW;
                        ileDopasowano++;
                        switch (intKodExpo0NKR1KW)
                        {
                            case 0:
                                {
                                    sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + item2.NKR + "\" _");
                                    break;
                                }
                            case 1:
                                {
                                    if (item2.ProponowKW.Equals(""))
                                    {

                                        if (czyDopisacBrakKw)
                                        {
                                            KW = "Brak KW";

                                            sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("break KW");
                                            break;
                                        }

                                    }

                                    if (czyDopisacBlad)
                                    {


                                        if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolnaBool(item2.ProponowKW))
                                        {
                                            KW = item2.ProponowKW + "@Błąd";
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

                    if (dzialkaNkrZSQL.Exists(x => x.Nrdz.Trim().Equals(item.Nr_Dz.Trim())))
                    {
                        nkrZSQL = dzialkaNkrZSQL.FindAll(x => x.Nrdz.Trim().Equals(item.Nr_Dz.Trim()));
                        foreach (var item3 in nkrZSQL)
                        {
                            ileDopasowano++;
                            string KW = item3.ProponowKW;
                            switch (intKodExpo0NKR1KW)
                            {
                                case 0:
                                    sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + item3.NKR + "\" _");
                                    break;
                                case 1:
                                    {
                                        if (item3.ProponowKW.Equals(""))
                                        {

                                            if (czyDopisacBrakKw)
                                            {
                                                KW = "Brak KW";
                                                sb.AppendLine(" " + item.DzX1.ToString("E").Replace(",", ".") + " " + item.DzY1.ToString("E").Replace(",", ".") + " " + 1.ToString("E").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                                break;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (czyDopisacBlad)
                                        {


                                            if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolnaBool(item3.ProponowKW))
                                            {
                                                KW = item3.ProponowKW + "@Błąd";
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
                    //logInfo = "Dopasowano " + ileDopasowano + " z " + punkt.Count + " NKR dla działek z pliku. Niedopasowano: " + ileNieZnaleziono;
                    break;
                case 1:
                    //logInfo = "Dopasowano " + ileDopasowano + " z " + punkt.Count + "KW";
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            return sb.ToString();
        }

        public class IDiNKR
        {
            public int IdJednN { get; set; }
            public int NKR { get; set; }
        }
        public class IDDZiNRDZ
        {
            public int iddz { get; set; }
            public string Nrdz { get; set; }
        }
        public class IdJednSiNRRej
        {
            public int iddJednSt { get; set; }
            public int NrRejGr { get; set; }
        }
        public class IddzKwPrzed
        {
            public int Iddz { get; set; }
            public string KwPrzed { get; set; }
        }

        public static void DopasujNrRejDoNowychDzialek(ref List<DopasowanieJednostek> listaJednostekZSQL, ListBox listBoxNkr, ListBox listBoxNoweDzialki, ListBox listBoxNrRej, string sender = "")
        {
            if (listaJednostekZSQL.Count > 0)
            {


                if (sender.Equals("AutoPrzypiszJednostki"))
                {
                    for (int i = listaJednostekZSQL[0].IdJednN; i <= listaJednostekZSQL[listaJednostekZSQL.Count - 1].IdJednN; i++)
                    {
                        List<DopasowanieJednostek> tmpListaJednostek = listaJednostekZSQL.FindAll(x => x.IdJednN.Equals(i));


                        if (tmpListaJednostek.Count == tmpListaJednostek.FindAll(x => x.IdJednS.Equals(tmpListaJednostek[0].IdJednS)).Count)
                        {
                            foreach (var item in tmpListaJednostek)
                            {
                                if (item.PrzypisanyNrRej.Equals(null))
                                {
                                    item.PrzypisanyNrRej = item.IdJednS;
                                }
                            }
                        }
                    }
                }

                if (listaJednostekZSQL.Exists(x => x.PrzypisanyNrRej.Equals(null)))
                {

                    List<int> NowyNkr = new List<int>();
                    List<DopasowanieJednostek> tmpListNKRbezJednRej = listaJednostekZSQL.FindAll(x => x.PrzypisanyNrRej.Equals(null));
                    /*
                    foreach (var item in tmpListNKRbezJednRej.GroupBy(x => x.NowyNKR))
                    {
                        NowyNkr.Add(item.Key);
                    }
                    listBoxNkr.ItemsSource = NowyNkr;
                    ////////////*/
                    List<IDiNKR> lisIDnkr_NKR = tmpListNKRbezJednRej.GroupBy(x => new { x.NowyNKR, x.IdJednN }).Select(x => new IDiNKR { IdJednN = x.Key.IdJednN, NKR = x.Key.NowyNKR }).ToList();
                    //foreach (var item in lisIDnkr_NKR)
                    //{ 
                    //    NowyNkr.Add(item.NKR);
                    //}
                    NowyNkr = lisIDnkr_NKR.Select(x => x.NKR).ToList();
                    listBoxNkr.ItemsSource = NowyNkr;
                    ///////////////////
                    List<string> NowyNrDz = new List<string>();
                    // List<DopasowanieJednostek> tmpListNKRbezJednRejNRDZ = tmpListNKRbezJednRej.FindAll(x => x.NowyNKR.Equals(NowyNkr[listBoxNkr.SelectedIndex]));
                    listBoxNkr.SelectedIndex = listBoxNkr.SelectedIndex >= 0 ? listBoxNkr.SelectedIndex : 0;
                    List<DopasowanieJednostek> tmpListNKRbezJednRejNRDZ = tmpListNKRbezJednRej.FindAll(x => x.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN));

                    List<IDDZiNRDZ> iDDZiNRDZ = tmpListNKRbezJednRejNRDZ.GroupBy(a => new { a.NrDzialki, a.IdDz }).Select(x => new IDDZiNRDZ { iddz = x.Key.IdDz, Nrdz = x.Key.NrDzialki }).ToList();
                    NowyNrDz = iDDZiNRDZ.Select(x => x.Nrdz).ToList();
                    //foreach (var item in iDDZiNRDZ)
                    //{
                    //    NowyNrDz.Add(item.Nrdz);
                    //}
                    listBoxNoweDzialki.ItemsSource = NowyNrDz;


                    List<int> NrRejGr = new List<int>();
                    listBoxNkr.SelectedIndex = listBoxNkr.SelectedIndex >= 0 ? listBoxNkr.SelectedIndex : 0;
                    List<DopasowanieJednostek> tmpListGrRej = listaJednostekZSQL.FindAll(x => x.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN));
                    List<IdJednSiNRRej> listidJednSiNRRejs = tmpListGrRej.GroupBy(x => new { x.NrJednEwopis, x.IdJednS }).Select(x => new IdJednSiNRRej { iddJednSt = x.Key.IdJednS, NrRejGr = x.Key.NrJednEwopis }).ToList();
                    NrRejGr = listidJednSiNRRejs.Select(x => x.NrRejGr).ToList();
                    listBoxNrRej.ItemsSource = NrRejGr;

                    listBoxNrRej.SelectedIndex = listBoxNrRej.SelectedIndex >= 0 && listBoxNrRej.SelectedIndex < listBoxNrRej.Items.Count ? listBoxNrRej.SelectedIndex : 0;
                    listBoxNkr.SelectedIndex = listBoxNkr.SelectedIndex >= 0 && listBoxNkr.SelectedIndex < listBoxNkr.Items.Count ? listBoxNkr.SelectedIndex : 0;

                    if (sender.Equals("PrzypiszZaznJedn"))
                    {
                        Console.WriteLine("sender jetsst " + sender.GetHashCode());

                        foreach (var item in listaJednostekZSQL)
                        {
                            //if (item.NowyNKR.Equals(listBoxNkr.SelectedValue) && item.NrDzialki.Equals(listBoxNoweDzialki.SelectedValue))
                            //{
                            //    Console.WriteLine(item.NowyNKR + " rowne " + listBoxNkr.SelectedValue + " " + item.NrDzialki + " " + listBoxNoweDzialki.SelectedValue + " " + item.NrJednEwopis + " " + listBoxNrRej.SelectedValue + " id" + item.IdJednS);
                            //    item.wypiszWConsoli();
                            //    item.PrzypisanyNrRej = (int)listBoxNrRej.SelectedValue;

                            //}
                           
                            if (item.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN) && item.IdDz.Equals(iDDZiNRDZ[listBoxNoweDzialki.SelectedIndex].iddz))
                            {
                                Console.WriteLine(item.NowyNKR + " rowne " + listBoxNkr.SelectedValue + " " + item.NrDzialki + " " + listBoxNoweDzialki.SelectedValue + " " + item.NrJednEwopis + " " + listBoxNrRej.SelectedValue + " id" + item.IdJednS);
                                item.wypiszWConsoli();
                                item.PrzypisanyNrRej = listidJednSiNRRejs[listBoxNrRej.SelectedIndex].iddJednSt;

                            }



                        }

                      //  if (listBoxNoweDzialki.Items.Count == 1 || listBoxNoweDzialki.Items.Count == 0)
                     //   {
                            Console.WriteLine("to teraz");
                           
                     //   }

                        Obliczenia.DopasujNrRejDoNowychDzialek(ref listaJednostekZSQL, listBoxNkr, listBoxNoweDzialki, listBoxNrRej);
                        listBoxNrRej.SelectedIndex = listBoxNrRej.SelectedIndex>=0 && listBoxNrRej.SelectedIndex<listBoxNrRej.Items.Count? listBoxNrRej.SelectedIndex:0;
                        Console.WriteLine(listBoxNrRej.SelectedIndex + " SELECTED INDEX NRREJ " + listBoxNrRej.Items.Count);
                        listBoxNoweDzialki.SelectedIndex = listBoxNoweDzialki.SelectedIndex >= 0 && listBoxNoweDzialki.SelectedIndex < listBoxNoweDzialki.Items.Count ? listBoxNoweDzialki.SelectedIndex : 0;
                    }
                }
                else
                {
                    List<string> lll = new List<string>();
                    lll.Add("WSZYSTKIE DOPASOWANO");
                    listBoxNrRej.ItemsSource = lll;
                    listBoxNoweDzialki.ItemsSource = lll;
                    listBoxNkr.ItemsSource = lll;
                }

            }
        }

        // funkcja dla KW 
        public static void DopasujNrKWDoNowychDzialek(ref List<DopasowanieKW> listaKWdlaNowychDzialek, ListBox listBoxNkr, ListBox listBoxNoweDzialki, ListBox listBoxNrKW, string sender = "")
        {
            if (listaKWdlaNowychDzialek.Count > 0)
            {

                int licznik = 0;
                int licznikZPustym = 0;
                if (sender.Equals("AutoPrzypiszKW"))
                {

                    for (int i = listaKWdlaNowychDzialek[0].IdJednN; i <= listaKWdlaNowychDzialek[listaKWdlaNowychDzialek.Count - 1].IdJednN; i++)
                    {
                        List<DopasowanieKW> tmpListaKW = listaKWdlaNowychDzialek.FindAll(x => x.IdJednN.Equals(i));

                        int nrNiePustego = 0;

                        for (int j = 0; j < tmpListaKW.Count; j++)
                        {
                            if (!(tmpListaKW[j].KWprzed == "" || tmpListaKW[j].KWprzed == null))
                            {
                                nrNiePustego = j;
                            }

                        }

                        if (tmpListaKW.Count != tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count)
                        {
                            if (tmpListaKW.Count == tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count + tmpListaKW.FindAll(x => x.KWprzed.Equals("")).Count)
                            {
                                foreach (var item in tmpListaKW)
                                {
                                    Console.WriteLine(item.NKRn + " | " + item.KWprzed + " | " + item.KWPoDopasowane);
                                }
                                Console.WriteLine(licznikZPustym++);
                            }
                        }
                        // Console.WriteLine(tmpListaKW.Count + " " + tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count +" " + tmpListaKW.FindAll(x => x.KWprzed.Equals("")).Count + tmpListaKW.FindAll(x => x.KWprzed.Equals(null)).Count);

                        if (tmpListaKW.Count == tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count)
                        {
                            foreach (var item in tmpListaKW)
                            {


                                if (item.KWPoDopasowane == null || item.KWPoDopasowane.Trim() == "")
                                {
                                    //  Console.WriteLine(item.NKRn + " " + item.KWprzed +" " + item.KWPoDopasowane);
                                    item.KWPoDopasowane = item.KWprzed;
                                    licznik++;
                                }
                            }
                        }
                    }
                    Console.WriteLine(licznikZPustym + " licznik wskazal: " + licznik);
                }


                if (sender.Equals("AutoPrzypiszKWPrzyblizony"))
                {

                    for (int i = listaKWdlaNowychDzialek[0].IdJednN; i <= listaKWdlaNowychDzialek[listaKWdlaNowychDzialek.Count - 1].IdJednN; i++)
                    {
                        List<DopasowanieKW> tmpListaKW = listaKWdlaNowychDzialek.FindAll(x => x.IdJednN.Equals(i));

                        int nrNiePustego = 0;

                        for (int j = 0; j < tmpListaKW.Count; j++)
                        {
                            if (!(tmpListaKW[j].KWprzed == "" || tmpListaKW[j].KWprzed == null))
                            {
                                nrNiePustego = j;
                            }

                        }

                        if (tmpListaKW.Count != tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count)
                        {
                            if (tmpListaKW.Count == tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count + tmpListaKW.FindAll(x => x.KWprzed.Equals("")).Count)
                            {
                                foreach (var item in tmpListaKW)
                                {
                                    Console.WriteLine(item.NKRn + " | " + item.KWprzed + " | " + item.KWPoDopasowane);
                                }
                                Console.WriteLine(licznikZPustym++);
                            }
                        }

                        // Console.WriteLine(tmpListaKW.Count + " " + tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count +" " + tmpListaKW.FindAll(x => x.KWprzed.Equals("")).Count + tmpListaKW.FindAll(x => x.KWprzed.Equals(null)).Count);

                        if (tmpListaKW.Count == tmpListaKW.FindAll(x => x.KWprzed.Equals(tmpListaKW[nrNiePustego].KWprzed)).Count + tmpListaKW.FindAll(x => x.KWprzed.Equals("")).Count)
                        {
                            foreach (var item in tmpListaKW)
                            {
                                if (item.KWPoDopasowane == null || item.KWPoDopasowane.Trim() == "")
                                {
                                   // item.KWPoDopasowane = item.KWprzed;
                                    item.KWPoDopasowane = tmpListaKW[nrNiePustego].KWprzed;
                                    licznik++;
                                }
                            }
                        }
                    }
                    Console.WriteLine(licznikZPustym + " licznik wskazal: " + licznik);
                }



                if (listaKWdlaNowychDzialek.Exists(x => x.KWprzed.Equals("")))
                {

                    List<int> NowyNkr = new List<int>();
                    List<DopasowanieKW> tmpListNKRbezJednRej = listaKWdlaNowychDzialek.FindAll(x => x.KWPoDopasowane == "" || x.KWPoDopasowane == null);
                    // Console.WriteLine(listaKWdlaNowychDzialek.FindAll(x => x.KWprzed.Equals("")).Count + "count 11 ");
                    List<IDiNKR> lisIDnkr_NKR = tmpListNKRbezJednRej.GroupBy(x => new { x.NKRn, x.IdJednN }).Select(x => new IDiNKR { IdJednN = x.Key.IdJednN, NKR = x.Key.NKRn }).ToList();
                    //foreach (var item in lisIDnkr_NKR)
                    //{ 
                    //    NowyNkr.Add(item.NKR);
                    //}

                    NowyNkr = lisIDnkr_NKR.Select(x => x.NKR).ToList();
                    listBoxNkr.ItemsSource = NowyNkr;
                    //  Console.WriteLine(NowyNkr.Count +  "NKR");
                    ///////////////////
                    List<string> NowyNrDz = new List<string>();
                    // List<DopasowanieJednostek> tmpListNKRbezJednRejNRDZ = tmpListNKRbezJednRej.FindAll(x => x.NowyNKR.Equals(NowyNkr[listBoxNkr.SelectedIndex]));
                    listBoxNkr.SelectedIndex = listBoxNkr.SelectedIndex >= 0 && listBoxNkr.SelectedIndex < listBoxNkr.Items.Count ? listBoxNkr.SelectedIndex : 0;
                    List<DopasowanieKW> tmpListNKRbezKWNRDZ = tmpListNKRbezJednRej.FindAll(x => x.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN));

                    List<IDDZiNRDZ> iDDZiNRDZ = tmpListNKRbezKWNRDZ.GroupBy(a => new { a.NrDZ, a.IdDzN }).Select(x => new IDDZiNRDZ { iddz = x.Key.IdDzN, Nrdz = x.Key.NrDZ }).ToList();
                    NowyNrDz = iDDZiNRDZ.Select(x => x.Nrdz).ToList();
                    listBoxNoweDzialki.ItemsSource = NowyNrDz;
                    listBoxNoweDzialki.SelectedIndex = listBoxNoweDzialki.SelectedIndex >= 0 && listBoxNoweDzialki.SelectedIndex < listBoxNoweDzialki.Items.Count ? listBoxNoweDzialki.SelectedIndex : 0;

                    List<string> NrKW = new List<string>();
                    // listBoxNkr.SelectedIndex = listBoxNkr.SelectedIndex >= 0 ? listBoxNkr.SelectedIndex : 0;
                    listBoxNkr.SelectedIndex = listBoxNkr.SelectedIndex >= 0 ? listBoxNkr.SelectedIndex : 0;

                    List<DopasowanieKW> tmpListKW = listaKWdlaNowychDzialek.FindAll(x => x.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN));
                    // List<IddzKwPrzed> listIddzKWPrzed = tmpListKW.GroupBy(x => new { x.IdJednN, x.KWprzed }).Select(x => new IddzKwPrzed { Iddz = x.Key.IdJednN, KwPrzed = x.Key.KWprzed }).ToList();
                    List<IddzKwPrzed> listIddzKWPrzed;
                    if (tmpListKW.GroupBy(x => new { x.IdJednN, x.KWprzed }).Select(x => new IddzKwPrzed { Iddz = x.Key.IdJednN, KwPrzed = x.Key.KWprzed }).ToList().Count > 1)
                    {
                        listIddzKWPrzed = tmpListKW.FindAll(x => x.KWprzed != "").GroupBy(x => new { x.IdJednN, x.KWprzed }).Select(x => new IddzKwPrzed { Iddz = x.Key.IdJednN, KwPrzed = x.Key.KWprzed }).ToList();
                    }
                    else
                    {
                        listIddzKWPrzed = tmpListKW.GroupBy(x => new { x.IdJednN, x.KWprzed }).Select(x => new IddzKwPrzed { Iddz = x.Key.IdJednN, KwPrzed = x.Key.KWprzed }).ToList();
                    }


                    NrKW = listIddzKWPrzed.Select(x => x.KwPrzed).ToList();

                    listBoxNrKW.ItemsSource = NrKW;

                    listBoxNrKW.SelectedIndex = listBoxNrKW.SelectedIndex >= 0 && listBoxNrKW.SelectedIndex < listBoxNrKW.Items.Count ? listBoxNrKW.SelectedIndex : 0;
                    listBoxNrKW.Items.Refresh();


                    if (sender.Equals("PrzypiszZaznJedn"))
                    {
                        Console.WriteLine("sender jetsst " + sender.GetHashCode());

                        foreach (var item in listaKWdlaNowychDzialek)
                        {
                            //if (item.NowyNKR.Equals(listBoxNkr.SelectedValue) && item.NrDzialki.Equals(listBoxNoweDzialki.SelectedValue))
                            //{
                            //    Console.WriteLine(item.NowyNKR + " rowne " + listBoxNkr.SelectedValue + " " + item.NrDzialki + " " + listBoxNoweDzialki.SelectedValue + " " + item.NrJednEwopis + " " + listBoxNrRej.SelectedValue + " id" + item.IdJednS);
                            //    item.wypiszWConsoli();
                            //    item.PrzypisanyNrRej = (int)listBoxNrRej.SelectedValue;
                            //}

                            if (item.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN) && item.IdDzN.Equals(iDDZiNRDZ[listBoxNoweDzialki.SelectedIndex].iddz))
                            {
                                Console.WriteLine(item.NKRn + " rowne " + listBoxNkr.SelectedValue + " " + item.NrDZ + " " + listBoxNoweDzialki.SelectedValue + " " + item.KWprzed + " " + listBoxNrKW.SelectedValue + " id");

                                item.KWPoDopasowane = listIddzKWPrzed[listBoxNrKW.SelectedIndex].KwPrzed;
                            }
                        }
                        if (listBoxNoweDzialki.Items.Count < 1)
                        {
                            Console.WriteLine("to teraz");
                            listBoxNkr.SelectedIndex = 0;
                        }

                        Obliczenia.DopasujNrKWDoNowychDzialek(ref listaKWdlaNowychDzialek, listBoxNkr, listBoxNoweDzialki, listBoxNrKW);
                        // listBoxNrKW.SelectedIndex = 0;
                        listBoxNrKW.SelectedIndex = listBoxNrKW.SelectedIndex >= 0 && listBoxNrKW.SelectedIndex < listBoxNrKW.Items.Count ? listBoxNrKW.SelectedIndex : 0;
                        listBoxNoweDzialki.SelectedIndex = 0;

                    }

                }
                else
                {
                    List<string> lll = new List<string>();
                    lll.Add("WSZYSTKIE DOPASOWANO");
                    listBoxNrKW.ItemsSource = lll;
                    listBoxNoweDzialki.ItemsSource = lll;
                    listBoxNkr.ItemsSource = lll;
                }

            }
        }
    }
}

