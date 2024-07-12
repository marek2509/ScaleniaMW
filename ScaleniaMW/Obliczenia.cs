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
        public static string DopasujNkrDoDziałkiGenerujtxtDoEWM(List<DzialkaEDZ> punkt, List<DzialkaNkrZSQL> dzialkaNkrZSQL, ref string logInfo, bool czyIgnorowacPrzecinkiIKropki, int intKodExpo0NKR1KW, bool czyDopisacBrakKw, bool czyDopisacBlad, double przesuniecieXtekstu = 8, int justyfikacja = 5)
        {

            StringBuilder sb = new StringBuilder();
            List<string[][]> ls = new List<string[][]>();

            if (punkt.Count.Equals(0))
            {
                logInfo = "Nie wczytano działek! ";
                return "";
            }
            else
            if (dzialkaNkrZSQL.Count.Equals(0))
            {
                logInfo += "Nie połączono z bazą FDB!";
                return "";
            }

            DzialkaNkrZSQL nkrZSQL;
            int ileDopasowano = 0;
            int ileNieZnaleziono = 0;

            if (czyIgnorowacPrzecinkiIKropki)
            {
                foreach (var item in punkt)
                {
                    var tmpCompare = item.Nr_Dz.Trim().Replace("-", "@").Replace(".", "-").Replace("@", ".");

                    if (dzialkaNkrZSQL.Exists(x => x.ObrDzialka.Trim().Equals(tmpCompare)))
                    {
                        nkrZSQL = dzialkaNkrZSQL.Find(x => x.ObrDzialka.Trim().Equals(tmpCompare));
                        //nkrZSQL = dzialkaNkrZSQL.Find(x => x.ObrDzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", "")));
                        string KW = nkrZSQL.KW;
                        ileDopasowano++;
                        switch (intKodExpo0NKR1KW)
                        {
                            case 0:
                                {
                                    sb.AppendLine(" " + (item.DzX1 + przesuniecieXtekstu).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + nkrZSQL.NKR + "\" _");
                                    break;
                                }
                            case 1:
                                {
                                    if (nkrZSQL.KW.Equals(""))
                                    {
                                        if (czyDopisacBrakKw)
                                        {
                                            KW = "Brak KW";
                                            sb.AppendLine(" " + (item.DzX1 + przesuniecieXtekstu).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + KW + "\" _");
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
                                        }
                                    }
                                    sb.AppendLine(" " + (item.DzX1 + przesuniecieXtekstu).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + KW + "\" _");
                                    break;
                                }
                            default:
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

                    if (dzialkaNkrZSQL.Exists(x => x.ObrDzialka.Trim().Equals(item.Nr_Dz.Trim())))
                    {
                        nkrZSQL = dzialkaNkrZSQL.Find(x => x.ObrDzialka.Trim().Equals(item.Nr_Dz.Trim()));
                        ileDopasowano++;
                        string KW = nkrZSQL.KW;
                        switch (intKodExpo0NKR1KW)
                        {
                            case 0:
                                sb.AppendLine(" " + (item.DzX1 + przesuniecieXtekstu).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + nkrZSQL.NKR + "\" _");
                                break;
                            case 1:
                                {
                                    if (nkrZSQL.KW.Equals(""))
                                    {
                                        if (czyDopisacBrakKw)
                                        {
                                            KW = "Brak KW";
                                            sb.AppendLine(" " + (item.DzX1 + przesuniecieXtekstu).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + KW + "\" _");
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
                                        }
                                    }
                                    sb.AppendLine(" " + (item.DzX1 + przesuniecieXtekstu).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + KW + "\" _");
                                    break;
                                }
                            default:
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
                return "";
            }
            else
            if (dzialkaNkrZSQL.Count.Equals(0))
            {
                logInfo += "Nie połączono z bazą FDB!";
                Console.WriteLine("Nie połączono z bazą FDB!");

                return "";
            }

            List<DzNkrKWzSQLProponow> nkrZSQL;
            int ileDopasowano = 0;
            int ileNieZnaleziono = 0;

            if (czyIgnorowacPrzecinkiIKropki)
            {


                foreach (var item in punkt)
                {

                    if (dzialkaNkrZSQL.Exists(x => x.ObrDzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", ""))))
                    {
                        nkrZSQL = dzialkaNkrZSQL.FindAll(x => x.ObrDzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", "")));
                        foreach (var item2 in nkrZSQL)
                        {


                            string KW = item2.ProponowKW;
                            ileDopasowano++;
                            switch (intKodExpo0NKR1KW)
                            {
                                case 0:
                                    {
                                        sb.AppendLine(" " + item.DzX1.ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + item2.NKR + "\" _");
                                        break;
                                    }
                                case 1:
                                    {
                                        if (item2.ProponowKW.Equals(""))
                                        {

                                            if (czyDopisacBrakKw)
                                            {
                                                KW = "Brak KW";

                                                sb.AppendLine(" " + item.DzX1.ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                                break;
                                            }
                                            else
                                            {
                                                break;
                                            }

                                        }

                                        if (czyDopisacBlad)
                                        {


                                            if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolnaBool(item2.ProponowKW))
                                            {
                                                KW = item2.ProponowKW + "@Błąd";
                                            }
                                        }

                                        sb.AppendLine(" " + item.DzX1.ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");

                                        break;
                                    }
                                default:
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

                    if (dzialkaNkrZSQL.Exists(x => x.ObrDzialka.Trim().Equals(item.Nr_Dz.Trim())))
                    {
                        nkrZSQL = dzialkaNkrZSQL.FindAll(x => x.ObrDzialka.Trim().Equals(item.Nr_Dz.Trim()));
                        foreach (var item3 in nkrZSQL)
                        {
                            ileDopasowano++;
                            string KW = item3.ProponowKW;
                            switch (intKodExpo0NKR1KW)
                            {
                                case 0:
                                    sb.AppendLine(" " + item.DzX1.ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + item3.NKR + "\" _");
                                    break;
                                case 1:
                                    {
                                        if (item3.ProponowKW.Equals(""))
                                        {

                                            if (czyDopisacBrakKw)
                                            {
                                                KW = "Brak KW";
                                                sb.AppendLine(" " + item.DzX1.ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
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
                                            }
                                        }
                                        sb.AppendLine(" " + item.DzX1.ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " 5 " + "\"" + KW + "\" _");
                                        break;
                                    }
                                default:
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
            public string NrRejGr { get; set; }
        }
        public class IddzKwPrzed
        {
            public int Iddz { get; set; }
            public string KwPrzed { get; set; }
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
                                licznikZPustym++;
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

                if (listaKWdlaNowychDzialek.Exists(x => x.KWPoDopasowane == "" || x.KWPoDopasowane == null))
                {
                    List<int> NowyNkr = new List<int>();
                    List<DopasowanieKW> tmpListNKRbezJednRej = listaKWdlaNowychDzialek.FindAll(x => (x.KWPoDopasowane == "" || x.KWPoDopasowane == null) && x.KWprzed != "");
                    List<IDiNKR> lisIDnkr_NKR = tmpListNKRbezJednRej.GroupBy(x => new { x.NKRn, x.IdJednN }).Select(x => new IDiNKR { IdJednN = x.Key.IdJednN, NKR = x.Key.NKRn }).ToList();
                    NowyNkr = lisIDnkr_NKR.Select(x => x.NKR).ToList();
                    listBoxNkr.ItemsSource = NowyNkr;
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

                    List<DopasowanieKW> tmpListKW = lisIDnkr_NKR.Count > 0 ? listaKWdlaNowychDzialek.FindAll(x => x.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex]?.IdJednN)) : new List<DopasowanieKW>();
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
                        bool czyPrzypisanoJedenToNieWyswietlajOstrzerzenia = false;
                        foreach (var item in listaKWdlaNowychDzialek)
                        {
                            if (item.IdJednN.Equals(lisIDnkr_NKR[listBoxNkr.SelectedIndex].IdJednN) && item.IdDzN.Equals(iDDZiNRDZ[listBoxNoweDzialki.SelectedIndex].iddz))
                            {
                          
                                item.KWPoDopasowane = listIddzKWPrzed[listBoxNrKW.SelectedIndex].KwPrzed;

                                foreach (var item2 in listaKWdlaNowychDzialek.FindAll(x => x.KWPoDopasowane == listIddzKWPrzed[listBoxNrKW.SelectedIndex].KwPrzed).ToList())
                                {
                                    Console.WriteLine("NR ILE :" + listaKWdlaNowychDzialek.FindAll(x => x.KWPoDopasowane == listIddzKWPrzed[listBoxNrKW.SelectedIndex].KwPrzed).ToList().Count);

                                    if (item.NKRn != item2.NKRn && !czyPrzypisanoJedenToNieWyswietlajOstrzerzenia)
                                    {
                                        czyPrzypisanoJedenToNieWyswietlajOstrzerzenia = true;
                                        var resultat = MessageBox.Show("KW jest przypisana w jednostce nr " + item2.NKRn  + "\nCZY PRZYPISAĆ  KW?" , "ERROR", MessageBoxButton.YesNo);

                                        if (resultat == MessageBoxResult.No)
                                        {
                                            item.KWPoDopasowane = null;
                                            goto wyjdzZPrzycisku;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                } 
                            }
                        }
                        if (listBoxNoweDzialki.Items.Count < 1)
                        {
                            listBoxNkr.SelectedIndex = 0;
                        }
                    wyjdzZPrzycisku:
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
            else
            {
                List<string> lll = new List<string>();
                lll.Add("BRAK DANYCH");
                listBoxNrKW.ItemsSource = lll;
                listBoxNoweDzialki.ItemsSource = lll;
                listBoxNkr.ItemsSource = lll;
            }
        }









        public static string DopasujWartosciDlaNowychDzialek(List<DzialkaEDZ> punkt, List<DzialkaNkrZSQL> dzialkaNkrZSQL, ref string logInfo, bool czyIgnorowacPrzecinkiIKropki, bool czyDopisacBrakKw, double przyrostXdoPrzesuniaeciaNr = 8, int justyfikacja = 8, bool Fkt_Dop = false)
        {

            StringBuilder sb = new StringBuilder();
            List<string[][]> ls = new List<string[][]>();
            string jakichDzialekNieOdnaleziono = "";
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

            Console.WriteLine("czy dopisac brak kw " + czyDopisacBrakKw + " ignor przecinek " + czyIgnorowacPrzecinkiIKropki);
            Console.WriteLine(dzialkaNkrZSQL.Count);
            Console.WriteLine(punkt.Count);
            DzialkaNkrZSQL nkrZSQL;
            int ileDopasowano = 0;
            int ileNieZnaleziono = 0;

            if (czyIgnorowacPrzecinkiIKropki)
            {


                foreach (var item in punkt)
                {

                    if (dzialkaNkrZSQL.Exists(x => x.ObrDzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", ""))))
                    {
                        nkrZSQL = dzialkaNkrZSQL.Find(x => x.ObrDzialka.Trim().Replace("-", "").Replace(".", "").Equals(item.Nr_Dz.Trim().Replace("-", "").Replace(".", "")));


                         string wartosc = "";
                        if (Fkt_Dop)
                        {
                            wartosc = nkrZSQL.Fkt_dop;
                        }
                        else
                        {
                            wartosc = nkrZSQL.WartoscDz;
                        }


                        ileDopasowano++;

                        if (wartosc.Equals(""))
                        {
                            if (czyDopisacBrakKw)
                            {
                                wartosc = "Brak WARTOSCI";

                                sb.AppendLine(" " + (item.DzX1 - przyrostXdoPrzesuniaeciaNr).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + wartosc + "\" _");
                            }
                            else
                            {
                                Console.WriteLine("break KW");
                            }
                        }
                        else
                        {
                            sb.AppendLine(" " + (item.DzX1 - przyrostXdoPrzesuniaeciaNr).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + wartosc + "\" _");
                        }
                    }
                    else
                    {
                        jakichDzialekNieOdnaleziono += item.Nr_Dz + "; ";
                        ileNieZnaleziono++;
                        continue;
                    }
                }
            }
            else
            {
                foreach (var item in punkt)
                {
                    Console.WriteLine("dx1 " + item.DzX1);
                    if (dzialkaNkrZSQL.Exists(x => x.ObrDzialka.Trim().Equals(item.Nr_Dz.Trim())))
                    {
                        nkrZSQL = dzialkaNkrZSQL.Find(x => x.ObrDzialka.Trim().Equals(item.Nr_Dz.Trim()));
                        ileDopasowano++;

                        string wartosc = "";
                        if (Fkt_Dop)
                        {
                            wartosc = nkrZSQL.Fkt_dop;
                        }
                        else
                        {
                            wartosc = nkrZSQL.WartoscDz;
                        }

                        if (wartosc.Equals(""))
                        {
                            if (czyDopisacBrakKw)
                            {
                                wartosc = "Brak Wartości";
                                sb.AppendLine(" " + (item.DzX1 - przyrostXdoPrzesuniaeciaNr).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + wartosc + "\" _");
                            }

                        }
                        else
                        {
                            sb.AppendLine(" " + (item.DzX1 - przyrostXdoPrzesuniaeciaNr).ToString("E9").Replace(",", ".") + " " + item.DzY1.ToString("E9").Replace(",", ".") + " " + 1.ToString("E9").Replace(",", ".") + " " + item.podajeKatUstawienia().ToString().Replace(",", ".") + " " + justyfikacja + " " + "\"" + wartosc + "\" _");

                        }

                    }
                    else
                    {
                        jakichDzialekNieOdnaleziono += item.Nr_Dz + "; ";
                        ileNieZnaleziono++;
                        continue;
                    }
                }
            }

            logInfo = "Dopasowano " + ileDopasowano + " z " + punkt.Count + " WARTOŚCI dla działek z pliku. Niedopasowano: " + ileNieZnaleziono + " Nie znaleziono: " + jakichDzialekNieOdnaleziono;


            return sb.ToString();
        }
    }
}


