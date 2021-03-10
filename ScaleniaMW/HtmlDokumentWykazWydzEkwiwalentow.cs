using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
     static class HtmlDokumentWykazWydzEkwiwalentow
    {
        public const string HTML_PodzialSekcjiNaStronieNieparzystej = "<span style='font-size:12.0pt;font-family:\"Times New Roman\",\"serif\";mso-fareast-font-family: \"Times New Roman\";mso-ansi-language:PL;mso-fareast-language:PL;mso-bidi-language: AR-SA'><br clear=all style='page-break-before:right;mso-break-type:section-break'></span>";
        public const string HTML_PoczatekWykazyWydzEkwiwalentow = "<html><head><meta content=\"text/html; charset=windows-1250\"> <meta name=Generator content=\"Microsoft Word 12 (filtered)\"> <style> table, th, td { border: 1px solid black; border-collapse: collapse; } </style> </head> <body lang=PL> <div class=WordSection1>";
        public const string HTML_ZakonczenieWykazuWydzEkwiw = "</body></html>";

        public static String GenerujWykazWE(JR_Nowa JednoskaRejNowa)
        {
            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine("<div style=\"text-align: right;\"> <span style=\"color: red\">NUMER GOSPODASTWA &nbsp;</span>  <span style = \"color: blue; text-decoration: underline;\">" + JednoskaRejNowa.IjrPo + "</span></div>");
            dokHTML.AppendLine("<div><span>Obręb:&nbsp;" + JednoskaRejNowa.NrObr + "&nbsp;" + JednoskaRejNowa.NazwaObrebu + "</span></div>");
            dokHTML.AppendLine("<div><span>Numer jednostki rejestrowej " + JednoskaRejNowa.Nkr + "</span><br /><span style=\"margin-bottom: 0; padding: 0; \" >Właściciele i władający</span> <hr style=\"height: 2px; background-color: black; border: 0px; margin-top: 0; margin-bottom: 0; \"></div>");




            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PodzialSekcjiNaStronieNieparzystej);
            return dokHTML.ToString();
        }
    }

 
}
