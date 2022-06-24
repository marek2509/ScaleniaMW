using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public abstract class HtmlDokument
    {
        // MOJE KLASY CSS
        static (string nameClass, string value) Css_tabelaCzarna = ("tabelaCzarna", "{width: 100%; border: 1px solid black; border-collapse: collapse; font-size: 9pt;}");
        static (string nameClass, string value) Css_borderBl = ("borderBl", "{border: 1px solid black;  text-align: center;}");
        static (string nameClass, string value) CSS_borderBl_redText = ("borderBl_redText", "{border: 1px solid black; text-align: center; color:red;}");
        static (string nameClass, string value) Css_greenText = ("greenText", "{color: green;}");
        static (string nameClass, string value) Css_borderBl_ml5 = ("borderBl_ml5", "{border: 1px solid black; margin-left: 5px;}");
        static (string nameClass, string value) Css_borderBl_mr5 = ("borderBl_mr5", "{border: 1px solid black; margin-right: 5px; text-align: right;}");
        static (string nameClass, string value) Css_borderBl_ml5_greenText = ("borderBl_ml5_greenText", "{border: 1px solid black; margin-left: 5px; color: green;}");
        static (string nameClass, string value) Css_borderBl_mr5_greenText = ("borderBl_mr5_greenText", "{border: 1px solid black; margin-right: 5px; text-align: right; color: green;}");
        static (string nameClass, string value) Css_borderBl_ml5_redText = ("borderBl_ml5_redText", "{border: 1px solid black; margin-left: 5px; color: red;}");
        static (string nameClass, string value) Css_borderBl_mr5_redText = ("borderBl_mr5_redText", "{border: 1px solid black; margin-right: 5px; text-align: right; color: red;}");

        static (string nameClass, string value) Css_tabelaCzerwona = ("tabelaCzerwona", "{width: 100%; border: 2px solid red; border-collapse: collapse; font-size: 9pt;}");
        static (string nameClass, string value) Css_borderRed = ("borderRed", "{border: 1px solid red;  text-align: center;}");
        static (string nameClass, string value) Css_borderRedTxtGreen = ("borderRedTxtGreen", "{border: 1px solid red;  text-align: center; color: green;}");
        static (string nameClass, string value) Css_TabelkaPodBilansem = ("TabelkaPodBilansem", "{border: 1px solid black; border-collapse: collapse; font-size: 9pt; width: 288}");


        public static string GetClassesNameAndContent(params (string nameClass, string value)[] krotka)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < krotka.Length; i++)
            {
                sb.AppendLine("." + krotka[i].nameClass + " " + krotka[i].value);
            }
            return sb.ToString();
        }

        public const string HTML_ZakonczenieWykazuWydzEkwiw = "</body></html>";
        public const string HTML_PodzialNowaStrona = "<br clear=all style = 'mso-special-character:line-break;page-break-before:always'> ";
        public const string HTML_PodzialSekcjiNaStronieNieparzystej = "<br clear=all style='page-break-before:right;mso-break-type:section-break'>";
        public static string HTML_PoczatekWykazyWydzEkwiwalentow()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html> <html lang=\"pl\"> <head>  ");
            sb.AppendLine("<meta charset=\"windows-1250\"> ");
            sb.AppendLine("<meta name=Generator content=\"Microsoft Word 12 (filtered)\"> ");
            sb.AppendLine("<style> ");
            sb.AppendLine("body { font-family: \"Arial Narrow\", Arial, sans-serif; font-style: italic; font-size: 11pt;} ");
            sb.AppendLine(GetClassesNameAndContent(Css_tabelaCzarna, Css_borderBl, CSS_borderBl_redText, Css_greenText, Css_borderBl_ml5, Css_borderBl_mr5,
                            Css_borderBl_ml5_greenText, Css_borderBl_mr5_greenText, Css_borderBl_ml5_redText, Css_borderBl_mr5_redText, Css_tabelaCzerwona, 
                            Css_borderRed, Css_borderRedTxtGreen, Css_TabelkaPodBilansem));
            sb.AppendLine("</style> ");
            sb.AppendLine("</head> ");
            sb.AppendLine("<body>");
            return sb.ToString();
        }



        public static string Html_SeparatorPoziomy(int grubosc = 2)
        {
            string gruboscLini = grubosc + "px";

            //return "<div style=\"height:" + gruboscLini + "; width:100%; background-color: black;\"></div>";
            return "<div style=\"width: 100%; background: black; font-size: " + gruboscLini + ";\">&nbsp </div>";
        }

        public static string WierszMalzenstwo(string Udzial = "", int gruboscPodkreslenia = 2, string tekstMalzenstwo = "małżeństwo")
        {
            string grubosc = gruboscPodkreslenia > 0 ? gruboscPodkreslenia + "px solid black" : "none";
            return "<tr style=\"border: none;\">" +
                 "<td style=\"border-bottom:" + grubosc + "; width:90%;\"><span>" + tekstMalzenstwo + "</span><br /><span></span></td>" +
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
                 "<td style=\"border-bottom: " + gruboscPodkresl + "; text-align: center; width:10%;\"><span>" + wl.Symbol_Wladania + " " + wl.Udzial + "</span> </td></tr>";
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

        public static string HTML_TabelaWlascicieleIWladajacy(List<Wlasciciel> wlasciciele, bool usunOstatniePodkreslenie = false)
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
            return HTML_TabelaWlascicieleIWladajacy(listaWlascicieli as List<Wlasciciel>, true);
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
                            "<th class=\"borderBl\"> Obręb </th>" +
                            "<th class=\"borderBl\"> Nazwa obrębu </th>" +
                            "<th class=\"borderBl\"> Nr jednostki </th>" +
                            "<th class=\"borderBl\"> Udział </th>" +
                            "<th class=\"borderBl\"> Powierzchnia </th>" +
                            "<th class=\"greenText borderBl\"> Należność </th> " +
                          "</tr>");

            foreach (var zeStarejJedn in JednoskaRejNowa.zJednRejStarej)
            {
                sb.AppendLine("<tr> " +
                                "<td class=\"borderBl_ml5\">" + zeStarejJedn.NrObr + "</td>" +
                                "<td class=\"borderBl_ml5\">" + zeStarejJedn.NazwaObrebu + "</td>" +
                                "<td class=\"borderBl_ml5\">" + zeStarejJedn.Ijr_Przed + "</td>" +
                                "<td class=\"borderBl_ml5\">" + zeStarejJedn.Ud_Z_Jrs + "</td>" +
                                "<td class=\"borderBl_mr5\">" + zeStarejJedn.Pow_Przed.ToString("F4", CultureInfo.InvariantCulture) + "</td>" +
                                "<td class=\"borderBl_mr5 greenText\">" + zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</td> " +
                              "</tr>");
            }

            //podsumowanie tabeli udzialow ze starych jednostek
            sb.AppendLine("<td colspan=\"4\" style=\"text-align: right; border: none;  margin-right:2px; \">" +
                                 "<span>Powierzchnia/</span><span class=\"greenText\">Wartość przed scaleniem:</span></td>" +
                             "<td class=\"borderBl_mr5\"><b>" + JednoskaRejNowa.SumaPowJednostekPrzed().ToString("F4", CultureInfo.InvariantCulture) + "</b></td>" +
                             "<td class=\"borderBl_mr5_greenText\"><b>" + JednoskaRejNowa.SumaWartJednostekPrzed().ToString("F2", CultureInfo.InvariantCulture) + "</b></td>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        static string HTML_TabelaEkwiwalentuNaleznegoBezPEW(JR_Nowa JednoskaRejNowa)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"tabelaCzarna\">");
            //"<th class=\"borderBl\"> Obręb </th>" +
            //"<th class=\"borderBl\"> Nazwa obrębu </th>" +
            //"<th class=\"borderBl\"> Nr jednostki </th>" +
            //"<th class=\"borderBl\"> Udział </th>" +
            //"<th class=\"greenText borderBl\"> Należność </th> " +
            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Obręb"));
            sb.AppendLine(Tbl_th("Nazwa obrębu"));
            sb.AppendLine(Tbl_th("Nr jednostki"));
            sb.AppendLine(Tbl_th("Udział"));
            sb.AppendLine(Tbl_th("Ekwiwalent należny", style: "color: green;"));
            sb.AppendLine("</tr>");


            foreach (var zeStarejJedn in JednoskaRejNowa.zJednRejStarej)
            {
                //"<td class=\"borderBl_ml5\">" + zeStarejJedn.NrObr + "</td>" +
                //"<td class=\"borderBl_ml5\">" + zeStarejJedn.NazwaObrebu + "</td>" +
                //"<td class=\"borderBl_ml5\">" + zeStarejJedn.Ijr_Przed + "</td>" +
                //"<td class=\"borderBl_ml5\">" + zeStarejJedn.Ud_Z_Jrs + "</td>" +
                //"<td class=\"borderBl_mr5\">" + zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture) + "</td> " +

                sb.AppendLine("<tr>");
                sb.AppendLine(Tbl_td(zeStarejJedn.NrObr.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.NazwaObrebu.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.Ijr_Przed.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.Ud_Z_Jrs.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
                sb.AppendLine("</tr>");
            }

            //podsumowanie tabeli udzialow ze starych jednostek
            sb.AppendLine("<td colspan=\"4\" class=\"greenText\" style=\"text-align: right; border: none;  margin-right:2px; \"><b>Ekwiwalent należny:</b></td>" +
                             "<td class=\"borderBl_mr5_greenText\"><b>" + JednoskaRejNowa.SumaWartJednostekPrzed().ToString("F2", CultureInfo.InvariantCulture) + "</b></td>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }
        static string HTML_TabelaEkwiwalentuNaleznegoBezPEWPotracenie(JR_Nowa JednoskaRejNowa)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"tabelaCzarna\">");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Obręb"));
            sb.AppendLine(Tbl_th("Nazwa obrębu"));
            sb.AppendLine(Tbl_th("Nr jednostki"));
            sb.AppendLine(Tbl_th("Udział"));
            sb.AppendLine(Tbl_th("Wartość<br>przed scaleniem", style: "color: green; width:14%;"));
            sb.AppendLine(Tbl_th("Potrącenie<br>pod drogi", style: "color: green; width:14%"));
            sb.AppendLine(Tbl_th("Ekwiwalent<br>należny", style: "color: green; width:14%"));
            sb.AppendLine("</tr>");

            foreach (var zeStarejJedn in JednoskaRejNowa.zJednRejStarej)
            {

                sb.AppendLine("<tr>");
                sb.AppendLine(Tbl_td(zeStarejJedn.NrObr.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.NazwaObrebu.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.Ijr_Przed.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.Ud_Z_Jrs.ToString()));
                sb.AppendLine(Tbl_td(zeStarejJedn.WrtJednPrzed.ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
                sb.AppendLine(Tbl_td(zeStarejJedn.PotrWart.ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
                sb.AppendLine(Tbl_td((zeStarejJedn.WrtJednPrzed - zeStarejJedn.PotrWart).ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
                sb.AppendLine("</tr>");
            }

            //podsumowanie tabeli udzialow ze starych jednostek
            sb.AppendLine("<td colspan =\"4\" class=\"greenText\" style=\"text-align: right; border: none;  margin-right:2px; \"><b>Suma wartości:</b></td>");
            sb.AppendLine(Tbl_th(JednoskaRejNowa.SumaWartJednostekPrzed().ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
            sb.AppendLine(Tbl_th(JednoskaRejNowa.SumaWartosciPotracen().ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
            sb.AppendLine(Tbl_th(JednoskaRejNowa.SumaWartoJednPrzedPoPotraceinu().ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass));
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string HTML_TabelaEkwiwalentuNaleznegoPotracenie(JR_Nowa JednoskaRejNowa)
        {
            if (JednoskaRejNowa.zJednRejStarej.FindAll(x => x.PotracenieCzyStosowac).Count > 0)
            {
                return HTML_TabelaEkwiwalentuNaleznegoBezPEWPotracenie(JednoskaRejNowa);
            }
            else
            {
                return HTML_TabelaEkwiwalentuNaleznegoBezPEW(JednoskaRejNowa);
            }
        }

        static string Tbl_th(string tytul, int? colspan = null, string style = null, string classCss = "borderBl")
        {
            string styleCss = style != null ? " style=\"" + style + "\" " : null;
            string columnSpan = colspan > 0 ? " colspan =\"" + colspan + "\"" : null;
            return "<th class=\"" + classCss + "\"" + styleCss + columnSpan + "><b>" + tytul + " </b></th>";
        }

        static string Tbl_td(string content, int? colspan = null, string style = null, string classCss = "borderBl")
        {
            string styleCss = style != null ? " style=\"" + style + "\" " : null;
            string columnSpan = colspan > 0 ? " colspan =\"" + colspan + "\"" : null;

            return "<td class=\"" + classCss + "\"" + styleCss + columnSpan + ">" + content + "</td>";
        }

        static string Tbl_PodsumowanieWDzialkachStarych(ZJednRejStarej jednostkaStara)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tbl_td("Razem:", classCss: Css_borderBl_mr5.nameClass, style: "font-weight: bold;"));
            if (jednostkaStara == null)
            {
                sb.AppendLine(Tbl_td("-.----"));
                sb.AppendLine(Tbl_td("-.--"));
            }
            else
            {

                sb.AppendLine(Tbl_td(jednostkaStara.SumaPowierzchniDzialek(), classCss: Css_borderBl_mr5.nameClass, style: "font-weight: bold;"));
                sb.AppendLine(Tbl_td(jednostkaStara.SumaWartosciDzialek(), classCss: Css_borderBl_mr5_greenText.nameClass, style: "font-weight: bold;"));
            }
            sb.AppendLine(Tbl_td(""));
            return sb.ToString();
        }

        static string Tbl_PodsumowanieWDzialkachNowych(JR_Nowa jednoskaRejNowa)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Tbl_td("Razem:", classCss: Css_borderBl_mr5.nameClass, style: "font-weight: bold;"));
            if (jednoskaRejNowa == null)
            {
                sb.AppendLine(Tbl_td("-.----"));
                sb.AppendLine(Tbl_td("-.--"));
            }
            else
            {
                sb.AppendLine(Tbl_td(jednoskaRejNowa.SumaPowierzchniDzialekNowych(), classCss: Css_borderBl_mr5.nameClass, style: "font-weight: bold;"));
                sb.AppendLine(Tbl_td(jednoskaRejNowa.SumaWartosciDzialekNowych(), classCss: Css_borderBl_mr5_greenText.nameClass, style: "font-weight: bold;"));
            }
            sb.AppendLine(Tbl_td(""));
            return sb.ToString();
        }

        static string Tbl_PodsumowanieDlaDzialek(ZJednRejStarej jednostkaStara, JR_Nowa jednoskaRejNowa)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_PodsumowanieWDzialkachStarych(jednostkaStara));
            sb.AppendLine(Tbl_PodsumowanieWDzialkachNowych(jednoskaRejNowa));
            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        static string Tbl_DzialkaStara(List<Dzialka> dzialki, int iterator)
        {
            StringBuilder sb = new StringBuilder();
            string nrdzPrzed;
            string pewDzialki;
            string wartPrzed;
            string kwPrzed;
            if (dzialki != null && dzialki.Count > iterator)
            {
                nrdzPrzed = dzialki[iterator].NrDz;
                pewDzialki = dzialki[iterator].PowDz.ToString("F4", CultureInfo.InvariantCulture);
                wartPrzed = dzialki[iterator].Wartosc.ToString("F2", CultureInfo.InvariantCulture);
                kwPrzed = dzialki[iterator].KW;
            }
            else
            {
                nrdzPrzed = "";
                pewDzialki = "";
                wartPrzed = "";
                kwPrzed = "";
            }

            sb.AppendLine(Tbl_td(nrdzPrzed, classCss: Css_borderBl.nameClass));
            sb.AppendLine(Tbl_td(pewDzialki, classCss: Css_borderBl_mr5.nameClass));
            sb.AppendLine(Tbl_td(wartPrzed, classCss: Css_borderBl_mr5_greenText.nameClass));
            sb.AppendLine(Tbl_td(kwPrzed, classCss: Css_borderBl.nameClass));

            return sb.ToString();
        }

        static string Tbl_DzialkaNowa(List<Dzialka_N> dzialki, int iterator)
        {
            StringBuilder sb = new StringBuilder();
            string nrdzPo;
            string powDzialkiPo;
            string wartPo;
            string kwPo;


            if ((dzialki != null) && dzialki.Count > iterator)
            {
                nrdzPo = dzialki[iterator].NrDz;
                powDzialkiPo = dzialki[iterator].PowDz.ToString("F4", CultureInfo.InvariantCulture);
                wartPo = dzialki[iterator].Wartosc.ToString("F2", CultureInfo.InvariantCulture);
                kwPo = dzialki[iterator].KW;
            }
            else
            {
                nrdzPo = "";
                powDzialkiPo = "";
                wartPo = "";
                kwPo = "";
            }

            sb.AppendLine(Tbl_td(nrdzPo, classCss: CSS_borderBl_redText.nameClass));
            sb.AppendLine(Tbl_td(powDzialkiPo, classCss: Css_borderBl_mr5.nameClass));
            sb.AppendLine(Tbl_td(wartPo, classCss: Css_borderBl_mr5_greenText.nameClass));
            sb.AppendLine(Tbl_td(kwPo, classCss: Css_borderBl.nameClass));

            return sb.ToString();
        }

        static string Tbl_DzialkaStara_Nowa(List<Dzialka> dzialkiStare, List<Dzialka_N> dzialkiNowe)
        {
            int iloscDzialekPrzed = dzialkiStare == null ? 0 : dzialkiStare.Count;
            int iloscDzialekPo = dzialkiNowe == null ? 0 : dzialkiNowe.Count;
            StringBuilder sb = new StringBuilder();

            int liczbaWierszyWTabeli = iloscDzialekPo < iloscDzialekPrzed ? iloscDzialekPrzed : iloscDzialekPo;

            for (int i = 0; i < liczbaWierszyWTabeli; i++)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(Tbl_DzialkaStara(dzialkiStare, i));
                sb.AppendLine(Tbl_DzialkaNowa(dzialkiNowe, i));
                sb.AppendLine("</tr>");
            }
            return sb.ToString();
        }

        public static string HTML_TabelaZDzialkami(JR_Nowa jednoskaRejNowa, ZJednRejStarej jednostkaStara)
        {
            //decimal SumaWartosciPo = jednoskaRejNowa.Dzialki_Nowe.Sum(x => x.Wartosc);
            StringBuilder sb = new StringBuilder();

            // tabela Ekwiwalentów nagłówki
            sb.AppendLine("<table class=\"tabelaCzarna\">");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Ekwiwalenty", 8));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Należny", 4));
            sb.AppendLine(Tbl_th("Zaprojektowany", 4));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Działka", style: "width: 12%"));
            sb.AppendLine(Tbl_th("Pow. ewid.", style: "width: 12%"));
            sb.AppendLine(Tbl_th("Wartość", style: "width: 12%"));
            sb.AppendLine(Tbl_th("KW", style: "width: 14%"));
            sb.AppendLine(Tbl_th("Działka", style: "width: 12%"));
            sb.AppendLine(Tbl_th("Pow.", style: "width: 12%"));
            sb.AppendLine(Tbl_th("Wartość", style: "width: 12%"));
            sb.AppendLine(Tbl_th("KW", style: "width: 14%"));
            sb.AppendLine("</tr>");

            if (jednoskaRejNowa == null)
            {
                jednoskaRejNowa = new JR_Nowa();
            }



            // treść zasadnicza 
            if (jednostkaStara == null)
            {
                sb.AppendLine(Tbl_DzialkaStara_Nowa(null, jednoskaRejNowa.Dzialki_Nowe));
                sb.AppendLine(Tbl_PodsumowanieDlaDzialek(null, jednoskaRejNowa));
            }
            else if (jednoskaRejNowa.Dzialki_Nowe.Count > 0)
            {
                //&& jednoskaRejNowa.Dzialki_Nowe.First().RjdrPrzed == jednostkaStara.Id_Jedns



                var dzialkiN_rjdr = jednoskaRejNowa;
                //var dzialkiN_rjdr = jednoskaRejNowa.JednostkaZDzialkamiZRJDRPrzed(jednostkaStara.Id_Jedns);

                sb.AppendLine(Tbl_DzialkaStara_Nowa(jednostkaStara.Dzialki, dzialkiN_rjdr.Dzialki_Nowe));

                sb.AppendLine(Tbl_PodsumowanieDlaDzialek(jednostkaStara, dzialkiN_rjdr));
            }
            else
            {
                sb.AppendLine(Tbl_DzialkaStara_Nowa(jednostkaStara.Dzialki, null));
                sb.AppendLine(Tbl_PodsumowanieDlaDzialek(jednostkaStara, null));
            }

            sb.AppendLine("</table>");
            sb.AppendLine("<br />");

            return sb.ToString();
        }

        public static string HTML_TabelkaPotraceniePodDzialkami(ZJednRejStarej jednostkaStara)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table class=\"tabelaCzarna\">");
            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Potrącenie pod drogi", 2, classCss: Css_borderBl_mr5.nameClass, style: "width: 24%"));
            sb.AppendLine(Tbl_th(jednostkaStara.PotrWart.ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass, style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 14%"));
            sb.AppendLine(Tbl_th("", style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 14%"));
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("Ekwiwalent należny", 2, classCss: Css_borderBl_mr5.nameClass, style: "width: 24%"));
            sb.AppendLine(Tbl_th((jednostkaStara.WrtJednPrzed - jednostkaStara.PotrWart).ToString("F2", CultureInfo.InvariantCulture), classCss: Css_borderBl_mr5_greenText.nameClass, style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 14%"));
            sb.AppendLine(Tbl_th("", style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 12%"));
            sb.AppendLine(Tbl_th("", style: "width: 14%"));
            sb.AppendLine("</tr>");


            sb.AppendLine("</table>");
            return sb.ToString();
        }

        public static string HTML_TytulCzerwonyPodkreslony(string tytul)
        {
            return "<div style=\"text-align: center; color: red; margin: 0; padding: 0;\"><b><u>" + tytul + "</u></b></div><br>";
        }

        public static string HTML_CzerwonkaTabelka(JR_Nowa jednoskaRejNowa)
        {
            string sumaPowierzchniDzialekPrzedScaleniem = jednoskaRejNowa.SumaPowJednostekPrzed().ToString("F4", CultureInfo.InvariantCulture);
            string SumaWartosciPrzed = jednoskaRejNowa.SumaWartJednostekPrzed().ToString("F2", CultureInfo.InvariantCulture);
            string sumaPowierzchniDzialekNowych = jednoskaRejNowa.SumaPowierzchniDzialekNowych();
            string SumaWartosciPo = jednoskaRejNowa.SumaWartosciDzialekNowych();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"" + Css_tabelaCzerwona.nameClass + "\">");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("", classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th("Należny", classCss: Css_borderRed.nameClass, colspan: 2));
            sb.AppendLine(Tbl_th("Zaprojektowany", classCss: Css_borderRed.nameClass, colspan: 2));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("", classCss: Css_borderRed.nameClass, style: "width: 14%"));
            sb.AppendLine(Tbl_th("Pow. ewid.", classCss: Css_borderRed.nameClass, style: "width: 22%"));
            sb.AppendLine(Tbl_th("Wartość", classCss: Css_borderRed.nameClass, style: "width: 23%; color: green;"));
            sb.AppendLine(Tbl_th("Powierzchnia", classCss: Css_borderRed.nameClass, style: "width: 22%"));
            sb.AppendLine(Tbl_th("Wartość", classCss: Css_borderRed.nameClass, style: "width: 23%; color: green;"));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("RAZEM GOSP.:", classCss: Css_borderRed.nameClass, style: "color: red;"));
            sb.AppendLine(Tbl_th(sumaPowierzchniDzialekPrzedScaleniem, classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th(SumaWartosciPrzed, classCss: Css_borderRed.nameClass, style: "color: green;"));
            sb.AppendLine(Tbl_th(sumaPowierzchniDzialekNowych, classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th(SumaWartosciPo, classCss: Css_borderRed.nameClass, style: "color: green;"));
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static string HTML_CzerwonkaTabelkaPotracenia(JR_Nowa jednoskaRejNowa)
        {
            string sumaPowierzchniDzialekPrzedScaleniem = jednoskaRejNowa.SumaPowJednostekPrzed().ToString("F4", CultureInfo.InvariantCulture);
            string sumaWartosciPrzed = jednoskaRejNowa.SumaWartJednostekPrzed().ToString("F2", CultureInfo.InvariantCulture);
            string sumaPowierzchniDzialekNowych = jednoskaRejNowa.SumaPowierzchniDzialekNowych();
            string sumaWartosciPo = jednoskaRejNowa.SumaWartosciDzialekNowych();
            string ekwiwalentNaleznyPopotraceniu = jednoskaRejNowa.SumaWartoJednPrzedPoPotraceinu().ToString("F2", CultureInfo.InvariantCulture);
            string odchFkt = (jednoskaRejNowa.SumaWartosciDzialekNowychDecimal() - jednoskaRejNowa.SumaWartoJednPrzedPoPotraceinu()).ToString("F2", CultureInfo.InvariantCulture);
            string odchDop = Math.Abs((jednoskaRejNowa.SumaWartoJednPrzedPoPotraceinu() * 0.03M)).ToString("F2", CultureInfo.InvariantCulture);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"" + Css_tabelaCzerwona.nameClass + "\">");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("", classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th("Stan przed scaleniem", classCss: Css_borderRed.nameClass, colspan: 3));
            sb.AppendLine(Tbl_th("Stan po scaleniu", classCss: Css_borderRed.nameClass, colspan: 2));
            sb.AppendLine(Tbl_th("", 2, classCss: Css_borderRed.nameClass));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("", classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th("Powierzchnia<br>w ha", classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th("Wartość<br>przed scaleniem", classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th("Ekwiwalent<br>należny", classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th("Powierzchnia<br>w ha", classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th("Wartość<br>po scaleniu", classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th("Odchyłka<br>faktyczna", classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th("Odchyłka<br>dopuszczalna ±3%", classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr>");
            sb.AppendLine(Tbl_th("RAZEM GOSP.:", classCss: Css_borderRed.nameClass, style: "height: 20"));
            sb.AppendLine(Tbl_th(sumaPowierzchniDzialekPrzedScaleniem, classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th(sumaWartosciPrzed, classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th(ekwiwalentNaleznyPopotraceniu, classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th(sumaPowierzchniDzialekNowych, classCss: Css_borderRed.nameClass));
            sb.AppendLine(Tbl_th(sumaWartosciPo, classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th(odchFkt, classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine(Tbl_th(odchDop, classCss: Css_borderRedTxtGreen.nameClass));
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static string HTML_TabelaDoplata(JR_Nowa jednoskaRejNowa)
        {
            StringBuilder sb = new StringBuilder();
            decimal sumaWartosciPrzed = jednoskaRejNowa.SumaWartJednostekPrzed();
            decimal SumaWartosciPo = jednoskaRejNowa.SumaWartosciDzialekNowychDecimal();
            decimal wartPotracenia = jednoskaRejNowa.zJednRejStarej.Sum(x => x.PotrWart);

            decimal odchylkaFaktyczna = SumaWartosciPo - sumaWartosciPrzed;

            string szerTabPodBilansem = jednoskaRejNowa.Odcht == true ? "288" : "0.36%";
            // Tabelka czarna pod bilansem
            sb.AppendLine("<table width = " + szerTabPodBilansem + " style =\"border: 1px solid black; border-collapse: collapse; font-size: 9pt; \">");
            sb.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka dopuszczalna 3%:</b></td><td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + Decimal.Round(sumaWartosciPrzed * 0.03M, 2).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
            sb.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka faktyczna:</b></td><td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + odchylkaFaktyczna.ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");


            if (jednoskaRejNowa.Odcht == false)
            {
                if (odchylkaFaktyczna < 0)
                {
                    sb.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Winien otrzymać: </b></td><td style =\"border: 1px solid black; text-align: center; color: green;\" width=33.34%><b>" + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
                }
                else
                {
                    sb.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Winien zapłacić: </b></td><td style =\"border: 1px solid black; text-align: center; color: red;\" width=33.34%><b>" + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td></tr>");
                }
            }
            else
            {
                sb.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka techniczna</b></td><td style =\"border: 1px solid black; text-align: center; \" width=33.34%><b>-.--</b></td></tr>");
            }
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static string HTML_TabelaDoplataPotracenia(JR_Nowa jednoskaRejNowa)
        {            // Tabelka czarna pod bilansem
            StringBuilder sb = new StringBuilder();
            decimal sumaWartosciPrzed = jednoskaRejNowa.SumaWartJednostekPrzed();
            decimal SumaWartosciPo = jednoskaRejNowa.SumaWartosciDzialekNowychDecimal();
            decimal ekwiwalentNaleznyPopotraceniu = jednoskaRejNowa.SumaWartoJednPrzedPoPotraceinu();

            decimal odchylkaFaktyczna = jednoskaRejNowa.Odcht == true || jednoskaRejNowa.ZerujDoplaty == true ? 0 : ekwiwalentNaleznyPopotraceniu - SumaWartosciPo;
            decimal doplataZaDrogi = jednoskaRejNowa.DoplZaDrNieNaliczaj == true || jednoskaRejNowa.ZerujDoplaty == true ? 0 : jednoskaRejNowa.SumaWartosciPotracen();
            decimal sumaDoplat = odchylkaFaktyczna + doplataZaDrogi;


            sb.AppendLine("<table class=\"" + Css_TabelkaPodBilansem.nameClass + "\">");


            sb.AppendLine("<tr>");
            sb.Append(Tbl_th(""));
            sb.Append(Tbl_th("Należność w PLN"));
            sb.AppendLine("</tr>");


            sb.AppendLine("<tr>");
            sb.Append(Tbl_th("Dopłata za drogi"));
            sb.Append(Tbl_th(doplataZaDrogi.ToString("F2", CultureInfo.InvariantCulture), style: "color: blue;"));
            sb.AppendLine("</tr>");

            //sb.AppendLine("<tr>" +
            //    "<td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka dopuszczalna 3%:</b></td>" +
            //"<td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + Decimal.Round(sumaWartosciPrzed * 0.03M, 2).ToString("F2", CultureInfo.InvariantCulture) + "</b></td>" +
            //"</tr>");

            sb.AppendLine("<tr>");
            sb.Append(Tbl_th((odchylkaFaktyczna >= 0 ? "Dopłata" : "Spłata") + " za ekwiwalent"));
            sb.Append(Tbl_th(odchylkaFaktyczna.ToString("F2", CultureInfo.InvariantCulture), style: "color: " + (odchylkaFaktyczna >= 0  ?  "blue;" : "red;")));
            sb.AppendLine("</tr>");




            //sb.AppendLine("<tr>" +
            //    "<td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka faktyczna:</b></td>" +
            //    "<td style =\"border: 1px solid black; text-align: center;\" width=33.34%><b>" + odchylkaFaktyczna.ToString("F2", CultureInfo.InvariantCulture) + "</b></td>" +
            //    "</tr>");

            //jednoskaRejNowa.DoplZaDrNieNaliczaj
            //jednoskaRejNowa.ZerujDoplaty
            //jednoskaRejNowa.Odcht



                sb.AppendLine("<tr>");

                sb.AppendLine(Tbl_th("Łącznie winien " + ( sumaDoplat >= 0 ? "otrzymać: " : "zapłacić")));

                sb.Append(Tbl_th(Math.Abs(sumaDoplat).ToString("F2", CultureInfo.InvariantCulture), style: "color: " + (sumaDoplat >= 0 ? "blue;" : "red;")));

                sb.AppendLine("</tr>");

                //sb.AppendLine("<tr>" +
                //    "<td style =\"border: 1px solid black;\" width=66.66%><b>Łącznie winien otrzymać: </b></td>" +
                //    "<td style =\"border: 1px solid black; text-align: center; color: green;\" width=33.34%><b>"
                //    + Math.Abs(odchylkaFaktyczna).ToString("F2", CultureInfo.InvariantCulture) + "</b></td>" +
                //    "</tr>");


            //sb.AppendLine("<tr><td style =\"border: 1px solid black;\" width=66.66%><b>Odchyłka techniczna</b></td><td style =\"border: 1px solid black; text-align: center; \" width=33.34%><b>-.--</b></td></tr>");

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static string HTML_TabelaOswiadczenia()
        {
            StringBuilder sb = new StringBuilder();
            // tabelka oświadczenia uczestnika i omówienie zastrzeżeń
            int wysWierszaTabeliPX = 18;
  
            sb.AppendLine("<table class=\""+ Css_tabelaCzarna.nameClass + " style=\"font-size: 10px;\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th style=\"border: 1px solid; width: 50%;\"><i>Oświadczenie uczestnika scalenia w sprawie projektu wstępnego, " +
                "treść ewentualnych zastrzeżeń, data i podpis uczestnika scalenia.</i></th>");
            sb.AppendLine("<th style=\"border: 1px solid; width: 50%;\"><i><b>Omówienie zastrzeżeń, proponowane zmiany <br> data i podpis geodety.</b></i></th>");
            sb.AppendLine("</tr>");


            //generowanie pustych wierszy w tabeli
            for (int i = 0; i < 9; i++)
            {
                sb.AppendLine("<tr style=\"border: 1px solid black; height: " + wysWierszaTabeliPX + "px;\">");
                sb.AppendLine("<td style=\"border-bottom: 1px solid #8c8c8c; border-right: 1px solid #000000;\"></td>");
                sb.AppendLine("<td style=\"border-bottom: 1px solid #8c8c8c;\"></td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }
    }
}
