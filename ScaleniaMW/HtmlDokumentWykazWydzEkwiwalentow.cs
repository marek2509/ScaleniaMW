using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    static class HtmlDokumentWykazWydzEkwiwalentow
    {


        //table { border: 2px solid black; border-collapse: collapse; width: 100%;}  th{ border-bottom:  2px solid black; border-left: 1px solid black; border-right: 1px solid black;	} tr, td{  border: 1px solid black; } #tableNoneBorder{ width: 100%; border: none; border-collapse: collapse; }
        public const string HTML_PodzialSekcjiNaStronieNieparzystej = "<span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";mso-fareast-font-family: \"Times New Roman\";mso-ansi-language:PL;mso-fareast-language:PL;mso-bidi-language: AR-SA'><br clear=all style='page-break-before:right;mso-break-type:section-break'></span>";
        public const string HTML_PoczatekWykazyWydzEkwiwalentow = "<html><head><meta content=\"text/html; charset=windows-1250\"> <meta name=Generator content=\"Microsoft Word 12 (filtered)\"> <style> body { font-family: \"Arial Narrow\", Arial, sans-serif; font-size: 11pt; }</style> </head> <body lang=PL> <div class=WordSection1>";
        public const string HTML_ZakonczenieWykazuWydzEkwiw = "</div></body></html>";

        public static string WierszWlasciciel(string NazwaWlasciciela = "", string AdresWlasciciela = "", string Udzial = "", int gruboscPodkreslenia = 1, bool czyWyrzucicOstatniePodkreslenie = false)
        {
            if (czyWyrzucicOstatniePodkreslenie)
            {
                return "<tr style=\"border: none;\">" +
                "<td style=\"border: none; width:90%;\"><span>" + NazwaWlasciciela + "</span><br /><span>" + AdresWlasciciela +
                " </span></td>" +
                "<td style=\" border: none; text -align: center; width:10%;\"><span>" + Udzial + "</span> </td></tr>";
            }

            return "<tr style=\"border: none;\">" +
                 "<td style=\"border-bottom: " + gruboscPodkreslenia + "px solid black; width:90%;\"><span>" + NazwaWlasciciela + "</span><br /><span>" + AdresWlasciciela +
                 " </span></td>" +
                 "<td style=\"border-bottom: " + gruboscPodkreslenia + "px solid black; text-align: center; width:10%;\"><span>" + Udzial + "</span> </td></tr>";

         
            /*
        return "<div style=\"border-bottom: 1px solid black;\"><div style = \"display: inline; width: 50%;\"><span>" + NazwaWlasciciela + "</span><br/><span>" +
                    AdresWlasciciela + "</span></div> <div style=\"display: inline; width: 50%; text-align: right;\"><span>" + Udzial+ "</span></div></div>";     */
        }

        //font-size: 16px
        public static String GenerujWykazWE(JR_Nowa JednoskaRejNowa)
        {
            string kolorZakreslacza = "#ffff40";
            int szerTabeli = 630;
            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine("<div style=\"text-align: right;\"><b> <span style=\"color: red\">NUMER GOSPODASTWA &nbsp;</span>  <span style = \"color: blue; text-decoration: underline; font-size: 14pt\">" + JednoskaRejNowa.IjrPo + "</span></b></div>");
            dokHTML.AppendLine("<div><span>Obręb:&nbsp;<b  style = \"color: blue; \">" + JednoskaRejNowa.NrObr + "&nbsp;" + JednoskaRejNowa.NazwaObrebu + "</b></span></div>");
            dokHTML.AppendLine("<div style=\"border-bottom: 2px solid black;\"><span>Numer jednostki rejestrowej " + JednoskaRejNowa.Nkr +
                "</span><br /><span style=\"margin-bottom: 0; padding: 0; \" >Właściciele i władający</span></div>");

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
            dokHTML.AppendLine("<div style=\"color: red\"><i>" + JednoskaRejNowa.Uwaga + "</i></div>");

            //nagłówek nad tabelą
            dokHTML.AppendLine("<br /> <div style=\"text-align: center;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>Ekwiwalent należny wg. udziału w pozycjach rejestrowych</i></u></b></span></div>");
            //tabela ekwiwalentu [obr][nazwaobr][nrjedn][udzial][powudzialu][naleznosc]
            dokHTML.AppendLine("<table  width=" + szerTabeli + " style =\"border: 2px solid black; border-collapse: collapse; font-size: 9pt \">");

            dokHTML.AppendLine("<tr> <th style=\"border: 1px solid black; \"> Obręb </th><th style=\"border: 1px solid black; \"> Nazwa obrębu </th><th style=\"border: 1px solid black; \"> Nr jednostki </th><th style=\"border: 1px solid black; \"> Udział </th><th style=\"border: 1px solid black; \"> Powierzchnia </th><th style=\"border: 1px solid black; color: green; \"> Należność </th> </tr>");


            foreach (var zeStarejJedn in JednoskaRejNowa.zJednRejStarej)
            {
                dokHTML.AppendLine("<tr> <td style=\"border: 1px solid black; margin-left: 5px;  \">" + zeStarejJedn.NrObr + "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.NazwaObrebu + "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.Ijr_Przed +
                    "</td><td style=\"border: 1px solid black; margin-left: 5px; \">" + zeStarejJedn.Ud_Z_Jrs + "</td><td style=\"border: 1px solid black; margin-right: 5px;text-align: right; \">" + zeStarejJedn.Pow_Przed.ToString("F4", CultureInfo.InvariantCulture) + "</td><td style=\"border: 1px solid black; color: green; text-align: right; margin-right: 5px; \">" + zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</td> </tr>");
            }
            //podsumowanie tabeli udzialow ze starych jednostek
            dokHTML.AppendLine("<td colspan=\"4\" style=\"text-align: right; border: none; \"><span style=\"margin-right:5px; \">Powierzchnia/<span style=\"color: green; \">Wartość przed scaleniem</span></span></td><td style=\"border: 1px solid black; text-align: right; margin-right: 5px;\" > <b>" + JednoskaRejNowa.zJednRejStarej.Sum(x => x.Pow_Przed).ToString("F4", CultureInfo.InvariantCulture) + "</b></td><td  style=\"border: 1px solid black; color: green;  text-align: right; margin-right: 5px;\"><b>" + JednoskaRejNowa.zJednRejStarej.Sum(x => x.WrtJednPrzed).ToString("F2", CultureInfo.InvariantCulture) + "</b></td>");
            dokHTML.AppendLine("</table>");

            //tytuł tabeli Ekwiwalent nalezny / zaprjektowany
            dokHTML.AppendLine("<br /> <div style=\"text-align: center;\"> <span style=\"background-color:" + kolorZakreslacza + "\"><b><u><i>Ekwiwalent należny/zaprojektowany</i></u></b></span></div>");
            dokHTML.AppendLine("<div><span>Obręb:&nbsp;<b  style = \"color: blue; \">" + JednoskaRejNowa.NrObr + "&nbsp;" + JednoskaRejNowa.NazwaObrebu + "</b></span></div>");
            dokHTML.AppendLine("<div style=\"border-bottom: 2px solid black;\"><span>Numer jednostki rejestrowej " + JednoskaRejNowa.Nkr +
                "</span><br /><span style=\"margin-bottom: 0; padding: 0; \" >Właściciele i władający</span></div>");

            foreach (var jednostkaStara in JednoskaRejNowa.zJednRejStarej)
            {

                dokHTML.Append( GenerujTabeleWlascicieliPRZED(jednostkaStara, szerTabeli));

                //tabela Ekwiwalentów nagłówki
                dokHTML.AppendLine("<table  width=" + szerTabeli + " style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; \"><tr><th colspan=\"8\"><b>Ekwiwalenty</b></th></tr>");
                dokHTML.AppendLine("<tr><th style =\"border: 1px solid black;\"  colspan=\"4\" width=50%><b>Należny</b></th><th style =\"border: 1px solid black;\"  colspan=\"4\"  width=50%><b>Zaprojektowany</b></th></tr>");
                dokHTML.AppendLine("<tr><th style =\"border: 1px solid black;\" width=9%><b>Działka</b></th><th style =\"border: 1px solid black;\"  width=13%><b>Pow. ewid.</b></th><th style =\"border: 1px solid black;\"  width=13%><b>Wartość</b></th><th style =\"border: 1px solid black;\"  width=15%><b>KW</b></th><th style =\"border: 1px solid black;\"  width=9%><b>Działka</b></th><th style =\"border: 1px solid black;\"  width=13%><b>Pow.</b></th><th style =\"border: 1px solid black;\"  width=13%><b>Wartość</b></th><th style =\"border: 1px solid black;\"  width=15%><b>KW</b></th></tr>");

                // treść zasadnicza 


                foreach (var dzialkaPrzed in jednostkaStara.Dzialki)
                {
                    dokHTML.AppendLine("<tr><td style =\"border: 1px solid black;\"  margin-left:5px; >" + dzialkaPrzed.NrDz + "</td><td style =\"border: 1px solid black;\" text-align: right; margin-right:5px;>" + dzialkaPrzed.PowDz.ToString("F4", CultureInfo.InvariantCulture) + "</td><td style =\"border: 1px solid black; text-align: right; margin-right:5px; color: green; \" >" + dzialkaPrzed.Wartosc.ToString("F2", CultureInfo.InvariantCulture) + "</td><td style =\"border: 1px solid black;\"  margin-left:5px;>" + dzialkaPrzed.KW + "</td><td style =\"border: 1px solid black;\" ></td><td style =\"border: 1px solid black;\" ></td><td style =\"border: 1px solid black;\" ></td><td style =\"border: 1px solid black;\" ></td></tr>");
                }


                dokHTML.AppendLine("</table>");
            }





            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PodzialSekcjiNaStronieNieparzystej);
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
                if (wlascicelPrzed.Equals(WlascicielePrzed.Wlasciciele.Last())){
                    czyWyrzucicOstatniePodkreslenie = true;
                }
                if (wlascicelPrzed.IdMalzenstwa > 0 && licznikMalzenskiPrzed == 0)
                {
                    licznikMalzenskiPrzed++;
                    dokHTML.AppendLine(WierszWlasciciel("małżeństwo", Udzial: wlascicelPrzed.Udzial));
                    dokHTML.AppendLine(WierszWlasciciel(wlascicelPrzed.NazwaWlasciciela, wlascicelPrzed.Adres));
                }
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

    }


}
