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
        static readonly string NKRzNieprzypisanymIjr = "select distinct jn.id_id ID,  jn.ijr nowy__nkr, dn.idd nr__dz from  JEDN_SN sn join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id where dn.rjdrprzed is null or dn.rjdrprzed = '' order by id_jednn";
        static readonly string NKRzPodejrzanymNrIjr = "select  js.ijr stara_jedn_ewop, js.id_id staraId, jn.ijr nowy_nkr, dn.idd, dn.rjdrprzed , (select ijr from jedn_rej where dn.rjdrprzed = id_id ) Przypisany_IJR from  JEDN_SN sn  join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id order by id_jednn";

        class ModelJednostekPodejrzanych
        {
            public int js_IJR { get; set; }
            public int js_Id_Id { get; set; }
            public int jn_Ijr { get; set; }
            public string dn_Idd { get; set; }
            public int? dn_Rjdrprzed { get; set; }
            public int? dn_rjdrjJakoIJR { get; set; }
           public void wypisz()
            {
                Console.WriteLine(jn_Ijr + " " + js_Id_Id + " " + jn_Ijr + " " + dn_Idd + " " + dn_Rjdrprzed);
            }
        }



        public static void NkrZPodejrzanymIJRem()
        {
            List<ModelJednostekPodejrzanych> listaJednPodejrzanych = new List<ModelJednostekPodejrzanych>();

            DataTable dt = BazaFB.Get_DataTable(NKRzPodejrzanymNrIjr);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Console.WriteLine("0 " + dt.Rows[i][0]);
                Console.WriteLine("1 " +dt.Rows[i][1]);
                Console.WriteLine("2 " +dt.Rows[i][2]);
                Console.WriteLine("3 " +dt.Rows[i][3]);
                Console.WriteLine("4 " + dt.Rows[i][4]);
                Console.WriteLine("5 " + dt.Rows[i][5]);

                listaJednPodejrzanych.Add(new ModelJednostekPodejrzanych
                {
                    js_IJR = Convert.ToInt32(dt.Rows[i][0]),
                    js_Id_Id = Convert.ToInt32(dt.Rows[i][1]),
                    jn_Ijr = Convert.ToInt32(dt.Rows[i][2]),
                    dn_Idd = dt.Rows[i][3].ToString(),
                    dn_Rjdrprzed = dt.Rows[i][4].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(dt.Rows[i][4]),
                    dn_rjdrjJakoIJR = dt.Rows[i][5].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(dt.Rows[i][5])
                });
            }
            foreach (var item in listaJednPodejrzanych)
            {
                item.wypisz();
            }

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
                    dzKw.Add(KW);
                }
            }
            return dzKw;
        }
    }
}
