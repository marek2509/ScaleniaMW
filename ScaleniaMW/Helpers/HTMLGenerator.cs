using FirebirdSql.Data.Services;
using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Helpers
{
    public static class HTMLGenerator
    {
        static double PewRazem { get; set; } = 0;
        static string TBody { get; set; } = string.Empty;

        public static void WzdeStep1TrDzialkiNieUjawnione(List<Dzialka> dzialki)
        {
            if (dzialki.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (Dzialka d in dzialki)
                {
                    sb.AppendLine(string.Format("\t\t<tr wierszDzialka>\r\n\t\t\t<td nrobr-obr>{0}</td>\r\n\t\t\t<td nrdz>{1}</td>\r\n\t\t\t<td pow>{2}</td>\r\n\t\t</tr>",
                        $"{d.Obreb.ID} {d.Obreb.NAZ}", d.IDD, (d.PEW / 10000d).ToString("F4")));
                }
                var sumaPew = dzialki.Sum(x => x.PEW);
                PewRazem += sumaPew;
                sb.AppendLine(string.Format("\t\t<tr wierszsuma>\r\n\t\t\t<td colspan=\"2\" class=\"right bold\">SUMA:</td>\r\n\t\t\t<td class=\"bold\">{0}</td>\r\n\t\t</tr>", (sumaPew / 10000d).ToString("F4")));
                sb.AppendLine("\t\t<tr pustyWiersz>\r\n\t\t\t<td colspan=\"3\">&nbsp</td>\r\n\t\t</tr>");
                TBody += sb.ToString();
            }
        }

        public static void WzdeStep2TrKW(List<Dzialka> dzialkiZTymSamymKW)
        {
            if (dzialkiZTymSamymKW.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("\t\t<tr KW>\r\n\t\t\t<td colspan=\"3\" class=\"bold\" KW>BI2P/00004909/9</td>\r\n\t\t</tr>", dzialkiZTymSamymKW.FirstOrDefault().KW));
                foreach (Dzialka d in dzialkiZTymSamymKW)
                {
                    sb.AppendLine(string.Format("\t\t<tr wierszDzialka>\r\n\t\t\t<td nrobr-obr>{0}</td>\r\n\t\t\t<td nrdz>{1}</td>\r\n\t\t\t<td pow>{2}</td>\r\n\t\t</tr>",
                        $"{d.Obreb.ID} {d.Obreb.NAZ}", d.IDD, (d.PEW/10000d).ToString("F4")));
                }
                var sumaPew = dzialkiZTymSamymKW.Sum(x => x.PEW);
                PewRazem += sumaPew;
                sb.AppendLine(string.Format("\t\t<tr wierszsuma>\r\n\t\t\t<td colspan=\"2\" class=\"right bold\">SUMA:</td>\r\n\t\t\t<td class=\"bold\">{0}</td>\r\n\t\t</tr>", (sumaPew / 10000d).ToString("F4")));
                TBody += sb.ToString();
            }
        }

        public static string WzdeStep3GetTable()
        {
            TBody += string.Format("\t\t<tr wiersz razem>\r\n\t\t\t<td colspan=\"2\" class=\"right bold\">RAZEM:</td>\r\n\t\t\t<td class=\"bold\">{0}</td>\r\n\t\t</tr>", (PewRazem / 10000d).ToString("F4"));
            var table = "\r\n<table class=\"myTable\">\r\n\t\r\n\r\n\t<thead>\r\n\t\t<tr>\r\n\t\t\t<th colspan=\"3\">Stan przed scaleniem</th>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<th style=\"width: 40%;\">Obręb</th>\r\n\t\t\t<th style=\"width: 30%;\">Nr działki</th>\r\n\t\t\t<th>Pow.[Ha]</th>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<th colspan=\"3\">Działki nie ujawnione w KW</th>\r\n\t\t</tr>\r\n\t</thead>\r\n\t<tbody>\r\n\t\t" + TBody + "\r\n\t</tbody>\r\n</table>";
            TBody = string.Empty;
            PewRazem = 0;
            return table;
        }


        public static string WzdeStep4InsertTablesIntoPage(string leftTable, string rightTable)
        {
            return $"<html lang=\"pl\">\r\n<head>\r\n<meta charset=\"windows-1250\">\r\n<meta http-equiv=Content-Type content=\"text/html;>\r\n<meta name=Generator content=\"Microsoft Word 12 (filtered)\"\r\nmlns:v=\"urn:schemas-microsoft-com:vml\"\r\nxmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\nxmlns:w=\"urn:schemas-microsoft-com:office:word\"\r\nxmlns:m=\"http://schemas.microsoft.com/office/2004/12/omml\"\r\nxmlns=\"http://www.w3.org/TR/REC-html40\"\r\n>\r\n\t<style>\r\n\t\tbody{{\r\n\t\t\tfont-family: \"Arial Narrow\";\r\n\t\t    font-style: italic;\r\n\t\t\twidth: 620;\r\n\t\t}}\r\n\t\t\r\n\t\t.container{{\r\n\t\t\twidth: 100%;\r\n\t\t}}\r\n\t\t\r\n\t\t.myTable {{\r\n\t\t   width: 100%;\r\n\t\t   border: 1 solid black;\r\n\t\t   background: #ffffff;\r\n\t\t   text-align: center;\r\n\t\t   border-collapse: collapse;\r\n\t\t   font-size: 13;\r\n\t\t}}\r\n\r\n\t\t.myTable>thead {{\r\n\r\n\t\t}}\r\n\t\t\r\n\t\t.right{{\r\n\t\t\ttext-align: right\r\n\t\t}}\r\n\t\t\r\n\t\t.bold{{\r\n\t\t\tfont-weight: bold;\r\n\t\t}}\r\n\t\t\r\n\t\ttd, th {{\r\n\t\t   border: 1px solid black;\r\n\t\t}}\r\n\t\t\r\n\t\t.b-none{{\r\n\t\t\tborder: none;\r\n\t\t}}\r\n\t\t\r\n\t\t.tytul{{\r\n\t\t\tfont-size: 16;\r\n\t\t    text-align: center;\r\n\t\t\tfont-weight: bold;\r\n\t\t   text-decoration: underline;\r\n\t\t}}\r\n\t\t\t.aligne-top{{\r\n\t\t\tvertical-align: top;\r\n\t\t}}</style>\r\n</head>\r\n<body>\r\n\r\n\t<p class=\"tytul\">Wykaz zmian danych ewidencyjnych<br>\r\n\tBI2P/00004909/9\t\r\n\t</p>\r\n\t<div style=\"display: flex; flex-direction: row;\">\r\n\t\t<table class=\"container b-none\" >\r\n\t\t\t<tr class=\"b-none\">\r\n\t\t\t\t<td class=\"b-none aligne-top\">{leftTable}\t\t\t\t</td>\r\n\t\t\t\t<td class=\"b-none aligne-top\">{rightTable}</td>\r\n\t\t\t</tr>\r\n\t\t</table>\r\n\t</div>\r\n</body>\r\n</html>";
        }
    }
}
