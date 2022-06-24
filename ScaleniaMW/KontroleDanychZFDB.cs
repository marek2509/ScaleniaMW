using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ScaleniaMW
{
    static class KontroleDanychZFDB
    {
        static readonly string SqlKwPrzed = "select  o.id || '-' || d.idd dzialka, d.kw  from dzialka   d join obreby o on o.id_id = d.idobr where(status<> 1 or status is null) and kw<> '' and kw is not null";
        static readonly string SqlKwPo = "select  o.id || '-' || d.idd dzialka, d.kw  from dzialki_n   d join obreby o on o.id_id = d.idobr where kw<> '' and kw is not null";
        static readonly string SQLNKRzNieprzypisanymIjr = "select distinct jn.id_id ID,  jn.ijr nowy__nkr, dn.idd nr__dz from  JEDN_SN sn join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id where dn.rjdrprzed is null or dn.rjdrprzed like '' order by id_jednn";
        static readonly string SQLNKRzPodejrzanymNrIjr = "select  js.ijr stara_jedn_ewop, js.id_id staraId, jn.ijr nowy_nkr, id_jednn, dn.idd, dn.rjdrprzed , (select ijr from jedn_rej where dn.rjdrprzed = id_id ) Przypisany_IJR from  JEDN_SN sn  join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id order by id_jednn";
        static readonly string SQLUdzialyJednPrzedWStaniePo = @"select sum(ud_nr) SUMA__UDZIALU , js.ijr, o.naz obreb from jedn_sn jsn join jedn_rej js on js.Id_id = jsn.id_jedns join obreby o on o.id_id = js.id_obr group by id_jedns, js.ijr, o.naz HAVING sum(ud_nr)  <> 1 order by SUMA__UDZIALU";
        static readonly string SQLsprawdzenieSumWartosciZDzialekIZJednRej = @"select ijr, (select sum(round(wwgsp,2)) from jedn_sn  where id_jednn = j2.id_id group by  id_jednn) sumA_z_GOSPODARSTW ,(select sum(d.ww*jsn.ud_nr) from dzialka d join jedn_rej j on j.ID_ID = d.rjdr join jedn_sn jsn on jsn.id_jedns=j.id_id join jedn_rej_n jn on jn.id_id = jsn.id_jednn where j2.id_id = jn.id_id and (jn.id_sti <> 1 or jn.id_sti is null )group by jsn.id_jednn) suma_Z_DZIALEK, round((select  sum(wwgsp) from jedn_sn  where id_jednn = j2.id_id group by  id_jednn) - (select sum(d.ww*jsn.ud_nr) from dzialka d join jedn_rej j on j.ID_ID = d.rjdr join jedn_sn jsn on jsn.id_jedns=j.id_id join jedn_rej_n jn on jn.id_id = jsn.id_jednn where j2.id_id = jn.id_id and (jn.id_sti <> 1 or jn.id_sti is null )group by jsn.id_jednn) ,2) roznica from jedn_rej_n j2 where id_sti <> 1 or id_sti is null order by roznica, ijr";
        static readonly string SQLbrakGrupyRejestrowejDlaJednostkiIWlascPrzed = @"select ijr, nkr, grj GRUPA_WLASC, gr grupa_JEDNOSTKI from udzialy  u join jedn_rej j on j.id_id = u.id_jedn where grj is null or grj like '' or gr is null or gr like '' group by ijr, nkr, grj, gr order by NKR";
        static readonly string SQLbrakGrupyRejestrowejDlaJednostkiIWLASCPo = @"select ijr, nkr, grj GRUPA_WLASC, gr grupa_JEDNOSTKI from udzialy_n  u join jedn_rej_n j on j.id_id = u.id_jedn where grj is null or grj like '' or gr is null or gr like '' group by ijr, nkr, grj, gr order by NKR";
        //static readonly string SQLPrzedJednoskiZUdzialamiRoznymiOd1 = @"select o.id obreb, j.ijr, sum( u.ud_nr) from jedn_rej j join udzialy u on u.id_jedn = j.id_id left join obreby o on o.id_id = j.id_obr group by j.id_id, ijr, o.id having sum(u.ud_nr) <> 1";
        static readonly string SQLPrzedJednoskiZUdzialamiRoznymiOd1 = @"select o.id obreb, j.ijr, sum( u.ud_nr) suma_udzialow, (select first 1 sum(u2.ud_nr) from jedn_rej j2 join udzialy u2 on u2.id_jedn = j2.id_id where j2.id_id = j.id_id and u2.rwd <> 11) suma_udzialow_innych_NIz_wl from jedn_rej j join udzialy u on u.id_jedn = j.id_id left join obreby o on o.id_id = j.id_obr group by j.id_id, ijr, o.id having sum(u.ud_nr) <> 1";
        //static readonly string SQLPoJednoskiZUdzialamiRoznymiOd1 = @"select o.id obreb, j.ijr, sum( u.ud_nr) from jedn_rej_n j join udzialy_n u on u.id_jedn = j.id_id left join obreby o on o.id_id = j.id_obr group by j.id_id, ijr, o.id having sum(u.ud_nr) <> 1";
        static readonly string SQLPoJednoskiZUdzialamiRoznymiOd1 = @"select o.id obreb, j.ijr, sum( u.ud_nr), (select first 1 sum(u2.ud_nr) from jedn_rej_n j2 join udzialy_N u2 on u2.id_jedn = j2.id_id where j2.id_id = j.id_id and u2.rwd <> 11)  suma_udzialow_innych_NIz_wl from jedn_rej_n j join udzialy_n u on u.id_jedn = j.id_id left join obreby o on o.id_id = j.id_obr group by j.id_id, ijr, o.id having sum(u.ud_nr) <> 1";

        public static DataTable udzialyRozneOd1Przed() //zwraca listę złych numerów KW
        {
            return BazaFB.Get_DataTable(SQLPrzedJednoskiZUdzialamiRoznymiOd1);
        }

        public static DataTable udzialyRozneOd1Po() //zwraca listę złych numerów KW
        {
            return BazaFB.Get_DataTable(SQLPoJednoskiZUdzialamiRoznymiOd1);
        }

        public class ModelJednostekPodejrzanych
        {
            public int js_IJR;
            public int js_Id_Id;
            public int NKR { get; set; }
            public int id_jednn;
            public string NR_DZIAŁKI { get; set; }
            public int? dn_Rjdrprzed;
            public int? PRZYPISANY__RJDR { get; set; }

            public void wypisz()
            {
                Console.WriteLine(js_IJR + " " + js_Id_Id + " " + NKR + " " + id_jednn + " " + NR_DZIAŁKI + " " + dn_Rjdrprzed + " " + PRZYPISANY__RJDR);
            }
        }

        class JednkostkiKtoreMogaByc // NKR nowy oraz dostępne do przypisania jednostki przed [RJDRPRZED] w List<>
        {
            public class JednstkiPrzed
            {
                public int js_id_id { get; set; }
                public int Js_ijr { get; set; }
            }
            public JednkostkiKtoreMogaByc()
            {
                IJR_Przed = new List<JednstkiPrzed>();
            }


            public int NKR { get; set; }
            public int Id_NKR { get; set; }
            public List<JednstkiPrzed> IJR_Przed { get; set; }

            public void DodajIjrPrzed(int js_idid, int js_ijr)
            {
                IJR_Przed.Add(new JednstkiPrzed { js_id_id = js_idid, Js_ijr = js_ijr });
            }
        }

        public static List<ModelJednostekPodejrzanych> NkrZPodejrzanymIJRem()
        {
            List<ModelJednostekPodejrzanych> surweZBazy = new List<ModelJednostekPodejrzanych>();
            List<JednkostkiKtoreMogaByc> jednkostkiKtoreMogaBycList = new List<JednkostkiKtoreMogaByc>();
            DataTable dt = BazaFB.Get_DataTable(SQLNKRzPodejrzanymNrIjr);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                /*Console.WriteLine("0 " + dt.Rows[i][0]);
                Console.WriteLine("1 " + dt.Rows[i][1]);
                Console.WriteLine("2 " + dt.Rows[i][2]);
                Console.WriteLine("3 " + dt.Rows[i][3]);
                Console.WriteLine("4 " + dt.Rows[i][4]);
                Console.WriteLine("5 " + dt.Rows[i][5]);*/

                surweZBazy.Add(new ModelJednostekPodejrzanych
                {
                    js_IJR = Convert.ToInt32(dt.Rows[i][0]),
                    js_Id_Id = Convert.ToInt32(dt.Rows[i][1]),
                    NKR = Convert.ToInt32(dt.Rows[i][2]),
                    id_jednn = Convert.ToInt32(dt.Rows[i][3]),
                    NR_DZIAŁKI = dt.Rows[i][4].ToString(),
                    dn_Rjdrprzed = dt.Rows[i][5].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(dt.Rows[i][5]),
                    PRZYPISANY__RJDR = dt.Rows[i][6].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(dt.Rows[i][6])
                });
            }

            //            tmpListNKRbezJednRej.GroupBy(x => new { x.NKRn, x.IdJednN }).Select(x => new IDiNKR { IdJednN = x.Key.IdJednN, NKR = x.Key.NKRn }).ToList();
            var tmp = surweZBazy.Select(x => new JednkostkiKtoreMogaByc { Id_NKR = x.id_jednn, NKR = x.NKR, IJR_Przed = surweZBazy.Where(G => G.id_jednn == x.id_jednn).Select(r => new JednkostkiKtoreMogaByc.JednstkiPrzed { js_id_id = r.js_Id_Id, Js_ijr = r.js_IJR }).ToList() });


            List<JednkostkiKtoreMogaByc> jktMogaByc = new List<JednkostkiKtoreMogaByc>();
            foreach (var item in tmp)
            {
                if (!jktMogaByc.Exists(x => x.Id_NKR == item.Id_NKR && x.NKR == item.NKR))
                {
                    jktMogaByc.Add(new JednkostkiKtoreMogaByc { Id_NKR = item.Id_NKR, NKR = item.NKR });
                }
                if (jktMogaByc.Exists(x => x.Id_NKR == item.Id_NKR && x.NKR == item.NKR))
                {
                    foreach (var jednSt in item.IJR_Przed)
                    {
                        if (!jktMogaByc.Find(x => x.Id_NKR == item.Id_NKR && x.NKR == item.NKR).IJR_Przed.Exists(y => y.js_id_id == jednSt.js_id_id))
                        {
                            jktMogaByc.Find(x => x.Id_NKR == item.Id_NKR && x.NKR == item.NKR).DodajIjrPrzed(jednSt.js_id_id, jednSt.Js_ijr);
                        }
                    }
                }
            }

            List<JednkostkiKtoreMogaByc> jednPrzypisaneWStaniePo = new List<JednkostkiKtoreMogaByc>();
            var tmpPrzypisane = surweZBazy.Select(x => x).Where(x => x.dn_Rjdrprzed != null);

            List<ModelJednostekPodejrzanych> modelJednosteks = new List<ModelJednostekPodejrzanych>();
            foreach (var item in tmpPrzypisane)
            {
                if (jktMogaByc.Find(x => x.Id_NKR == item.id_jednn).IJR_Przed.Exists(y => y.js_id_id == item.dn_Rjdrprzed))
                {

                }
                else
                {
                    modelJednosteks.Add(item);
                }
            }
            return modelJednosteks;
        }

        public static DataView WypiszNkrZNieprzypiasnymNrIJR()
        {
            return BazaFB.Get_DataTable(SQLNKRzNieprzypisanymIjr).AsDataView();
        }

        public class DzKW
        {
            public string Obr_Dz { get; set; }
            public string KW { get; set; }
        }

        public static List<DzKW> sprawdzKwPoScaleniu()
        {
            return sprawdzKw(SqlKwPo);
        }

        public static List<DzKW> sprawdzKwPrzedScaleniem()
        {
            return sprawdzKw(SqlKwPrzed);
        }

        static List<DzKW> sprawdzKw(string SqlDzKw) //zwraca listę złych numerów KW
        {
            DataTable dt = BazaFB.Get_DataTable(SqlDzKw);
            List<DzKW> tmpDzKw = new List<DzKW>();
            List<DzKW> dzKw = new List<DzKW>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tmpDzKw.Add(new DzKW { Obr_Dz = dt.Rows[i][0].ToString(), KW = dt.Rows[i][1].ToString() });
            }

            foreach (var KW in tmpDzKw)
            {
                if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolnaBool(KW.KW))
                {
                    if (KW.KW != "" && dzKw != null)
                        dzKw.Add(KW);
                }
            }
            return dzKw;
        }

        public static DataTable UdzialyPrzedWStaniePo()
        {
            return BazaFB.Get_DataTable(SQLUdzialyJednPrzedWStaniePo);
        }

        public static DataTable jednostkiBezGrupRejestrowychPrzed()
        {
            return BazaFB.Get_DataTable(SQLbrakGrupyRejestrowejDlaJednostkiIWlascPrzed);
        }

        public static DataTable jednostkiBezGrupRejestrowychPo()
        {
            return BazaFB.Get_DataTable(SQLbrakGrupyRejestrowejDlaJednostkiIWLASCPo);
        }

        public static List<ModelSWartosciZDzialekIZjednRej> sprawdzenieSumWartosci()
        {
            List<ModelSWartosciZDzialekIZjednRej> listaWarosci = new List<ModelSWartosciZDzialekIZjednRej>();

            DataTable dt = BazaFB.Get_DataTable(SQLsprawdzenieSumWartosciZDzialekIZJednRej);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
               
                listaWarosci.Add(new ModelSWartosciZDzialekIZjednRej
                {
                    NKR = Convert.ToInt32(dt.Rows[i][0].Equals(DBNull.Value) ? 0 : dt.Rows[i][0]),
                    WARTOŚĆ__Z__GOSPODARSTW = Convert.ToDouble(dt.Rows[i][1].Equals(DBNull.Value) ? 0 : dt.Rows[i][1]),
                    WARTOŚĆ__Z__DZIAŁEK = Convert.ToDouble(dt.Rows[i][2].Equals(DBNull.Value) ? 0 : dt.Rows[i][2]),
                    RÓŻNICA = Convert.ToDouble(dt.Rows[i][3].Equals(DBNull.Value) ? 0 : dt.Rows[i][3])
                });
            }
            return listaWarosci.FindAll(x => x.RÓŻNICA != 0);
        }

        public static void pobierzWlascicieliZBazy()
        {
            try
            {
                ListaObrebow.ClearData();
                JednostkiRejestroweNowe.ClearData();
                Potracenie potracenie = new Potracenie();

                DataTable obreby = BazaFB.Get_DataTable(Constants.SQL_Obreby);
                for (int i = 0; i < obreby.Rows.Count; i++)
                {
                    ListaObrebow.DodajObreb(Convert.ToInt32(obreby.Rows[i][0]), Convert.ToInt32(obreby.Rows[i][1]), obreby.Rows[i][2].ToString());
                }

                DataTable jednostkaRejestrowa = BazaFB.Get_DataTable(Constants.SQLJedn_rej_N);
                for (int i = 0; i < jednostkaRejestrowa.Rows.Count; i++)
                {
                    int idJednRejN = jednostkaRejestrowa.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][0]);
                    int ijr = jednostkaRejestrowa.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][1]);
                    int nkr = jednostkaRejestrowa.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][2]);
                    bool odcht = jednostkaRejestrowa.Rows[i][3].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][3]);
                    bool zgoda = jednostkaRejestrowa.Rows[i][4].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][4]);
                    string uwaga = jednostkaRejestrowa.Rows[i][5].ToString().Equals(DBNull.Value) ? "" : jednostkaRejestrowa.Rows[i][5].ToString();
                    int idobr = jednostkaRejestrowa.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][6]);

                    bool dpldr = jednostkaRejestrowa.Rows[i][7].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][7]);
                    bool zerujDopla = jednostkaRejestrowa.Rows[i][8].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][8]);
                    JednostkiRejestroweNowe.DodajJrNowa(idJednRejN, ijr, nkr, odcht, zgoda, uwaga, idobr, dpldr, zerujDopla);
                }

                DataTable WlascicielePO = BazaFB.Get_DataTable(Constants.SQLWlascicieleAdresyUdziayIdNKRNOWY);
                for (int i = 0; i < WlascicielePO.Rows.Count; i++)
                {
                    int idJednN = WlascicielePO.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePO.Rows[i][0]);
                    string udzial = WlascicielePO.Rows[i][1].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][1].ToString();
                    double udzial_NR = WlascicielePO.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToDouble(WlascicielePO.Rows[i][2]);
                    string nazwaWlasciciela = WlascicielePO.Rows[i][3].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][3].ToString();
                    string adres = WlascicielePO.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][4].ToString();
                    int idMalzenstwa = WlascicielePO.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePO.Rows[i][5]);
                    string symbolWl = WlascicielePO.Rows[i][6].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][6].ToString();

                    Wlasciciel wlasciciel = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa, symbolWl);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == idJednN).DodajWlasciciela(wlasciciel);
                }

                DataTable Jedn_SN = BazaFB.Get_DataTable(Constants.SQLJedn_SN);
                for (int i = 0; i < Jedn_SN.Rows.Count; i++)
                {
                    int idJednN = Jedn_SN.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][0]);
                    int idJednS = Jedn_SN.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][1]);
                    int IJR = Jedn_SN.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][2]);
                    int Idobr = Jedn_SN.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][3]);
                    string Ud_z_Jedn = Jedn_SN.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : Jedn_SN.Rows[i][4].ToString();
                    decimal WrtPrzed = Jedn_SN.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Jedn_SN.Rows[i][5]);

                    double PowPrzed = Jedn_SN.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Jedn_SN.Rows[i][6]);

                    bool czyJestPotracenie = Jedn_SN.Rows[i][7].Equals(DBNull.Value) ? false : Convert.ToBoolean(Jedn_SN.Rows[i][7]);

                    ZJednRejStarej zJednRej = new ZJednRejStarej(idJednS, IJR, Ud_z_Jedn, WrtPrzed, PowPrzed, Idobr, potracenie.WartoscPotracenia, czyJestPotracenie);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == idJednN).DodajZJrStarej(zJednRej);
                }

                DataTable Dzialki_nowe = BazaFB.Get_DataTable(Constants.SQL_Dzialki_N);
                for (int i = 0; i < Dzialki_nowe.Rows.Count; i++)
                {

                    int Id_dz = Dzialki_nowe.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][0]);
                    int Id_obr = Dzialki_nowe.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][1]);
                    string NrDz = Dzialki_nowe.Rows[i][2].ToString().Equals(DBNull.Value) ? "" : Dzialki_nowe.Rows[i][2].ToString();
                    double PowDz = Dzialki_nowe.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Dzialki_nowe.Rows[i][3]);
                    int Rjdr = Dzialki_nowe.Rows[i][4].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][4]);
                    int RjdrPrzed = Dzialki_nowe.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][5]);
                    string KW = Dzialki_nowe.Rows[i][6].ToString().Equals(DBNull.Value) ? "" : Dzialki_nowe.Rows[i][6].ToString();
                    decimal Wartosc = Dzialki_nowe.Rows[i][7].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Dzialki_nowe.Rows[i][7]);

                    Dzialka_N dzialka = new Dzialka_N(Id_dz, Id_obr, NrDz, PowDz, Rjdr, RjdrPrzed, KW, Wartosc);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == Rjdr).DodajDzialke(dzialka);

                }

                DataTable Dzialki_przed = BazaFB.Get_DataTable(Constants.SQL_Dzialka);
                List<Dzialka> DzialkaStaraTMP = new List<Dzialka>();
                for (int i = 0; i < Dzialki_przed.Rows.Count; i++)
                {

                    int Id_dz = Dzialki_przed.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][0]);
                    int Id_obr = Dzialki_przed.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][1]);
                    string NrDz = Dzialki_przed.Rows[i][2].ToString().Equals(DBNull.Value) ? "" : Dzialki_przed.Rows[i][2].ToString();
                    double PowDz = Dzialki_przed.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Dzialki_przed.Rows[i][3]);
                    int Rjdr = Dzialki_przed.Rows[i][4].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][4]);
                    string KW = Dzialki_przed.Rows[i][5].ToString().Equals(DBNull.Value) ? "" : Dzialki_przed.Rows[i][5].ToString();
                    decimal Wartosc = Dzialki_przed.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Dzialki_przed.Rows[i][6]);

                    DzialkaStaraTMP.Add(new Dzialka(Id_dz, Id_obr, NrDz, PowDz, Rjdr, KW, Wartosc));
                }

                foreach (var JN in JednostkiRejestroweNowe.Jedn_REJ_N)
                {
                    foreach (var JRst in JN.zJednRejStarej)
                    {
                        foreach (var dzialkaPrzed in DzialkaStaraTMP.FindAll(x => x.Rjdr == JRst.Id_Jedns))
                        {
                            JRst.DodajDzialkePrzed(dzialkaPrzed);
                        }
                    }
                }

                DataTable WlascicielePrzed = BazaFB.Get_DataTable(Constants.SQL_WlascicielAdresyUdzialyIdNkrStary);
                List<WlascicielStanPrzed> WlascicielePrzedTMP = new List<WlascicielStanPrzed>();
                for (int i = 0; i < WlascicielePrzed.Rows.Count; i++)
                {
                    int idJednS = WlascicielePrzed.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePrzed.Rows[i][0]);
                    string udzial = WlascicielePrzed.Rows[i][1].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][1].ToString();
                    double udzial_NR = WlascicielePrzed.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToDouble(WlascicielePrzed.Rows[i][2]);
                    string nazwaWlasciciela = WlascicielePrzed.Rows[i][3].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][3].ToString();
                    string adres = WlascicielePrzed.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][4].ToString();
                    int idMalzenstwa = WlascicielePrzed.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePrzed.Rows[i][5]);
                    string symbolWlad = WlascicielePrzed.Rows[i][6].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][6].ToString();

                    // Wlasciciel wlascicielPrzed = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa);
                    WlascicielePrzedTMP.Add(new WlascicielStanPrzed(idJednS, udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa, symbolWlad));
                }

                foreach (var JN in JednostkiRejestroweNowe.Jedn_REJ_N)
                {
                    foreach (var JRst in JN.zJednRejStarej)
                    {
                        foreach (var wlascicielPrzed in WlascicielePrzedTMP.FindAll(x => x.IdJednPrzed == JRst.Id_Jedns))
                        {
                            JRst.DodajWlascicielaWStaniePrzed(wlascicielPrzed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błą połączenia z bazą" + " " + ex.Message);
            }
        }

        static bool CzyJestRóżnaWlasnosc(JR_Nowa JednoskaRejNowa)
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

        static List<int> idNkrZeZmiana;
        public static string GenerujTabeleRroznychWlasnosci()
        {
            idNkrZeZmiana = new List<int>();
            pobierzWlascicieliZBazy();
            StringBuilder sb = new StringBuilder();
            foreach (var JednoskaRejNowa in JednostkiRejestroweNowe.Jedn_REJ_N)
            {
                if (CzyJestRóżnaWlasnosc(JednoskaRejNowa))
                {
                    idNkrZeZmiana.Add(JednoskaRejNowa.IdJednRejN);

                    sb.AppendLine("Obręb: " + JednoskaRejNowa.NazwaObrebu + "\nNKR: " + JednoskaRejNowa.IjrPo.ToString());
                    sb.AppendLine("----------------Własność PRZED scaleniem:");
                    JednoskaRejNowa.zJednRejStarej.ForEach(x => x.Wlasciciele.ForEach(y => sb.AppendLine(y.NazwaWlasciciela + " ADRES: " + y.Adres)));
                    sb.AppendLine("----------------Własność PO scaleniu:");
                    JednoskaRejNowa.Wlasciciele.ForEach(x => sb.AppendLine(x.NazwaWlasciciela + " ADRES: " + x.Adres));
                    sb.AppendLine();
                    sb.AppendLine("____________________________________________________________________________________________");
                }
            }

            var resultQuerySql = BazaFB.Get_DataTable("select jn.id_id, jn.ijr from jedn_sn jsn join jedn_rej js on js.id_id = jsn.id_jedns join jedn_rej_n jn on jn.id_id = jsn.id_jednn where (upper(js.historia) like '%NAZWA%' or upper(js.historia) like '%UDZIAŁ%') and js.dtw is not null order by jn.ijr");
            List<string> listaJednZeZmianamyWlasnosciIzmianaWStaniePrzed = new List<string>();
            for (int i = 0; i < resultQuerySql.Rows.Count; i++)
            {
                if (idNkrZeZmiana.Exists(x => x == Convert.ToInt32(resultQuerySql.Rows[i][0])))
                {
                    listaJednZeZmianamyWlasnosciIzmianaWStaniePrzed.Insert(0, "Id: " + resultQuerySql.Rows[i][0] + " NKR: " +  resultQuerySql.Rows[i][1]);
                }
            }

            sb.Insert(0, "____________________________________________________________________________________________\n");
            listaJednZeZmianamyWlasnosciIzmianaWStaniePrzed.Distinct().ToList().ForEach(x => sb.Insert(0,x+"\n"));
            if (listaJednZeZmianamyWlasnosciIzmianaWStaniePrzed.Count > 0)
            {
                sb.Insert(0, "Jednostki na które zwrócić szczególną uwagę, ponieważ były edytowane w stanie przed:\n");
            }

            return sb.ToString();
        }

        // koniec klasy 
    }

    class ModelSWartosciZDzialekIZjednRej
    {
        public int NKR { get; set; }
        public double WARTOŚĆ__Z__GOSPODARSTW { get; set; }
        public double WARTOŚĆ__Z__DZIAŁEK { get; set; }
        public double RÓŻNICA { get; set; }
    }


}
