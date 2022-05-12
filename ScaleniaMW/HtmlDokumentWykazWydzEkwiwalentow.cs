using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    abstract class HtmlDokument
    {
        public const string HTML_ZakonczenieWykazuWydzEkwiw = "</body></html>"; //"</div>
        public const string HTML_PodzialNowaStrona = "<br clear=all style = 'mso-special-character:line-break;page-break-before:always'> ";
        public const string HTML_PodzialSekcjiNaStronieNieparzystej = "<br clear=all style='page-break-before:right;mso-break-type:section-break'>";
        public const string HTML_PoczatekWykazyWydzEkwiwalentow = "<!DOCTYPE html> <html lang=\"pl\"> <head>  " +
            "<meta charset=\"windows-1250\"> " +
            "<meta name=Generator content=\"Microsoft Word 12 (filtered)\"> " +
            "<style> " +
            "body { font-family: \"Arial Narrow\", Arial, sans-serif; font-style: italic; font-size: 11pt;} " +
            ".tabelaCzarna {width: 100%; border: 1px solid black; border-collapse: collapse; font-size: 9pt;} " +
            ".borderBl { border: 1px solid black;} " +
            ".greenText {color: green;} " +
            ".borderBl-ml5 { border: 1px solid black; margin-left: 5px;} " +
            ".borderBl-mr5 { border: 1px solid black; margin-right: 5px; text-align: right;} " +
            ".borderBl-ml5-greenText { border: 1px solid black; margin-left: 5px; color: green;} " +
            ".borderBl-mr5-greenText { border: 1px solid black; margin-right: 5px; text-align: right; color: green;} " +
            "</style> " +
            "</head> " +
            "<body>";

        public static string Html_SeparatorPoziomy(int grubosc = 2)
        {
            string gruboscLini = grubosc + "px";

            //return "<div style=\"height:" + gruboscLini + "; width:100%; background-color: black;\"></div>";
            return "<div style=\"width: 100%; background: black; font-size: " + gruboscLini + ";\">&nbsp </div>";
        }

        public static string WierszMalzenstwo(string Udzial = "", int gruboscPodkreslenia = 2 ,string tekstMalzenstwo = "małżeństwo")
        {
            string grubosc = gruboscPodkreslenia > 0 ? gruboscPodkreslenia + "px solid black" : "none";
            return "<tr style=\"border: none;\">" +
                 "<td style=\"border-bottom:"+ grubosc + "; width:90%;\"><span>" + tekstMalzenstwo + "</span><br /><span></span></td>" +
                 "<td style=\"border-bottom:" + grubosc + "; text-align: center; width:10%;\"><span>" + Udzial + "</span> </td></tr>";
        }

        public static string WierszWlasciciel(Wlasciciel wl, int gruboscPodkreslenia = 1, bool czyWyrzucicOstatniePodkreslenie = false)
        {

            string gruboscPodkresl = czyWyrzucicOstatniePodkreslenie ? "none" : gruboscPodkreslenia.ToString() + "px solid black";
            //if (czyWyrzucicOstatniePodkreslenie)
            //{
            //    return "<tr style=\"border: none;\">" +
            //    "<td style=\"border: none; width:90%;\"><span>" + wl.NazwaWlasciciela + "</span><br /><span>" + wl.Adres +
            //    " </span></td>" +
            //    "<td style=\" border: none; text-align: center; width:10%;\"><span>" + wl.Udzial + "</span> </td></tr>";
            //}

            return "<tr style=\"border: none;\">" +
                 "<td style=\"border-bottom: " + gruboscPodkresl + "; width:90%;\"><span>" + wl.NazwaWlasciciela + "</span><br /><span>" + wl.Adres +
                 " </span></td>" +
                 "<td style=\"border-bottom: " + gruboscPodkresl + "; text-align: center; width:10%;\"><span>" + wl.Udzial + "</span> </td></tr>";
        }

        public static string WierszWlasciciel(WlascicielStanPrzed wl, int gruboscPodkreslenia = 1, bool czyWyrzucicOstatniePodkreslenie = false)
        {
            return WierszWlasciciel((wl as Wlasciciel), gruboscPodkreslenia, czyWyrzucicOstatniePodkreslenie);
        }

        // zwróci true jeśli są różne właśności w stanie przed i po scaleniu
        public static bool CzyGenerowacWlascicieliZStarychJEdnostek(JR_Nowa JednoskaRejNowa)
        {

            int ilczbWlasNowych = JednoskaRejNowa.Wlasciciele.Count;
            List<int> liczbaWlascicieliDlaJednostek = new List<int>();
            JednoskaRejNowa.zJednRejStarej.ForEach(x => liczbaWlascicieliDlaJednostek.Add(x.Wlasciciele.Count));

            foreach (var lplWlasc in liczbaWlascicieliDlaJednostek)
            {
                if (lplWlasc != ilczbWlasNowych)
                {
                    return true;
                }
            }

            foreach (var NowiWlasciciele in JednoskaRejNowa.Wlasciciele)
            {
                foreach (var zJednStarej in JednoskaRejNowa.zJednRejStarej)
                {
                    if (!zJednStarej.Wlasciciele.Exists(x => x.NazwaWlasciciela == NowiWlasciciele.NazwaWlasciciela))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string HTML_NaglowekNrGosp<T>(T nrIjrPo)
        {
            return "<div style=\"text-align: right;\"><b> <span style=\"color: red\">NUMER GOSPODARSTWA &nbsp;</span> <span style = \"color: blue; text-decoration: underline; font-size: 14pt\">" +
              nrIjrPo.ToString() + "</span></b></div>";
        }

        public static string HTML_NaglowekObreb<T, T2>(T nrObr, T2 nazwaObrebu)
        {
            return "<div><span>Obręb:&nbsp;<b  style = \"color: blue; \">" + nrObr.ToString() + "&nbsp;" + nazwaObrebu.ToString() + "</b></span></div>";
        }

        public static string HTML_NaglowekJednostkaRejestrowaPrzed<T>(T nrJednostkiPrzed)
        {
            return "<div><span>Numer jednostki rejestrowej: " + nrJednostkiPrzed + "<br /></div>";
        }

        public static string HTML_NaglowekTabelaWlascicieleIWladajacy(List<Wlasciciel> wlasciciele, bool usunOstatniePodkreslenie = false)
        {
            StringBuilder sb = new StringBuilder();
            bool czyWyrzucicOstatniePodkreslenie = false;
            int licznikMalzenski = 0;

            sb.AppendLine("<table width=100%>");
            sb.AppendLine("<tr style=\"border: none;\"> <td style=\"border-bottom: 2px solid black; width:90%;\"><span style=\"margin-bottom: 0; padding: 0;\">Właściciele i władający</span></td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<table width=100%>");

            foreach (var wlascicelPo in wlasciciele)
            {
                if (usunOstatniePodkreslenie && wlascicelPo.Equals(wlasciciele.Last())) czyWyrzucicOstatniePodkreslenie = true;

                if (wlascicelPo.IdMalzenstwa > 0 && licznikMalzenski == 0)
                {
                    licznikMalzenski++;
                    sb.AppendLine(WierszMalzenstwo(wlascicelPo.Udzial));
                    sb.AppendLine(WierszWlasciciel(wlascicelPo, 1, czyWyrzucicOstatniePodkreslenie: czyWyrzucicOstatniePodkreslenie));
                }
                else
                if (wlascicelPo.IdMalzenstwa > 0 && licznikMalzenski > 0)
                {
                    sb.AppendLine(WierszWlasciciel(wlascicelPo, gruboscPodkreslenia: 2, czyWyrzucicOstatniePodkreslenie: czyWyrzucicOstatniePodkreslenie));
                    licznikMalzenski = 0;
                }
                else
                {
                    sb.AppendLine(WierszWlasciciel(wlascicelPo, gruboscPodkreslenia: 2, czyWyrzucicOstatniePodkreslenie: czyWyrzucicOstatniePodkreslenie));
                }
            }
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static string HTML_NaglowekTabelaWlascicieleIWladajacy(List<WlascicielStanPrzed> listaWlascicieliPrzed)
        {
            List<Wlasciciel> listaWlascicieli = new List<Wlasciciel>();
            listaWlascicieliPrzed.ForEach(x => listaWlascicieli.Add(x as Wlasciciel));
           return HTML_NaglowekTabelaWlascicieleIWladajacy(listaWlascicieli as List<Wlasciciel>, true);
        }

        public static string HTML_Uwaga(string trescUwagi)
        {
            return "<div style=\"color: red\"><i>" + trescUwagi + "</i></div>";
        }

        public static string HTML_TytulZakreslony(string tytul, string kolorZakreslacza = "#ffff40")
        {
            return "<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>" + tytul + "</i></u></b></span></div>";
        }

        public static string HTML_TabelaEkwiwalentuNaleznego(JR_Nowa JednoskaRejNowa)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"tabelaCzarna\">");
            sb.AppendLine("<tr>" +
                            "<th> Obręb </th>" +
                            "<th class=\"borderBl\"> Nazwa obrębu </th>" +
                            "<th class=\"borderBl\"> Nr jednostki </th>" +
                            "<th class=\"borderBl\"> Udział </th>" +
                            "<th class=\"borderBl\"> Powierzchnia </th>" +
                            "<th class=\"greenText borderBl\"> Należność </th> " +
                          "</tr>");

            foreach (var zeStarejJedn in JednoskaRejNowa.zJednRejStarej)
            {
                sb.AppendLine("<tr> " +
                                "<td class=\"borderBl-ml5\">" + zeStarejJedn.NrObr + "</td>" +
                                "<td class=\"borderBl-ml5\">" + zeStarejJedn.NazwaObrebu + "</td>" +
                                "<td class=\"borderBl-ml5\">" + zeStarejJedn.Ijr_Przed + "</td>" +
                                "<td class=\"borderBl-ml5\">" + zeStarejJedn.Ud_Z_Jrs + "</td>" +
                                "<td class=\"borderBl-mr5\">" +   zeStarejJedn.Pow_Przed.ToString("F4", CultureInfo.InvariantCulture) + "</td>" +
                                "<td class=\"borderBl-mr5 greenText\">" + zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</td> " +
                              "</tr>");
            }

            //podsumowanie tabeli udzialow ze starych jednostek
            sb.AppendLine("<td colspan=\"4\" style=\"text-align: right; border: none;  margin-right:2px; \">" +
                                 "<span>Powierzchnia/</span><span class=\"greenText\">Wartość przed scaleniem:</span></td>" +
                             "<td class=\"borderBl-mr5\"><b>" + JednoskaRejNowa.SumaPowJednostekPrzed().ToString("F4", CultureInfo.InvariantCulture) + "</b></td>" +
                             "<td class=\"borderBl-mr5-greenText\"><b>" + JednoskaRejNowa.SumaWartJednostekPrzed().ToString("F2", CultureInfo.InvariantCulture) + "</b></td>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        //public static string GenerujTabeleWlascicieliPRZED(ZJednRejStarej WlascicielePrzed, string szerTabeli)
        //{
        //    bool czyWyrzucicOstatniePodkreslenie = false;
        //    StringBuilder dokHTML = new StringBuilder();
        //    int licznikMalzenskiPrzed = 0;
        //    dokHTML.AppendLine("<table width=" + szerTabeli + ">");

        //    foreach (var wlascicelPrzed in WlascicielePrzed.Wlasciciele)
        //    {
        //        if (wlascicelPrzed.Equals(WlascicielePrzed.Wlasciciele.Last()))
        //        {
        //            czyWyrzucicOstatniePodkreslenie = true;
        //        }
        //        if (wlascicelPrzed.IdMalzenstwa > 0 && licznikMalzenskiPrzed == 0)
        //        {
        //            licznikMalzenskiPrzed++;
        //            dokHTML.AppendLine(WierszMalzenstwo(wlascicelPrzed.Udzial));
        //            dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed));
        //        }
        //        else
        //        if (wlascicelPrzed.IdMalzenstwa > 0 && licznikMalzenskiPrzed > 0)
        //        {
        //            dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed, 2, czyWyrzucicOstatniePodkreslenie));
        //            licznikMalzenskiPrzed = 0;
        //        }
        //        else
        //        {
        //            dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed, 2, czyWyrzucicOstatniePodkreslenie));
        //        }
        //    }
        //    dokHTML.AppendLine("</table>");
        //    return dokHTML.ToString();
        //}
    }


    class HtmlDokumentWykazWydzEkwiwalentow : HtmlDokument
    {
        public static String GenerujWykazWE(JR_Nowa jednoskaRejNowa)
        {
            string kolorZakreslacza = "#ffff40";
            string szerTabeli = "100%";
            decimal SumaWartosciPrzed = jednoskaRejNowa.SumaWartJednostekPrzed();
            decimal SumaWartosciPo = jednoskaRejNowa.Dzialki_Nowe.Sum(x => x.Wartosc);



            StringBuilder dokHTML = new StringBuilder();
            //dokHTML.AppendLine("<div style=\"text-align: right;\"><b> <span style=\"color: red\">NUMER GOSPODARSTWA &nbsp;</span> <span style = \"color: blue; text-decoration: underline; font-size: 14pt\">" + 
            //    JednoskaRejNowa.IjrPo + "</span></b></div>");
            dokHTML.AppendLine(HTML_NaglowekNrGosp(jednoskaRejNowa.IjrPo));

            //dokHTML.AppendLine("<div><span>Obręb:&nbsp;<b  style = \"color: blue; \">" + JednoskaRejNowa.NrObr + "&nbsp;" + JednoskaRejNowa.NazwaObrebu + "</b></span></div>");
            dokHTML.AppendLine(HTML_NaglowekObreb(jednoskaRejNowa.NrObr, jednoskaRejNowa.NazwaObrebu));

            //dokHTML.AppendLine("<div><span>Numer jednostki rejestrowej: " + JednoskaRejNowa.Nkr + "<br /></div>");
            dokHTML.AppendLine(HTML_NaglowekJednostkaRejestrowaPrzed(jednoskaRejNowa.Nkr));

            //nagłówek właściciele i władający
            /*
            dokHTML.AppendLine("<table width=" + szerTabeli + ">");
            dokHTML.AppendLine("<tr style=\"border: none;\"> <td style=\"border-bottom: 2px solid black; width:90%;\"><span style=\"margin-bottom: 0; padding: 0;\" >Właściciele i władający</span></td></tr>");
            dokHTML.AppendLine("</table>");

            // tabela z obecnymi właścicielami
            int licznikMalzenski = 0;
            dokHTML.AppendLine("<table width=" + szerTabeli + ">");

            foreach (var wlascicelPo in JednoskaRejNowa.Wlasciciele)
            {
                if (wlascicelPo.IdMalzenstwa > 0 && licznikMalzenski == 0)
                {
                    licznikMalzenski++;
                    dokHTML.AppendLine(WierszWlasciciel("małżeństwo", Udzial: wlascicelPo.Udzial));
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPo.NazwaWlasciciela, wlascicelPo.Adres));
                }
                else
                if (wlascicelPo.IdMalzenstwa > 0 && licznikMalzenski > 0)
                {
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPo.NazwaWlasciciela, wlascicelPo.Adres, gruboscPodkreslenia: 2));
                    licznikMalzenski = 0;
                }
                else
                {
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPo.NazwaWlasciciela, wlascicelPo.Adres, wlascicelPo.Udzial, 2));
                }
            }
            dokHTML.AppendLine("</table>");
            */

            dokHTML.AppendLine(HTML_NaglowekTabelaWlascicieleIWladajacy(jednoskaRejNowa.Wlasciciele));
            //dokHTML.AppendLine(Html_SeparatorPoziomy());
            //if(JednoskaRejNowa.zJednRejStarej.Count> 0) dokHTML.AppendLine(GenerujTabeleWlascicieliPRZED(JednoskaRejNowa.zJednRejStarej[0], "100%"));



            //dokHTML.AppendLine(HTML_NaglowekTabelaWlascicieleIWladajacy(()))
            // uwaga 
            //dokHTML.AppendLine("<div style=\"color: red\"><i>" + JednoskaRejNowa.Uwaga + "</i></div>");

            dokHTML.AppendLine(HTML_Uwaga(jednoskaRejNowa.Uwaga));

            //nagłówek nad tabelą
            //dokHTML.AppendLine("<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>Ekwiwalent należny wg. udziału w pozycjach rejestrowych</i></u></b></span></div>");
            dokHTML.AppendLine(HTML_TytulZakreslony("Ekwiwalent należny wg. udziału w pozycjach rejestrowych"));

            //tabela ekwiwalentu [obr][nazwaobr][nrjedn][udzial][powudzialu][naleznosc]


            //dokHTML.AppendLine("<table  width=" + szerTabeli + " style =\"border: 2px solid black; border-collapse: collapse; font-size: 9pt \">");
            //dokHTML.AppendLine("<tr> <th style=\"border: 1px solid black; \"> Obręb </th><th style=\"border: 1px solid black; \"> Nazwa obrębu </th><th style=\"border: 1px solid black; \"> Nr jednostki </th><th style=\"border: 1px solid black; \"> Udział </th><th style=\"border: 1px solid black; \"> Powierzchnia </th><th style=\"border: 1px solid black; color: green; \"> Należność </th> </tr>");

            //foreach (var zeStarejJedn in jednoskaRejNowa.zJednRejStarej)
            //{
            //    dokHTML.AppendLine("<tr> <td style=\"border: 1px solid black; margin-left: 5px;  \">" + zeStarejJedn.NrObr + "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.NazwaObrebu + "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.Ijr_Przed +
            //        "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.Ud_Z_Jrs + "</td><td style=\"border: 1px solid black; margin-right: 5px;text-align: right; \">" + zeStarejJedn.Pow_Przed.ToString("F4", CultureInfo.InvariantCulture) + "</td><td style=\"border: 1px solid black; color: green; text-align: right; margin-right: 5px; \">" + zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</td> </tr>");
            //}

            ////podsumowanie tabeli udzialow ze starych jednostek
            //dokHTML.AppendLine("<td colspan=\"4\" style=\"text-align: right; border: none;  margin-right:2px;  \"><span>Powierzchnia/<span style=\"color: green;\">Wartość przed scaleniem:</span></span></td><td style=\"border: 1px solid black; text-align: right; margin-right: 5px;\" > <b>" + jednoskaRejNowa.zJednRejStarej.Sum(x => x.Pow_Przed).ToString("F4", CultureInfo.InvariantCulture) + "</b></td><td  style=\"border: 1px solid black; color: green;  text-align: right; margin-right: 5px;\"><b>" + SumaWartosciPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</b></td>");
            //dokHTML.AppendLine("</table>");


            dokHTML.AppendLine(HTML_TabelaEkwiwalentuNaleznego(jednoskaRejNowa));


            ///////////////////////////////////////////////////////   TABLE TABLE TABLE 


            //tytuł tabeli Ekwiwalent nalezny / zaprjektowany
            //dokHTML.AppendLine("<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>Ekwiwalent należny/zaprojektowany</i></u></b></span></div>");
            dokHTML.AppendLine(HTML_TytulZakreslony("Ekwiwalent należny/zaprojektowany"));

            if (jednoskaRejNowa.zJednRejStarej.Count > 0)
            {
                foreach (var jednostkaStara in jednoskaRejNowa.zJednRejStarej)
                {
                    //dokHTML.AppendLine("<div><span>Obręb:&nbsp;<b  style = \"color: blue; \">" + jednostkaStara.NrObr + "&nbsp;" + jednostkaStara.NazwaObrebu + "</b></span></div>");
                    dokHTML.AppendLine(HTML_NaglowekObreb(jednostkaStara.NrObr, jednostkaStara.NazwaObrebu));
                    dokHTML.AppendLine(HTML_NaglowekJednostkaRejestrowaPrzed(jednostkaStara.Ijr_Przed));

                    if (CzyGenerowacWlascicieliZStarychJEdnostek(jednoskaRejNowa))
                    {
                        //dokHTML.AppendLine("<div style=\"border-bottom: 2px solid black;\"><span>Numer jednostki rejestrowej " + jednostkaStara.Ijr_Przed + "</span>");
                        Console.WriteLine("inny wlasciciel przed dla NKRu: " + jednoskaRejNowa.IjrPo);
                        //dokHTML.AppendLine("<br/><span style=\"margin-bottom: 0; padding: 0; \" >Właściciele i władający</span></div>");
                        //dokHTML.Append(GenerujTabeleWlascicieliPRZED(jednostkaStara, szerTabeli));
                        dokHTML.AppendLine(HTML_NaglowekTabelaWlascicieleIWladajacy(jednostkaStara.Wlasciciele));
                    }



                    //tabela Ekwiwalentów nagłówki
                    dokHTML.AppendLine("<table  width=" + szerTabeli + " style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; \"><tr><th colspan=\"8\"><b>Ekwiwalenty</b></th></tr>");
                    dokHTML.AppendLine("<tr><th style =\"border: 1px solid black;\"  colspan=\"4\" width=50%><b>Należny</b></th><th style =\"border: 1px solid black;\"  colspan=\"4\"  width=50%><b>Zaprojektowany</b></th></tr>");
                    dokHTML.AppendLine("<tr><th style =\"border: 1px solid black;\" width=12%><b>Działka</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Pow. ewid.</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Wartość</b></th><th style =\"border: 1px solid black;\"  width=14%><b>KW</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Działka</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Pow.</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Wartość</b></th><th style =\"border: 1px solid black;\"  width=14%><b>KW</b></th></tr>");

                    // treść zasadnicza 


                    if (jednoskaRejNowa.Dzialki_Nowe.Count > 0 && jednoskaRejNowa.Dzialki_Nowe.First().RjdrPrzed == jednostkaStara.Id_Jedns)
                    {
                        int iloscDzialekPrzed = jednostkaStara.Dzialki.Count;
                        int iloscDzialekPo = jednoskaRejNowa.Dzialki_Nowe.Count;

                        int liczbaIteracjiPetli = iloscDzialekPo < iloscDzialekPrzed ? iloscDzialekPrzed : iloscDzialekPo;

                        for (int i = 0; i < liczbaIteracjiPetli; i++)
                        {
                            string nrdzPrzed = iloscDzialekPrzed > i ? jednostkaStara.Dzialki[i].NrDz : "";
                            string pewDzialki = iloscDzialekPrzed > i ? jednostkaStara.Dzialki[i].PowDz.ToString("F4", CultureInfo.InvariantCulture) : "";
                            string wartPrzed = iloscDzialekPrzed > i ? jednostkaStara.Dzialki[i].Wartosc.ToString("F2", CultureInfo.InvariantCulture) : "";
                            string kwPrzed = iloscDzialekPrzed > i ? jednostkaStara.Dzialki[i].KW : "";

                            string nrdzPo = iloscDzialekPo > i ? jednoskaRejNowa.Dzialki_Nowe[i].NrDz : "";
                            string powDzialkiPo = iloscDzialekPo > i ? jednoskaRejNowa.Dzialki_Nowe[i].PowDz.ToString("F4", CultureInfo.InvariantCulture) : "";
                            string wartPo = iloscDzialekPo > i ? jednoskaRejNowa.Dzialki_Nowe[i].Wartosc.ToString("F2", CultureInfo.InvariantCulture) : "";
                            string kwPo = iloscDzialekPo > i ? jednoskaRejNowa.Dzialki_Nowe[i].KW : "";

                            // Ekwiwalent należny
                            dokHTML.AppendLine("<tr><td style =\"border: 1px solid black; text-align: center; \">" + nrdzPrzed + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + pewDzialki + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + wartPrzed + "</td><td style =\"border: 1px solid black; text-align: center; \">" + kwPrzed + "</td>");

                            // Ekwiwalent zaprojektowany
                            dokHTML.AppendLine("<td style =\"border: 1px solid black; text-align: center;  color: red; \">" + nrdzPo + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + powDzialkiPo + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + wartPo + "</td><td style =\"border: 1px solid black; text-align: center; \">" + kwPo + "</td></tr>");

                        }
                        dokHTML.AppendLine("<tr><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \"><b>" + jednostkaStara.Dzialki.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" ><b>" + jednostkaStara.Dzialki.Sum(x => x.Wartosc).ToString("F2", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: center; \"></td>");
                        dokHTML.AppendLine("<td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \"><b>" + jednoskaRejNowa.Dzialki_Nowe.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" ><b>" + SumaWartosciPo.ToString("F2", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: center; \"></td></tr>");
                    }
                    else
                    {
                        foreach (var dzialkaPrzed in jednostkaStara.Dzialki)
                        {

                            dokHTML.AppendLine("<tr><td style =\"border: 1px solid black; margin-left:5px; \">" + dzialkaPrzed.NrDz + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + dzialkaPrzed.PowDz.ToString("F4", CultureInfo.InvariantCulture) + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + dzialkaPrzed.Wartosc.ToString("F2", CultureInfo.InvariantCulture) + "</td><td style =\"border: 1px solid black;\"   text-align: center;>" + dzialkaPrzed.KW + "</td><td style =\"border: 1px solid black;\" ></td><td style =\"border: 1px solid black;\" ></td><td style =\"border: 1px solid black;\" ></td><td style =\"border: 1px solid black;\" ></td></tr>");
                        }
                        dokHTML.AppendLine("<tr><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \"><b>" + jednostkaStara.Dzialki.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" ><b>" + jednostkaStara.Dzialki.Sum(x => x.Wartosc).ToString("F2", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: center; \"></td>");
                        dokHTML.AppendLine("<td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<b>-.----</b>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + "<b>-.--</b>" + "</td><td style =\"border: 1px solid black; text-align: center; \"></td></tr>");

                    }
                    dokHTML.AppendLine("</table>");
                    dokHTML.AppendLine("<br />");
                }
            }
            else // Przypadek gdy jest tylko stan PO
            {
                Console.WriteLine("stan tylko Po :" + jednoskaRejNowa.IjrPo);
                //tabela Ekwiwalentów nagłówki
                dokHTML.AppendLine("<table  width=" + szerTabeli + " style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; \"><tr><th colspan=\"8\"><b>Ekwiwalenty</b></th></tr>");
                dokHTML.AppendLine("<tr><th style =\"border: 1px solid black;\" colspan=\"4\" width=50%><b>Należny</b></th><th style =\"border: 1px solid black;\"  colspan=\"4\"  width=50%><b>Zaprojektowany</b></th></tr>");
                dokHTML.AppendLine("<tr><th style =\"border: 1px solid black;\" width=12%><b>Działka</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Pow. ewid.</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Wartość</b></th><th style =\"border: 1px solid black;\"  width=14%><b>KW</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Działka</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Pow.</b></th><th style =\"border: 1px solid black;\"  width=12%><b>Wartość</b></th><th style =\"border: 1px solid black;\"  width=14%><b>KW</b></th></tr>");
                dokHTML.AppendLine("<tr><td style =\"border: 1px solid black; text-align: center; \">" + "" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + "" + "</td><td style =\"border: 1px solid black; text-align: center; \">" + "" + "</td>");

                // Ekwiwalent zaprojektowany
                foreach (var dzialkaNowa in jednoskaRejNowa.Dzialki_Nowe)
                {
                    dokHTML.AppendLine("<td style =\"border: 1px solid black; text-align: center;  color: red; \">" + dzialkaNowa.NrDz + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + dzialkaNowa.PowDz.ToString("F4", CultureInfo.InvariantCulture) + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + dzialkaNowa.Wartosc.ToString("F2", CultureInfo.InvariantCulture) + "</td><td style =\"border: 1px solid black; text-align: center; \">" + dzialkaNowa.KW + "</td></tr>");

                }

                dokHTML.AppendLine("<tr><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \"><b>" + "-.----" + "</b></td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" ><b>" + "-.--" + "</b></td><td style =\"border: 1px solid black; text-align: center; \"></td>");
                dokHTML.AppendLine("<td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \"><b>" + jednoskaRejNowa.Dzialki_Nowe.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" ><b>" + SumaWartosciPo.ToString("F2", CultureInfo.InvariantCulture) + "</b></td><td style =\"border: 1px solid black; text-align: center; \"></td></tr>");
                dokHTML.AppendLine("</table>");
                dokHTML.AppendLine("<br />");
            }


            // BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW
            dokHTML.AppendLine("<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"color: red\"><b><u>BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW</u></b></span></div>");
            // Tabelka czerwona BILANSU
            dokHTML.AppendLine("<table width=" + szerTabeli + " style =\"border: 2px solid red; border-collapse: collapse; font-size: 9pt; \"><tr><th style =\"border: 1px solid red;  color: red;\" width=12%><b>RAZEM GOSP.:</b></th><th style =\"border: 1px solid red; \"  width=12%><b>" + jednoskaRejNowa.zJednRejStarej.Sum(x => x.Pow_Przed).ToString("F4", CultureInfo.InvariantCulture) + "</b></th><th style =\"border: 1px solid red; color: green;\"  width=12%><b>" + SumaWartosciPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</b></th><th style =\"border: 1px solid red;\"  width=14%></th><th style =\"border: 1px solid red;\"  width=12%></th><th style =\"border: 1px solid red;\"  width=12%><b>" + jednoskaRejNowa.Dzialki_Nowe.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture) + "</b></th><th style =\"border: 1px solid red; color: green;\"  width=12%><b>" + SumaWartosciPo.ToString("F2", CultureInfo.InvariantCulture) + "</b></th><th style =\"border: 1px solid red;\"  width=14%></th></tr></table>");

            dokHTML.AppendLine("<br/>");

            string szerTabPodBilansem = jednoskaRejNowa.Odcht == true ? "288" : "0.36%";
            // Tabelka czarna pod bilansem
            dokHTML.AppendLine("<table width = " + szerTabPodBilansem + " style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; \">");
            dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka dopuszczalna 3%:</b></td><td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + Decimal.Round(SumaWartosciPrzed * 0.03M, 2).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
            decimal odchylkaFaktyczna = SumaWartosciPo - SumaWartosciPrzed;
            dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka faktyczna:</b></td><td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + odchylkaFaktyczna.ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
            if (jednoskaRejNowa.Odcht == false)
            {
                if (odchylkaFaktyczna < 0)
                {
                    dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Winien otrzymać: </b></td><td style =\"border: 1px solid black; text-align: center; color: red;\" width=33.34%><b>" + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
                }
                else
                {
                    dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Winien zapłacić: </b></td><td style =\"border: 1px solid black; text-align: center; color: red;\" width=33.34%><b>" + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
                }
            }
            else
            {
                dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka techniczna</b></td><td style =\"border: 1px solid black; text-align: center; \" width=33.34%><b>-.--</b></td></tr>");
            }

            dokHTML.AppendLine("</table>");
            //dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PodzialSekcjiNaStronieNieparzystej);

            return dokHTML.ToString();
        }
    }
}
