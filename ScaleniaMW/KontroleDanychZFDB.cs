using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScaleniaMW
{
    static class KontroleDanychZFDB
    {
        static readonly string SqlKwPrzed = "select  o.id || '-' || d.idd dzialka, d.kw  from dzialka   d join obreby o on o.id_id = d.idobr where(status<> 1 or status is null) and kw<> '' and kw is not null";
        static readonly string SqlKwPo = "select  o.id || '-' || d.idd dzialka, d.kw  from dzialki_n   d join obreby o on o.id_id = d.idobr where kw<> '' and kw is not null";
        static readonly string NKRzNieprzypisanymIjr = "select distinct jn.id_id ID,  jn.ijr nowy__nkr, dn.idd nr__dz from  JEDN_SN sn join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id where dn.rjdrprzed is null or dn.rjdrprzed like '' order by id_jednn";
        static readonly string NKRzPodejrzanymNrIjr = "select  js.ijr stara_jedn_ewop, js.id_id staraId, jn.ijr nowy_nkr, id_jednn, dn.idd, dn.rjdrprzed , (select ijr from jedn_rej where dn.rjdrprzed = id_id ) Przypisany_IJR from  JEDN_SN sn  join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id order by id_jednn";
        static readonly string UdzialyJednPrzedWStaniePo = @"select sum(ud_nr) SUMA__UDZIALU , js.ijr, o.id obreb from jedn_sn jsn join jedn_rej js on js.Id_id = jsn.id_jedns join obreby o on o.id_id = js.id_obr group by id_jedns, js.ijr, o.id HAVING sum(ud_nr)  <> 1 order by SUMA__UDZIALU";




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
            DataTable dt = BazaFB.Get_DataTable(NKRzPodejrzanymNrIjr);
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
            return BazaFB.Get_DataTable(NKRzNieprzypisanymIjr).AsDataView();
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
            Console.WriteLine("ILE KW: " + dzKw.Count);
            return dzKw;
        }

        public static DataTable UdzialyPrzedWStaniePo()
        {
            return BazaFB.Get_DataTable(UdzialyJednPrzedWStaniePo);
        }
    }
}
