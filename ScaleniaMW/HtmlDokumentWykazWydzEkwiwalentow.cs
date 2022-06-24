using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScaleniaMW
{
    class HtmlDokumentWykazWydzEkwiwalentow : HtmlDokument, IHTMLDokument
    {
        public static String GenerujKarteWykazuWE(JR_Nowa jednoskaRejNowa)
        {
            decimal SumaWartosciPrzed = jednoskaRejNowa.SumaWartJednostekPrzed();
            decimal SumaWartosciPo = jednoskaRejNowa.Dzialki_Nowe.Sum(x => x.Wartosc);

            StringBuilder dokHTML = new StringBuilder();

            // Numer gospodarstwa <4001>
            dokHTML.AppendLine(HTML_NaglowekNrGosp(jednoskaRejNowa.IjrPo));

            //Obręb: <4> <Eliaszuki>
            dokHTML.AppendLine(HTML_NaglowekObreb(jednoskaRejNowa.NrObr, jednoskaRejNowa.NazwaObrebu));

            // Numer jednostki rejestrowej <1>
            dokHTML.AppendLine(HTML_NaglowekJednostkaRejestrowaPrzed(jednoskaRejNowa.Nkr));

            // Jan Kowalski 
            // Eliaszuki 5, 10-100 Narewka
            dokHTML.AppendLine(HTML_TabelaWlascicieleIWladajacy(jednoskaRejNowa.Wlasciciele));

            // Uwaga pisana na czarwono
            dokHTML.AppendLine(HTML_Uwaga(jednoskaRejNowa.Uwaga));

            dokHTML.AppendLine(HTML_TytulZakreslony("Ekwiwalent należny wg. udziału w pozycjach rejestrowych"));

            // [Obreb] [Nazwa obrębu] [Nr jedn.] [Udzial] [Powierzchnia] [Należność]
            //        Powierzchnia/Warość przed scaleniem [Suma pow.]    [Suma należn.]
            dokHTML.AppendLine(HTML_TabelaEkwiwalentuNaleznego(jednoskaRejNowa));

            dokHTML.AppendLine(HTML_TytulZakreslony("Ekwiwalent należny/zaprojektowany"));


            // Tabela należny / zaprojektowany
            // [                      Ekwiwalent                     ]
            // [           Należny        ][      Zaprojektowany     ]
            // [Dzialka] [Pow] [Wart] [KW] [Dzialka] [Pow] [Wart] [KW]
            if (jednoskaRejNowa.zJednRejStarej.Count > 0)
            {
                List<Dzialka_N> tmpDzWypisaneWWykazie = new List<Dzialka_N>();

                foreach (var jednostkaStara in jednoskaRejNowa.zJednRejStarej)
                {
                    dokHTML.AppendLine(HTML_NaglowekObreb(jednostkaStara.NrObr, jednostkaStara.NazwaObrebu));
                    dokHTML.AppendLine(HTML_NaglowekJednostkaRejestrowaPrzed(jednostkaStara.Ijr_Przed));

                    if (CzyGenerowacWlascicieliZStarychJEdnostek(jednoskaRejNowa))
                    {
                        dokHTML.AppendLine(HTML_NaglowekTabelaWlascicieleIWladajacy(jednostkaStara.Wlasciciele));
                    }

                    if (jednoskaRejNowa.Dzialki_Nowe.Exists(x => x.RjdrPrzed == jednostkaStara.Id_Jedns))
                    {
                        JR_Nowa jR_Nowa = jednoskaRejNowa.JednostkaZDzialkamiZRJDRPrzed(jednostkaStara.Id_Jedns);
                        dokHTML.AppendLine(HTML_TabelaZDzialkami(jR_Nowa, jednostkaStara));

                        jR_Nowa.Dzialki_Nowe.ForEach(x => tmpDzWypisaneWWykazie.Add(new Dzialka_N(x)));


                        // kontrola
                        if (jR_Nowa.Dzialki_Nowe.Exists(x => x.Id_obr != jednostkaStara._id_obr))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"Poniższe działki przypisano do innego obrębu\nniż same pochodzą! ({jednostkaStara.NrObr} {jednostkaStara.NazwaObrebu})");
                            jR_Nowa.Dzialki_Nowe.FindAll(x => x.Id_obr != jednostkaStara._id_obr).ForEach(x => sb.AppendLine(x.NrObr + "-" + x.NrDz));
                            MessageBox.Show(sb.ToString(), "Error");
                        }
                    }
                    else
                    {
                        dokHTML.AppendLine(HTML_TabelaZDzialkami(null, jednostkaStara));
                    }
                }


                // sprawdzenie jakich jeszcze działek po scaleniu nie wrzucono do rejestru.
                List<Dzialka_N> tmpDzPominiete = new List<Dzialka_N>();
                foreach (var item in jednoskaRejNowa.Dzialki_Nowe)
                {
                    if (!(tmpDzWypisaneWWykazie.FindAll(x => x.Id_dz == item.Id_dz).Count > 0))
                    {
                        tmpDzPominiete.Add(new Dzialka_N(item));
                        Console.WriteLine(item.NrDz);
                    }
                }

                // wygenerowanie tych działek w kolejnych tabelach.
                foreach (var obr in tmpDzPominiete.Select(x => new { idobr = x.Id_obr, nrOb = x.NrObr, nazwaObr = x.NazwaObrebu }).Distinct())
                {
                    JR_Nowa jR_Nowa = new JR_Nowa(jednoskaRejNowa, tmpDzPominiete.FindAll(x => x.Id_obr == obr.idobr));
                    dokHTML.AppendLine(HTML_NaglowekObreb(obr.nrOb, obr.nazwaObr));
                    dokHTML.AppendLine(HTML_TabelaZDzialkami(jR_Nowa, null));
                }

            }
            else // Przypadek gdy jest tylko stan PO
            {
                foreach (var obrebyPoScaleniu in jednoskaRejNowa.Dzialki_Nowe.Select(x => new { x.NrObr, x.NazwaObrebu, x.Id_obr }).Distinct())
                {
                    dokHTML.AppendLine(HTML_NaglowekObreb(obrebyPoScaleniu.NrObr, obrebyPoScaleniu.NazwaObrebu));
                    //JR_Nowa jrn = new JR_Nowa(jednoskaRejNowa, jednoskaRejNowa.Dzialki_Nowe.FindAll(x => x.Id_obr == obrebyPoScaleniu.Id_obr));
                    JR_Nowa jrn = jednoskaRejNowa.JednostkaZDzialkamiZObrebu(obrebyPoScaleniu.Id_obr);
                    dokHTML.AppendLine(HTML_TabelaZDzialkami(jrn, null));

                }

            }

            // BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW
            //dokHTML.AppendLine("<br /> <div style=\"text-align: center; margin-bottom: 5px;\"> <span style=\"color: red\"><b><u>BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW</u></b></span></div>");
            dokHTML.AppendLine(HTML_TytulCzerwonyPodkreslony("BILANS POWIERZCHNI I WARTOŚCI WYDZIELONYCH EKWIWALENTÓW"));

            // Tabelka czerwona BILANSU
            dokHTML.AppendLine(HTML_CzerwonkaTabelka(jednoskaRejNowa));

            dokHTML.AppendLine("<br/>");

            dokHTML.AppendLine(HTML_TabelaDoplata(jednoskaRejNowa));


            return dokHTML.ToString();
        }

        public string GenerujWWE(List<JR_Nowa> jR_Nowa)
        {
            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine(HtmlDokument.HTML_PoczatekWykazyWydzEkwiwalentow());
            dokHTML.AppendLine(HtmlDokument.HTML_PodzialSekcjiNaStronieNieparzystej);
            bool podzialSekcjiNaStronieNieparzystej = true;
            foreach (var JednoskaRejNowa in jR_Nowa)
            {
                dokHTML.Append(HtmlDokumentWykazWydzEkwiwalentow.GenerujKarteWykazuWE(JednoskaRejNowa));
                if (podzialSekcjiNaStronieNieparzystej)
                {
                    dokHTML.AppendLine(HtmlDokument.HTML_PodzialSekcjiNaStronieNieparzystej);
                }
                else
                {
                    dokHTML.AppendLine(HtmlDokument.HTML_PodzialNowaStrona);
                }
            }
            dokHTML.AppendLine(HtmlDokument.HTML_ZakonczenieWykazuWydzEkwiw);
            //JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.AppendText("W jednostce: " + x.IjrPo.ToString() + " brakuje numeru obrębu"));
            return dokHTML.ToString();
        }
    }
}
