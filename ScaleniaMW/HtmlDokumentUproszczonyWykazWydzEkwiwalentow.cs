using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    static class HtmlDokumentUproszczonyWykazWydzEkwiwalentow
    {
        //public const string HTML_PodzialSekcjiNaStronieNieparzystej = "<span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";mso-fareast-font-family: \"Times New Roman\";mso-ansi-language:PL;mso-fareast-language:PL;mso-bidi-language: AR-SA'><br clear=all style='page-break-before:right;mso-break-type:section-break'></span>";
        public const string HTML_PodzialNowaStrona = "<br clear=all style = 'mso-special-character:line-break;page-break-before:always'> ";
        public const string HTML_PodzialSekcjiNaStronieNieparzystej = "<br clear=all style='page-break-before:right;mso-break-type:section-break'>";
        public const string HTML_PoczatekWykazyWydzEkwiwalentow = "<!DOCTYPE html> <html lang=\"pl\"> <head>  " +
            "<meta charset=\"windows-1250\"> " +
            "<meta name=Generator content=\"Microsoft Word 12 (filtered)\"> " +
            "<style> body { font-family: \"Arial Narrow\", Arial, sans-serif; font-style: italic; font-size: 11pt; }" +
            "</style> " +
            "</head> " +
            "<body>";

        public const string HTML_ZakonczenieWykazuWydzEkwiw = "</body></html>";

        public static string WierszWlasciciel(string NazwaWlasciciela = "", string AdresWlasciciela = "", string Udzial = "", int gruboscPodkreslenia = 1, bool czyWyrzucicOstatniePodkreslenie = false)
        {
            if (czyWyrzucicOstatniePodkreslenie)
            {
                return "<tr style=\"border: none;\">" +
                "<td style=\"border: none; width:90%;\"><span>" + NazwaWlasciciela + "</span><br /><span>" + AdresWlasciciela +
                " </span></td>" +
                "<td style=\" border: none; text-align: center; width:10%;\"><span>" + Udzial + "</span> </td></tr>";
            }

            return "<tr style=\"border: none;\">" +
                 "<td style=\"border-bottom: " + gruboscPodkreslenia + "px solid black; width:90%;\"><span>" + NazwaWlasciciela + "</span><br /><span>" + AdresWlasciciela +
                 " </span></td>" +
                 "<td style=\"border-bottom: " + gruboscPodkreslenia + "px solid black; text-align: center; width:10%;\"><span>" + Udzial + "</span> </td></tr>";
        }


        public static String GenerujWykazWE(JR_Nowa JednoskaRejNowa)
        {
            string kolorZakreslacza = "#ffff40";
            string szerTabeli = "100%";

            decimal SumaWartosciPrzed = JednoskaRejNowa.zJednRejStarej.Sum(x => x.WrtJednPrzed);
            decimal SumaWartosciPo = JednoskaRejNowa.Dzialki_Nowe.Sum(x => x.Wartosc);
            string sumaPowierzchniDzialekNowych = JednoskaRejNowa.Dzialki_Nowe.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture);
            string sumaPowierzchniDzialekPrzedScaleniem = JednoskaRejNowa.zJednRejStarej.Sum(x => x.Pow_Przed).ToString("F4", CultureInfo.InvariantCulture);

            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine("<div style=\"text-align: right;\"><b> <span style=\"color: red\">NUMER GOSPODARSTWA &nbsp;</span>  <span style = \"color: blue; text-decoration: underline; font-size: 14pt\">" + JednoskaRejNowa.IjrPo + "</span></b></div>");


            dokHTML.AppendLine("<table style=\" width=\"" + szerTabeli + "\" \"><tr>");
            dokHTML.AppendLine("<td style=\" vertical-align: top;  \">Obręb:</td>");


            bool brakStanuPo = JednoskaRejNowa.Dzialki_Nowe.Count > JednoskaRejNowa.Dzialki_Nowe.FindAll(dzialka => dzialka.NrDz[0] == '0').Count ? false : true;

            if (!brakStanuPo)
            {
                dokHTML.AppendLine("<td style=\" width:100%; \"> <b style = \"color: blue;\">");

                bool firstElem = true;
                foreach (var dzialka in JednoskaRejNowa.Dzialki_Nowe.Select(x => new { nrObr = x.NrObr, nazwaObr = x.NazwaObrebu }).Distinct().OrderBy(x => x.nrObr))
                {
                    if (!firstElem) dokHTML.AppendLine("<br />");
                    dokHTML.AppendLine(dzialka.nrObr + "&nbsp;" + dzialka.nazwaObr);
                    firstElem = false;
                }
            }
            else
            {
                dokHTML.AppendLine("<td style=\" width:100%; \"> <b>");
            }

            if (brakStanuPo) dokHTML.Append("-");

            dokHTML.AppendLine("</b></td>");

            // dodanie tekstu zbywa całe gospodarstwo
            string tekstZbywaCaleGosp = "<td style =\" text-align:right; color: red; display: inline-block; \"><span>Zbywa&nbsp;całe&nbsp;gospodarstwo<span></td>";

            if (JednoskaRejNowa.Dzialki_Nowe.Count < 1)
            {
                dokHTML.AppendLine(tekstZbywaCaleGosp);
            }
            else if (JednoskaRejNowa.Dzialki_Nowe[0].NrDz == "0")
            {
                dokHTML.AppendLine(tekstZbywaCaleGosp);
                JednoskaRejNowa.Dzialki_Nowe.RemoveAt(0);
            }




            dokHTML.AppendLine("</tr></table>");
            //jednostka rejestrowa po lewej
            string nrJednostkiRejestrowejPrzed;
            if (Properties.Settings.Default.czyWziacNrJednRejZNkrPo == true)
            {
                nrJednostkiRejestrowejPrzed = JednoskaRejNowa.Nkr == 0 ? "" : JednoskaRejNowa.Nkr.ToString();
            }
            else
            {
                nrJednostkiRejestrowejPrzed = JednoskaRejNowa.Uwaga;
            }

            // gdy firstRowObreb == true wynika że brak jest stanu po scaleniu.
            if (brakStanuPo) nrJednostkiRejestrowejPrzed = "<b>-</b>";
            dokHTML.AppendLine("<div><span>Numer jednostki rejestrowej: " + nrJednostkiRejestrowejPrzed + "<br /></div>");

            //nagłówek właściciele i władający
            //dokHTML.AppendLine("<div style=\"border-bottom: 2px solid black; width:90%;\"><br /><span style=\"margin-bottom: 0; padding: 0; \" >Właściciele i władający</span></div>");
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

            // uwaga 
            //dokHTML.AppendLine("<div style=\"color: red\"><i>" + JednoskaRejNowa.Uwaga + "</i></div>");

            //nagłówek nad tabelą
            dokHTML.AppendLine("<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>Ekwiwalent należny wg. udziału w pozycjach rejestrowych</i></u></b></span></div>");

            //tabela ekwiwalentu [obr][nazwaobr][nrjedn][udzial][powudzialu][naleznosc]
            dokHTML.AppendLine("<table  width=" + szerTabeli + " style =\"border: 2px solid black; border-collapse: collapse; font-size: 9pt \">");
            dokHTML.AppendLine("<tr> " +
                "<th style=\"border: 1px solid black; \"> Obręb </th>" +
                "<th style=\"border: 1px solid black; \"> Nazwa obrębu </th>" +
                "<th style=\"border: 1px solid black; \"> Nr jednostki </th>" +
                "<th style=\"border: 1px solid black; \"> Udział </th>" +
                "<th style=\"border: 1px solid black; \"> Powierzchnia </th>" +
                "<th style=\"border: 1px solid black; color: green; \"> Należność </th> " +
                "</tr>");


            foreach (var zeStarejJedn in JednoskaRejNowa.zJednRejStarej.OrderBy(x => x.NrObr))
            {
                dokHTML.AppendLine("<tr> <td style=\"border: 1px solid black; margin-left: 5px;  \">" + zeStarejJedn.NrObr + "</td>" +
                    "<td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.NazwaObrebu + "</td>" +
                    "<td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.Ijr_Przed +
                    "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.Ud_Z_Jrs + "</td>" +
                    "<td style=\"border: 1px solid black; margin-right: 5px;text-align: right; \">" + zeStarejJedn.Pow_Przed.ToString("F4", CultureInfo.InvariantCulture) + "</td>" +
                    "<td style=\"border: 1px solid black; color: green; text-align: right; margin-right: 5px; \">" + zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</td> </tr>");
            }
            //podsumowanie tabeli udzialow ze starych jednostek
            dokHTML.AppendLine("<td colspan=\"4\" style=\"text-align: right; border: none;  margin-right:2px;\"><span>Powierzchnia/<span style=\"color: green;\">Wartość przed scaleniem:</span></span></td><td style=\"border: 1px solid black; text-align: right; margin-right: 5px;\" > <b>" + sumaPowierzchniDzialekPrzedScaleniem + "</b></td><td  style=\"border: 1px solid black; color: green;  text-align: right; margin-right: 5px;\"><b>" + SumaWartosciPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</b></td>");
            dokHTML.AppendLine("</table>");


            ///////////////////////////////////////////////////////


            //tytuł tabeli Ekwiwalent zaprojektowany
            dokHTML.AppendLine("<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>Ekwiwalent zaprojektowany</i></u></b></span></div>");

            //tabela Ekwiwalentów nagłówki
            dokHTML.AppendLine("<table style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; width: " + szerTabeli + "; \">");
            dokHTML.AppendLine("<tr><th style =\"border: 1px solid black; width: 12%;\"><b>Obręb</b></th><th style =\"border: 1px solid black; width: 12%;\"  ><b>Działka</b></th><th style =\"border: 1px solid black; width: 12%;\"><b>Powierzchnia</b></th><th style =\"border: 1px solid black; width: 12%;\"><b>Wartość</b></th><th style =\"border: 1px solid black; width: 14%;\"><b>KW</b></th></tr>");

            // treść zasadnicza 
            int liczbaIteracjiPetli = JednoskaRejNowa.Dzialki_Nowe.Count;
            for (int i = 0; i < liczbaIteracjiPetli; i++)
            {

                string nrdzPo = JednoskaRejNowa.Dzialki_Nowe[i].NrDz;
                string powDzialkiPo = JednoskaRejNowa.Dzialki_Nowe[i].PowDz.ToString("F4", CultureInfo.InvariantCulture);
                string wartPo = JednoskaRejNowa.Dzialki_Nowe[i].Wartosc.ToString("F2", CultureInfo.InvariantCulture);
                string kwPo = JednoskaRejNowa.Dzialki_Nowe[i].KW;
                string nrobr = JednoskaRejNowa.Dzialki_Nowe[i].NrObr.ToString();

                // Ekwiwalent zaprojektowany
                dokHTML.AppendLine("<tr>" +
                    "<td style =\"border: 1px solid black; text-align: center; \">" + nrobr +
                    "</td><td style =\"border: 1px solid black; text-align: center;  color: red; \">" + nrdzPo +
                    "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; \">" + powDzialkiPo +
                    "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + wartPo +
                    "</td><td style =\"border: 1px solid black; text-align: center; \">" + kwPo +
                    "</td>" +
                    "</tr>");
            }


            dokHTML.AppendLine("<tr><td style =\"border: none; text-align: center; \"></td>" +
                "<td style =\"border: none; text-align: right; margin-right:5px; \">" + "<i><b>Razem:</b></i>" + "</td>" +
                "<td style =\"border: 1px solid black; text-align: right; margin-right:5px; \"><b>" + sumaPowierzchniDzialekNowych + "</b></td>" +
                "<td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" ><b>" + SumaWartosciPo.ToString("F2", CultureInfo.InvariantCulture) + "</b></td>" +
                "<td style =\"border: none; text-align: center; \"></td></tr>");
            dokHTML.AppendLine("</table>");
            dokHTML.AppendLine("<br />");


            // BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW
            dokHTML.AppendLine("<div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"color: red\"><b><u>BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW</u></b></span></div>");
            // Tabelka czerwona BILANSU

            dokHTML.AppendLine("<table style=\"border: 2px solid red; border-collapse: collapse; font-size: 9pt; width: " + szerTabeli + ";\">");
            dokHTML.AppendLine("<tr>");
            dokHTML.AppendLine("<th colspan=\"1\" style=\"border: 1px solid red;\"><i><b></b></i></th>");
            dokHTML.AppendLine("<th colspan=\"2\" style=\"border: 1px solid red;\"><i><b>Należny</b></i></th>");
            dokHTML.AppendLine("<th colspan=\"2\" style=\"border: 1px solid red;\"><i><b>Zaprojektowany</b></i></th>");
            dokHTML.AppendLine("</tr>");
            dokHTML.AppendLine("<tr>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red; color: red;\"></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red;\"><i><b>Pow. ewid.</b></i></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red;\"><i><b>Wartość</b></i></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red;\"><i><b>Powierzchnia</b></i></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red;\"><i><b>Wartość</b></i></th>");
            dokHTML.AppendLine("</tr>");
            dokHTML.AppendLine("<tr>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red; color: red; width: 12%;\"><b>RAZEM GOSP.:</b></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red; width: 12%;\"><b>" + sumaPowierzchniDzialekPrzedScaleniem + "</b></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red; color: green; width: 12%;\"><b>" + SumaWartosciPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</b></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red;  width: 12%;\"><b>" + sumaPowierzchniDzialekNowych + "</b></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid red; color: green; width: 12%;\"><b>" + SumaWartosciPo.ToString("F2", CultureInfo.InvariantCulture) + "</b></th>");
            dokHTML.AppendLine("</tr>");
            dokHTML.AppendLine("</table>");





            dokHTML.AppendLine("<br/>");

            // Tabelka czarna pod bilansem
            // (int)(szerTabeli * 0.36D)

            string szerTabPodBilansem = JednoskaRejNowa.Odcht == true ? "288" : "0.36%";




            dokHTML.AppendLine("<table width = " + szerTabPodBilansem + " style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; \">");
            dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b> Odchyłka dopuszczalna ±3%:</b></td><td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + Decimal.Round(SumaWartosciPrzed * 0.03M, 2).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");

            decimal odchylkaFaktyczna = SumaWartosciPo - SumaWartosciPrzed;
            dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b> Odchyłka faktyczna:</b></td><td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + odchylkaFaktyczna.ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");

            if (JednoskaRejNowa.Odcht == false)
            {
                if (odchylkaFaktyczna < 0)
                {
                    dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b> Winien otrzymać: </b></td><td style =\"border: 1px solid black; text-align: center; color: green;\" width=33.34%><b>" + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
                }
                else
                {
                    dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b> Winien zapłacić: </b></td><td style =\"border: 1px solid black; text-align: center; color: red;\" width=33.34%><b>" + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
                }
            }
            else
            {
                dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b> Dopłata za ekwiwalent:</b></td><td style =\"border: 1px solid black; text-align: center; \" width=33.34%><b>0.00</b></td></tr>");
                dokHTML.AppendLine("<tr><td  colspan=\"2\" style =\"border: 1px solid black; color: green; text-align: center; \" width=66.66%> Nie naliczono dopłaty do ekwiwalentu z przyczyn technicznych </td></tr>");
            }

            dokHTML.AppendLine("</table>");

            dokHTML.AppendLine("<br/>");

            // tabelka oświadczenia uczestnika i omówienie zastrzeżeń
            int wysWierszaTabeliPX = 20;
            dokHTML.AppendLine("<table style=\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; width: " + szerTabeli + ";\">");
            dokHTML.AppendLine("<tr>");
            dokHTML.AppendLine("<th style=\"border: 1px solid; width: 50%; font-size: 10px;\"><i>Oświadczenie uczestnika scalenia w sprawie projektu wstępnego, " +
                "treść ewentualnych zastrzeżeń, data i odpis uczestnika scalenia.</i></th>");
            dokHTML.AppendLine("<th style=\"border: 1px solid; width: 50%; font-size: 10px;\"><i><b>Omówienie zastrzeżeń, proponowane zmiany <br> data i podpis geodety.</b></i></th>");
            dokHTML.AppendLine("</tr>");


            //generowanie pustych wierszy w tabeli
            for (int i = 0; i < 10; i++)
            {
                dokHTML.AppendLine("<tr style=\"border: 1px solid black; height: " + wysWierszaTabeliPX + "px;\">");
                dokHTML.AppendLine("<td style=\"border-bottom: 1px solid #8c8c8c; border-right: 1px solid #000000;\"></td>");
                dokHTML.AppendLine("<td style=\"border-bottom: 1px solid #8c8c8c;\"></td>");
                dokHTML.AppendLine("</tr>");
            }

            dokHTML.AppendLine("</table>");

            //dokHTML.AppendLine(HTML_PodzialSekcjiNaStronieNieparzystej);
            return dokHTML.ToString();
        }

        public static string GenerujTabeleWlascicieliPRZED(ZJednRejStarej WlascicielePrzed, int szerTabeli)
        {
            bool czyWyrzucicOstatniePodkreslenie = false;
            StringBuilder dokHTML = new StringBuilder();
            int licznikMalzenskiPrzed = 0;
            dokHTML.AppendLine("<table width=" + szerTabeli + ">");
            foreach (var wlascicelPrzed in WlascicielePrzed.Wlasciciele)
            {
                if (wlascicelPrzed.Equals(WlascicielePrzed.Wlasciciele.Last()))
                {
                    czyWyrzucicOstatniePodkreslenie = true;
                }
                if (wlascicelPrzed.IdMalzenstwa > 0 && licznikMalzenskiPrzed == 0)
                {
                    licznikMalzenskiPrzed++;
                    dokHTML.AppendLine(WierszWlasciciel("małżeństwo", Udzial: wlascicelPrzed.Udzial));
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed.NazwaWlasciciela, wlascicelPrzed.Adres));
                }
                else
                if (wlascicelPrzed.IdMalzenstwa > 0 && licznikMalzenskiPrzed > 0)
                {
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed.NazwaWlasciciela, wlascicelPrzed.Adres, gruboscPodkreslenia: 2, czyWyrzucicOstatniePodkreslenie: czyWyrzucicOstatniePodkreslenie));
                    licznikMalzenskiPrzed = 0;
                }
                else
                {
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed.NazwaWlasciciela, wlascicelPrzed.Adres, wlascicelPrzed.Udzial, 2, czyWyrzucicOstatniePodkreslenie: czyWyrzucicOstatniePodkreslenie));
                }
            }

            dokHTML.AppendLine("</table>");

            return dokHTML.ToString();
        }

        public static bool CzyGenerowacWlascicieliZStarychJEdnostek(JR_Nowa JednoskaRejNowa)
        {

            int ilczbWlasNowych = JednoskaRejNowa.Wlasciciele.Count;
            List<int> liczbaWlascicieliDlaJednostek = new List<int>();
            JednoskaRejNowa.zJednRejStarej.ForEach(x => liczbaWlascicieliDlaJednostek.Add(x.Wlasciciele.Count));
            //  bool czyLiczbaWlascicieliTakaSama = true;
            // int WIluJednJestTyleSamoWlascicieli
            foreach (var lplWlasc in liczbaWlascicieliDlaJednostek)
            {
                if (lplWlasc != ilczbWlasNowych)
                {
                    // Console.WriteLine("LP WLAS: " + lplWlasc + "lP PO:" + ilczbWlasNowych);
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
    }
}
