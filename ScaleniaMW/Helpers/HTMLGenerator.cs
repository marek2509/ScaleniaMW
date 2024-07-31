using FirebirdSql.Data.Services;
using ScaleniaMW.Entities;
using ScaleniaMW.Repositories.Results;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ScaleniaMW.Helpers
{
    public static class HTMLGenerator
    {
        static double PewSum { get; set; } = 0;
        static string TBody { get; set; } = string.Empty;
        static bool IsNieujawnioneKW { get; set; }
        static string CurrentKW { get; set; }
        static string EkwiwalenZamiennyCz1DzialkiPO { get; set; }

        public static void WzdeStep1TrDzialkiNieUjawnione(List<Dzialka> dzialki)
        {

            if (dzialki.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (Dzialka d in dzialki.OrderBy(x => x.Obreb.ID).ThenBy(x => x.SIDD))
                {
                    sb.AppendLine(string.Format("\t\t<tr wierszDzialka>\r\n\t\t\t<td nrobr-obr>{0}</td>\r\n\t\t\t<td nrdz>{1}</td>\r\n\t\t\t<td pow>{2}</td>\r\n\t\t</tr>",
                        $"{d.Obreb.ID} {d.Obreb.NAZ}", d.IDD, (d.PEW / 10000d).ToString("F4")));
                }
                var sumaPew = dzialki.Sum(x => x.PEW);
                PewSum += sumaPew;
                sb.AppendLine(string.Format("\t\t<tr wierszsuma>\r\n\t\t\t<td colspan=\"2\" class=\"right bold\">SUMA:</td>\r\n\t\t\t<td class=\"bold\">{0}</td>\r\n\t\t</tr>", (sumaPew / 10000d).ToString("F4")));
                sb.AppendLine("\t\t<tr pustyWiersz>\r\n\t\t\t<td colspan=\"3\">&nbsp</td>\r\n\t\t</tr>");
                TBody += sb.ToString();
                IsNieujawnioneKW = true;
            }
        }

        public static void WzdeStep2TrKW(List<Dzialka> dzialkiZTymSamymKW)
        {
            if (dzialkiZTymSamymKW.Any())
            {
                StringBuilder sb = new StringBuilder();
                if (dzialkiZTymSamymKW.Any())
                {
                    CurrentKW = dzialkiZTymSamymKW.FirstOrDefault().KW;
                    sb.AppendLine($"\t\t<tr KW>\r\n\t\t\t<td colspan=\"3\" class=\"bold\" KW>{CurrentKW}</td>\r\n\t\t</tr>");
                    foreach (Dzialka d in dzialkiZTymSamymKW.OrderBy(x => x.Obreb.ID).ThenBy(x => x.SIDD))
                    {
                        sb.AppendLine(string.Format("\t\t<tr wierszDzialka>\r\n\t\t\t<td nrobr-obr>{0}</td>\r\n\t\t\t<td nrdz>{1}</td>\r\n\t\t\t<td pow>{2}</td>\r\n\t\t</tr>",
                            $"{d.Obreb.ID} {d.Obreb.NAZ}", d.IDD, (d.PEW / 10000d).ToString("F4")));
                    }
                }
                else
                {
                    sb.AppendLine(string.Format("\t\t<tr wierszDzialka>\r\n\t\t\t<td nrobr-obr>{0}</td>\r\n\t\t\t<td nrdz>{1}</td>\r\n\t\t\t<td pow>{2}</td>\r\n\t\t</tr>",
                                "-", "-", "-"));
                }
                var sumaPew = dzialkiZTymSamymKW.Sum(x => x.PEW);
                PewSum += sumaPew;
                sb.AppendLine(string.Format("\t\t<tr wierszsuma>\r\n\t\t\t<td colspan=\"2\" class=\"right bold\">SUMA:</td>\r\n\t\t\t<td class=\"bold\">{0}</td>\r\n\t\t</tr>", (sumaPew / 10000d).ToString("F4")));
                TBody += sb.ToString();
            }
        }

        public static string WzdeStep3GetTable(bool poScaleniu = false)
        {

            var nieujawnioneKWTitle = IsNieujawnioneKW ? "<tr>\r\n\t\t\t<th colspan=\"3\">Działki nie ujawnione w KW</th>\r\n\t\t</tr>" : "";

            TBody += string.Format("\t\t<tr wiersz razem>\r\n\t\t\t<td colspan=\"2\" class=\"right bold\">RAZEM:</td>\r\n\t\t\t<td class=\"bold\">{0}</td>\r\n\t\t</tr>", (PewSum / 10000d).ToString("F4"));
            var table = $"\r\n<table class=\"myTable\">\r\n\t\r\n\r\n\t<thead>\r\n\t\t<tr>\r\n\t\t\t<th colspan=\"3\">{(poScaleniu ? "Stan po scaleniem" : "Stan przed scaleniem")}</th>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<th class=\"widthObr\">Obręb</th>\r\n\t\t\t<th class=\"widthDz\">Nr działki</th>\r\n\t\t\t<th>Pow.[Ha]</th>\r\n\t\t</tr>\r\n\t\t" +
                nieujawnioneKWTitle +
                "\r\n\t</thead>\r\n\t<tbody>\r\n\t\t" + TBody + "\r\n\t</tbody>\r\n</table>";
            TBody = string.Empty;
            PewSum = 0;
            IsNieujawnioneKW = false;
            return table;
        }

        public static string WzdeStep4Description(List<Dzialka> dzialkiPrzed, List<Dzialka> dzialkiNieujawnione, List<Dzialki_n> dzialkiPo,string txtStarosta, string txtDecyzja, string txtDataDecyzji)
        {
            StringBuilder stringBuilderDescription = new StringBuilder();
            stringBuilderDescription.Append("<div>");
            stringBuilderDescription.Append("<p>Uwagi:</p>");
            stringBuilderDescription.Append("<p>");
            if (dzialkiPo.Any())
            {
                var firstLoop = true;

                var obrebyDzialekPo = dzialkiPo.Select(x => x.IDOBR).Distinct().ToList();

                foreach (var idobr in obrebyDzialekPo)
                {
                    var dzialkiWObrebie = dzialkiPo.Where(x => x.IDOBR == idobr).ToList();
                    if (firstLoop)
                    {
                        firstLoop = false;
                        stringBuilderDescription.Append($"Działk{(dzialkiWObrebie.Count > 1 ? "i" : "a")} nr: ");
                        stringBuilderDescription.Append(string.Join(", ", dzialkiWObrebie.Select(x => x.IDD).ToList()));
                        stringBuilderDescription.Append($" położon{(dzialkiWObrebie.Count > 1 ? "e" : "a")} w obrębie ewidencyjnym {dzialkiWObrebie.FirstOrDefault()?.Obreb?.NAZ?.ToUpper()}");
                    }
                    else
                    {
                        stringBuilderDescription.Append($", działk{(dzialkiWObrebie.Count > 1 ? "i" : "a")} nr: ");
                        stringBuilderDescription.Append(string.Join(", ", dzialkiWObrebie.Select(x => x.IDD).ToList()));
                        stringBuilderDescription.Append($" położon{(dzialkiWObrebie.Count > 1 ? "e" : "a")} w obrębie ewidencyjnym {dzialkiWObrebie.FirstOrDefault()?.Obreb?.NAZ?.ToUpper()}");
                    }
                }
                stringBuilderDescription.Append($" o łącznej powierzchni {(dzialkiPo.Sum(x => x.PEW) / 10000d).ToString("F4")} ha stanowi{(dzialkiPo.Count > 1 ? "ą" : "")} ekwiwalent zamienny za:");
            }
            else
            {
                stringBuilderDescription.Append("Brak działek po scaleniu.");
            }

            if (dzialkiPrzed.Any())
            {
                var firstLoop = true;

                var obrebyDzialekPrzed = dzialkiPrzed.Select(x => x.IDOBR).Distinct().ToList();

                foreach (var idobr in obrebyDzialekPrzed)
                {
                    var dzialkiWObrebie = dzialkiPrzed.Where(x => x.IDOBR == idobr).ToList();


                    if (firstLoop)
                    {
                        firstLoop = false;
                        stringBuilderDescription.Append($" działk{(dzialkiWObrebie.Count > 1 ? "i" : "ę")} nr: ");
                        stringBuilderDescription.Append(string.Join(", ", dzialkiWObrebie.Select(x => x.IDD).ToList()));
                        stringBuilderDescription.Append($" położon{(dzialkiWObrebie.Count > 1 ? "e" : "ą")} w obrębie ewidencyjnym {dzialkiWObrebie.FirstOrDefault()?.Obreb?.NAZ?.ToUpper()}");
                    }
                    else
                    {
                        stringBuilderDescription.Append($", działk{(dzialkiWObrebie.Count > 1 ? "i" : "ę")} nr: ");
                        stringBuilderDescription.Append(string.Join(", ", dzialkiWObrebie.Select(x => x.IDD).ToList()));
                        stringBuilderDescription.Append($" położon{(dzialkiWObrebie.Count > 1 ? "e" : "ą")} w obrębie ewidencyjnym {dzialkiWObrebie.FirstOrDefault()?.Obreb?.NAZ?.ToUpper()}");
                    }
                }
                stringBuilderDescription.Append($" o łącznej powierzchni {(dzialkiPrzed.Sum(x => x.PEW) / 10000d).ToString("F4")} ha opisan{(dzialkiPrzed.Count > 1 ? "e" : "ą")} w ");
                stringBuilderDescription.Append($"{dzialkiPrzed.FirstOrDefault().KW}");
            }


            if (dzialkiNieujawnione.Any())
            {
                var firstLoop = true;

                var obrebyDzialekPrzed = dzialkiNieujawnione.Select(x => x.IDOBR).Distinct().ToList();

                foreach (var idobr in obrebyDzialekPrzed)
                {
                    var dzialkiWObrebie = dzialkiNieujawnione.Where(x => x.IDOBR == idobr).ToList();
                    if (firstLoop)
                    {
                        firstLoop = false;
                        stringBuilderDescription.Append($" oraz działk{(dzialkiWObrebie.Count > 1 ? "i" : "ę")} nr: ");
                        stringBuilderDescription.Append(string.Join(", ", dzialkiWObrebie.Select(x => x.IDD).ToList()));
                        stringBuilderDescription.Append($" położon{(dzialkiWObrebie.Count > 1 ? "e" : "ą")} w obrębie ewidencyjnym {dzialkiWObrebie.FirstOrDefault()?.Obreb?.NAZ?.ToUpper()}");
                    }
                    else
                    {
                        stringBuilderDescription.Append($", działk{(dzialkiWObrebie.Count > 1 ? "i" : "ę")} nr: ");
                        stringBuilderDescription.Append(string.Join(", ", dzialkiWObrebie.Select(x => x.IDD).ToList()));
                        stringBuilderDescription.Append($" położon{(dzialkiWObrebie.Count > 1 ? "e" : "ą")} w obrębie ewidencyjnym {dzialkiWObrebie.FirstOrDefault()?.Obreb?.NAZ?.ToUpper()}");
                    }
                }
                stringBuilderDescription.Append($" o łącznej powierzchni {(dzialkiNieujawnione.Sum(x => x.PEW) / 10000d).ToString("F4")} ha - brak KW");
            }

            stringBuilderDescription.Append(".</p><p>");
            stringBuilderDescription.Append("Zmiana numeracji i powierzchni działek nastąpiła w wyniku scalenia gruntów zatwierdzonego<br>decyzją ");
            stringBuilderDescription.Append(string.IsNullOrWhiteSpace(txtStarosta) ? "............................................." : txtStarosta); // dodać zmienną na staroste
            stringBuilderDescription.Append(" Nr "); // dodać zmienną na nr decyzji
            stringBuilderDescription.Append(string.IsNullOrWhiteSpace(txtDecyzja) ? "............................." : txtDecyzja);
            stringBuilderDescription.Append(" z dnia ");
            stringBuilderDescription.Append(string.IsNullOrWhiteSpace(txtDataDecyzji) ? "............................." : txtDataDecyzji);
            stringBuilderDescription.Append("</p>");
            stringBuilderDescription.Append("</div>");

            return stringBuilderDescription.ToString();
        }

        public static string WzdeStep5InsertTablesIntoPage(string leftTable, string rightTable, string description)
        {
            return $"<html lang=\"pl\">\r\n<head>\r\n<meta charset=\"windows-1250\">\r\n<meta http-equiv=Content-Type content=\"text/html;>\r\n<meta name=Generator content=\"Microsoft Word 12 (filtered)\"\r\nmlns:v=\"urn:schemas-microsoft-com:vml\"\r\nxmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\nxmlns:w=\"urn:schemas-microsoft-com:office:word\"\r\nxmlns:m=\"http://schemas.microsoft.com/office/2004/12/omml\"\r\nxmlns=\"http://www.w3.org/TR/REC-html40\"\r\n>\r\n\t" +
                $"<style>\r\n\t\tbody{{\r\n\t\t\tfont-family: \"Arial Narrow\";\r\n\t\t    font-style: italic;\r\n\t\t\twidth: 620;\r\n\t\t}}\r\n\t\t\r\n\t\t" +
                $".container{{\r\n\t\t\twidth: 100%;\r\n\t\t}}\r\n\t\t\r\n\t\t" +
                $".myTable {{\r\n\t\t   width: 100%;\r\n\t\t   border: 1 solid black;\r\n\t\t   background: #ffffff;\r\n\t\t   text-align: center;\r\n\t\t   border-collapse: collapse;\r\n\t\t   font-size: 13;\r\n\t\t}}\r\n\r\n\t\t" +
                $".myTable>thead {{\r\n\r\n\t\t}}\r\n\t\t\r\n\t\t" +
                $".right{{\r\n\t\t\ttext-align: right\r\n\t\t}}\r\n\t\t\r\n\t\t" +
                $".bold{{\r\n\t\t\tfont-weight: bold;\r\n\t\t}}\r\n\t\t\r\n\t\ttd, th {{\r\n\t\t   border: 1px solid black;\r\n\t\t}}\r\n\t\t\r\n\t\t" +
                $".b-none{{\r\n\t\t\tborder: none;\r\n\t\t}}\r\n\t\t\r\n\t\t" +
                $".tytul{{\r\n\t\t\tfont-size: 16;\r\n\t\t    text-align: center;\r\n\t\t\tfont-weight: bold;\r\n\t\t   text-decoration: underline;\r\n\t\t}}\r\n\t\t\t" +
                $".widthObr{{width: 48%;}} " +
                $".widthDz{{width: 26%;}} " +
                $".w-50{{width: 50%;}} " +
                $"</style>" +
                $"\r\n</head>\r\n<body>\r\n\r\n\t<p class=\"tytul\">Wykaz zmian danych ewidencyjnych<br>\r\n\t{CurrentKW}\t\r\n\t</p>\r\n\t<div>\r\n\t\t" +
                $"<table class=\"container b-none\" >\r\n\t\t\t<tr class=\"b-none\">\r\n\t\t\t\t<td valign=\"top\" class=\"b-none w-50\">{leftTable}\t\t\t\t</td>\r\n\t\t\t\t<td valign=\"top\" class=\"b-none w-50\">{rightTable}</td>\r\n\t\t\t</tr>\r\n\t\t</table>\r\n\t</div>\r\n" +
                $"{description}</body>\r\n</html>";
        }


        public static string KWInfoData(Dzialka dzialka, List<GetOwnersForJRResult> owners)
        {
            StringBuilder sbWlasciciele = new StringBuilder();
            foreach (var owner in owners)
            {
                sbWlasciciele.AppendLine($"<span>{owner.SYMBOL} {owner.UD} - {owner.WLASCICIELE}</span><br>");
            }


            return $"<html lang=\"pl\">\r\n<head>\r\n<meta charset=\"UTF-8\"></head><body>" +
                $"<span><b>Obr:</b> {dzialka.Obreb.ID} {dzialka.Obreb.NAZ}</span><br>" +
                $"<span><b>JR:</b> {dzialka.JednRej.IJR}</span><br>" +
                $"<span><b>Dz. z jedn.:</b> {string.Join(", ", dzialka.JednRej.Dzialki.Select(x => $"{x.Obreb.ID}-{x.IDD}"))}</span><br>" +
                "<b>Właściciele:</b><br>" +
                sbWlasciciele.ToString() +
                $"</body></html>";
        }

        public static string KWLista(List<string> kwList)
        {
            StringBuilder sbWlasciciele = new StringBuilder();
            foreach (var kw in kwList)
            {
                sbWlasciciele.AppendLine($"<span>{kw}</span>");
            }

            return $"<html lang=\"pl\">\r\n<head>\r\n<meta charset=\"UTF-8\"></head><body>" +
                sbWlasciciele.ToString() +
                $"</body></html>";
        }
    }
}
