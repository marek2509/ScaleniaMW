using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ScaleniaMW
{
    public class Wspolnota
    {
        public class Jednostki_s
        {
            public int ID_ID { get; set; }
            public int NrObr
            {
                get => Wspolnota.listObreby.Exists(x => x.IdObrebu == ID_Obr) ? Wspolnota.listObreby.Find(x => x.IdObrebu == ID_Obr).NrObrebu : 0;

                private set
                {
                }
            }
            public int IJR { get; set; }
            public string NazwaObrebu
            {

                get => Wspolnota.listObreby.Exists(x => x.IdObrebu == (ID_Obr.Equals(DBNull.Value) ? -1 : ID_Obr)) ? Wspolnota.listObreby.Find(x => x.IdObrebu == (ID_Obr.Equals(DBNull.Value) ? -1 : ID_Obr)).Nazwa : "BRAK_OBREBU";

                private set
                {
                }
            }
            public decimal WartoscJednostki { get; set; }
            public double PEW { get; set; }
            public int? ID_Obr;
        }

        public class RWD
        {
            public int ID_ID { get; set; }
            public string Symbol { get; set; }
        }

        public static readonly string SQLIdpdmNazwaWlascAdres = "select distinct p.id_id, case when upper(p.typ) like 'F' then (select case when dim is null then nzw || ' ' || pim else nzw || ' ' ||  pim || ' ' || dim end from osoby o where p.id_os = o.id_id) when upper(p.typ) like 'P' then(select NPE from INSTYTUCJE i where p.id_os = i.id_id) when upper(p.typ) like 'I' then (select NPE from INNE_PODM inne where p.id_os = inne.id_id) when upper(p.typ) like 'M' then (select case when maz.dim is null and zona.dim is null then '' || maz.nzw  || ' '  ||  maz.pim || ' ' ||  zona.nzw || ' ' || zona.pim when maz.dim is null and zona is not null then '' || maz.nzw  || ' '  ||  maz.pim || ' ' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim when maz.dim is not null and zona.dim is null then '' || maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' ' ||  zona.nzw || ' ' || zona.pim else '' ||  maz.nzw  || ' '  ||  maz.pim || ' '  ||  maz.dim || ' ' ||  zona.nzw || ' ' || zona.pim || ' ' ||  zona.dim end from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Wlasciciele, case when upper(p.typ) like 'F' then(select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = p.id_os) when upper(p.typ) like 'P' then( select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from instytucje o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrlok is not null then nra || '/' || nrlok when nra is not null then nra else '' end from instytucje o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from instytucje o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from instytucje o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from instytucje o1 where o1.id_id = p.id_os) when upper(p.typ) like 'I' then(select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from INNE_PODM o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrlok is not null then nra || '/' || nrlok when nra is not null then nra else '' end from INNE_PODM o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from INNE_PODM o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from INNE_PODM o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from INNE_PODM o1 where p.id_os = o1.id_id) when upper(p.typ) like 'M' then(select '' || (select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = maz.id_id) || ' ' || (select replace(upper(replace((select case when msc is not null and pct is not null and msc <> pct and ulc is not null and msc <> '' and pct <> '' then msc || ' ul. ' || ulc when ulc is not null and ulc <> '' then 'ul. ' || ulc when msc is not null then msc when pct is not null then pct else '' end from osoby o2 where o1.id_id = o2.id_id)  || ' ' || (select  case when nra is not null and nrl is not null then nra || '/' || nrl when nra is not null then nra else '' end from osoby o2 where o1.id_id = o2.id_id)  || '; ' || (select case when kod is not null then kod else '' end  from osoby o2 where o1.id_id = o2.id_id) || ' ' || (select case when pct is not null then pct when msc is not null then msc else '' end from osoby o2 where o1.id_id = o2.id_id), ' ;', ';')),'UL.'  , 'ul.')  from osoby o1 where o1.id_id = zona.id_id) from malzenstwa m join osoby maz on maz.id_id = m.maz join osoby zona on zona.id_id = m.zona where p.id_os = m.id_id) end Adresy from udzialy u right join jedn_rej jn on jn.id_id = u.id_jedn join podmioty p on p.ID_ID = u.id_podm where jn.id_sti<> 1  or jn.id_sti is null order by Wlasciciele";
        public class Gmina
        {
            public int ID_ID { get; set; }
            public string Nazwa { get; set; }
        }
        public class ModelJednostkiTworzonejZeWspolnowy
        {
            public int id_podm { get; set; }
            public double ud_nr { get; set; }
            public string ud { get; set; }
            public int id_jedn_n { get; set; }
            public int IJR_n { get; set; }
            public int id_sortowania = 0;
        }
        public class Id_PodmId_Jedn_N
        {
            public int Id_Podm { get; set; }
            public int Id_Jedn_N { get; set; }
            public int NKR { get; set; }
        }

        private static void UpdateProgressJedn_rej()
        {
            progressBarJedn_rej.Value += 1;

        }

        private static void UpdateProgressJedn_SN()
        {
            progressBarJednSN.Value += 1;

        }

        private static void UpdateProgressUdzialy()
        {
            progressBarUzdialy.Value += 1;
        }

        private delegate void ProgressBarDelegate();
        private delegate void ProgressLabelDelegadate();

        //   progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);

        public static CheckBox checkBoxCzyDopisywacDoIstniejaczych = new CheckBox();
        public static DockPanel dpTworzenie = new DockPanel();
        public static ProgressBar progressBarJedn_rej = new ProgressBar();
        public static ProgressBar progressBarUzdialy = new ProgressBar();
        public static ProgressBar progressBarJednSN = new ProgressBar();
        public static List<int> listaIjrBedaceWBazie = new List<int>();
        public static List<Gmina> listGminy = new List<Gmina>();
        public static List<Obreb> listObreby = new List<Obreb>();
        public static List<Jednostki_s> listaJednostki_s = new List<Jednostki_s>();
        // public static List<Jednostki_s> listaJednostki_N = new List<Jednostki_s>();
        public static List<Jednostki_s> listaWybranychJednstek_s = new List<Jednostki_s>();
        public static List<ModelJednostkiTworzonejZeWspolnowy> jednTworzZeWspolnoty = new List<ModelJednostkiTworzonejZeWspolnowy>();
        public static List<RWD> listaRWD = new List<RWD>();
        public static List<Id_PodmId_Jedn_N> ListaId_PodmId_Jedn_N = new List<Id_PodmId_Jedn_N>(); // lista potrzebna do tego żeby rozpoznać przypisać podmioty do jednotek istniejących innych niż współnotowe
        //wstaw w tabele jedn_sn
        //insert into JEDN_SN(id_id,id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select first 1  id_id + 1 from JEDN_sn order by id_Id desc),0, @id_gm , @ud, @ud_nr(select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, 0, 0)

        public static int ileJednostekTrzebaUtworzyc()
        { //select id_podm, ud , ud_nr from udzialy_n u      join jedn_rej_n j on j.id_id = u.id_jedn  where  (j.id_obr = 1 and j.ijr = 11000) or(j.ijr = 20007 and j.id_obr = 2) or(j.ijr = 30002 and  j.id_obr = 3)   group by id_podm, ud , ud_nr

            List<int> id_Podm_kolejnosc = new List<int>();
            var sortowanie = BazaFB.Get_DataTable(SQLIdpdmNazwaWlascAdres);
            for (int i = 0; i < sortowanie.Rows.Count; i++)
            {
                id_Podm_kolejnosc.Add(Convert.ToInt32(sortowanie.Rows[i][0]));
            }


            string s = "select id_podm, ud , ud_nr from udzialy u      join jedn_rej j on j.id_id = u.id_jedn  where ";
            for (int i = 0; i < listaWybranychJednstek_s.Count; i++)
            {
                if (i == 0)
                {
                    s += " (j.id_obr = " + listaWybranychJednstek_s[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_s[i].IJR + ") ";
                }
                else
                {
                    s += "or (j.id_obr = " + listaWybranychJednstek_s[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_s[i].IJR + ") ";
                }
            }
            s += " group by id_podm, ud , ud_nr";
            DataTable dt = BazaFB.Get_DataTable(s);
            List<ModelJednostkiTworzonejZeWspolnowy> jednDoUtworz = new List<ModelJednostkiTworzonejZeWspolnowy>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jednDoUtworz.Add(new ModelJednostkiTworzonejZeWspolnowy { id_podm = Convert.ToInt32(dt.Rows[i][0]), ud = dt.Rows[i][1].ToString(), ud_nr = Convert.ToDouble(dt.Rows[i][2]), id_sortowania = id_Podm_kolejnosc.FindIndex(x => x == Convert.ToInt32(dt.Rows[i][0])) });
            }
            //    jednTworzZeWspolnoty = new List<ModelJednostkiTworzonejZeWspolnowy>( (List<ModelJednostkiTworzonejZeWspolnowy>)jednTworzZeWspolnoty.OrderBy(x => x.id_sortowania));
            jednTworzZeWspolnoty = jednDoUtworz.OrderBy(x => x.id_sortowania).ToList(); // pobranie danych do utworzenia noweych jednostek

            // wyszukiwanie jednostek do których można dopisać wspolnotę
            //var id_podm_jedn = BazaFB.Get_DataTable("select  id_podm, id_jedn, id_jedns from jedn_rej_n jn join udzialy_n u on u.id_jedn = jn.id_id join jedn_sn jsn on jsn.id_jednn = jn.id_id where jn.id_sti is null or jn.id_sti <> 1 group by ijr, id_podm, id_jedn, id_jedns");

            var id_podm_jedn = BazaFB.Get_DataTable(@"select ijr, id_podm, jn.id_id from jedn_rej_n jn join udzialy_n u on u.id_jedn = jn.id_id join jedn_sn jsn on jsn.id_jednn = jn.id_id where jn.id_sti is null or jn.id_sti <> 1 group by ijr, id_podm, jn.id_id  order by jn.id_id");

            for (int i = 0; i < id_podm_jedn.Rows.Count; i++)
            {
                    if (jednTworzZeWspolnoty.Exists(x => x.id_podm == Convert.ToInt32(id_podm_jedn.Rows[i][1])))
                    {
                        ListaId_PodmId_Jedn_N.Add(new Id_PodmId_Jedn_N { NKR = Convert.ToInt32(id_podm_jedn.Rows[i][0]), Id_Podm = Convert.ToInt32(id_podm_jedn.Rows[i][1]), Id_Jedn_N = Convert.ToInt32(id_podm_jedn.Rows[i][2]) });
                        //Console.WriteLine("Do edycji:" + id_podm_jedn.Rows[i][0].ToString() + " " + id_podm_jedn.Rows[i][1].ToString() + " " + id_podm_jedn.Rows[i][2].ToString());
                    }
            }

            //ListaId_PodmId_Jedn_N.ForEach(z => Console.WriteLine(z.Id_Podm));

            List<int> jakieIdJednUsunac = new List<int>();
            foreach (var item in ListaId_PodmId_Jedn_N)
            {
                if (ListaId_PodmId_Jedn_N.FindAll(x => x.Id_Jedn_N == item.Id_Jedn_N).Count > 1)
                {
                    // Console.WriteLine("SPRAWDZAM CZY SA DWA: ");
                    //   ListaId_PodmId_Jedn_N.FindAll(x => x.Id_Jedn_N == item.Id_Jedn_N).ForEach(x => Console.WriteLine(x.Id_Jedn_N + "JEDN I PODM" + x.Id_Podm));

                    jakieIdJednUsunac.Add(item.Id_Jedn_N);
                }
            }
            jakieIdJednUsunac = jakieIdJednUsunac.Distinct().ToList();
            foreach (var item in jakieIdJednUsunac)
            {
                ListaId_PodmId_Jedn_N.RemoveAll(x => x.Id_Jedn_N == item);
            }

            ListaId_PodmId_Jedn_N.ForEach(x =>
            {
                Console.WriteLine("jednostki istniejące " + x.NKR + " " + x.Id_Jedn_N + " " + x.Id_Podm);
            });
            return BazaFB.Get_DataTable(s).Rows.Count;
        }



        public static DataView sprawdzSpojnoscWybranychJednostek()
        {
            string s = "select id_podm, ud, count(id_podm)  from udzialy u join jedn_rej j on j.id_id = u.id_jedn where ";


            for (int i = 0; i < listaWybranychJednstek_s.Count; i++)
            {
                if (i == 0)
                {
                    s += " (j.id_obr = " + listaWybranychJednstek_s[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_s[i].IJR + ") ";
                }
                else
                {
                    s += "or (j.id_obr = " + listaWybranychJednstek_s[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_s[i].IJR + ") ";
                }
            }

            //   (j.id_obr = 1 and j.ijr = 11000) or(j.ijr = 20007 and j.id_obr = 2) or(j.ijr = 30002 and  j.id_obr = 3)
            s += " group by id_podm, ud having  count(id_podm) <>" + listaWybranychJednstek_s.Count;

            var db = BazaFB.Get_DataTable(s);
            Console.WriteLine("ile otrzymalem Elementow błędnych: " + db.Rows.Count);

            if (db.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return db.AsDataView();
            }

        }

        public static void pobierzGminy()
        {
            listGminy.Clear();
            var gm = BazaFB.Get_DataTable("select id_id, nazwa from gminy");
            for (int i = 0; i < gm.Rows.Count; i++)
            {
                listGminy.Add(new Gmina { ID_ID = Convert.ToInt32(gm.Rows[i][0]), Nazwa = gm.Rows[i][1].ToString() });
            }
        }

        public static void pobierzObreby()
        {
            listObreby.Clear();
            var obr = BazaFB.Get_DataTable("select id_id, id, naz from obreby where id_sti = 0");

            for (int i = 0; i < obr.Rows.Count; i++)
            {
                listObreby.Add(new Obreb(Convert.ToInt32(obr.Rows[i][0]), Convert.ToInt32(obr.Rows[i][1]), obr.Rows[i][2].ToString()));
            }
        }

        public static void pobierzJednostki()
        {
            listaIjrBedaceWBazie.Clear();
            listaWybranychJednstek_s.Clear();
            listaJednostki_s.Clear();
            listaRWD.Clear();
            // var jedn = BazaFB.Get_DataTable("select id_id, ijr, id_obr from jedn_rej where id_sti <> 1 or id_sti is null");
            // var jedn = BazaFB.Get_DataTable(@"select j.id_id, j.ijr, j.id_obr, sum(d.ww) Wartosc, sum(d.pew) PEWm2 from jedn_rej j join dzialka d on d.rjdr = j.id_id where j.id_sti <> 1 or j.id_sti is null group by j.id_id, j.ijr, j.id_obr");
            var jedn = BazaFB.Get_DataTable(@"select j.id_id, j.ijr, j.id_obr, sum(d.ww) Wartosc, sum(d.pew) PEWm2 from jedn_rej j join dzialka d on d.rjdr = j.id_id where j.id_sti <> 1 or j.id_sti is null group by j.id_id, j.ijr, j.id_obr order by id_obr, ijr");
            for (int i = 0; i < jedn.Rows.Count; i++)
            {
                listaJednostki_s.Add(new Jednostki_s { ID_ID = Convert.ToInt32(jedn.Rows[i][0]), IJR = Convert.ToInt32(jedn.Rows[i][1]), ID_Obr = jedn.Rows[i][2].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(jedn.Rows[i][2]), WartoscJednostki = Convert.ToDecimal(jedn.Rows[i][3]), PEW = Convert.ToDouble(jedn.Rows[i][4]) });
            }

            var NKR_PO_Z_BAZY = BazaFB.Get_DataTable("select ijr from jedn_rej_n");
            for (int i = 0; i < NKR_PO_Z_BAZY.Rows.Count; i++)
            {
                listaIjrBedaceWBazie.Add(Convert.ToInt32(NKR_PO_Z_BAZY.Rows[i][0]));
            }

            var RWD = BazaFB.Get_DataTable("select id_id, symbol from RODZ_WLADA");
            for (int i = 0; i < RWD.Rows.Count; i++)
            {
                listaRWD.Add(new RWD { ID_ID = Convert.ToInt32(RWD.Rows[i][0]), Symbol = RWD.Rows[i][1].ToString() });
            }
        }

        public static void TworzenieJednostek(int pierwszyIjr, int id_gm, int id_obr, int RWD_id, string gr = "null")
        {
            dpTworzenie.Visibility = Visibility.Visible;
            int id_idJedn = Convert.ToInt32(BazaFB.Get_DataTable("select first 1 id_id+1 from JEDN_REJ_N order by id_Id desc").Rows[0][0]);
            using (var connection = new FbConnection(BazaFB.connectionString()))
            {
                connection.Open();
                // FbCommand writeCommandJedn_rej_N = new FbCommand("insert into JEDN_REJ_N(id_id, id_gm, ijr, dtu, dtw, osou, osow, id_obr, gsp, GR, zgoda) values((select first 1  id_id + 1 from JEDN_REJ_N order by id_Id desc), @id_gm , @Ijr, (select cast('NOW' as timestamp) from rdb$database), (select cast('NOW' as timestamp) from rdb$database), 1, 1, @id_obr, 0,  @gr, 0)", connection);
                FbCommand writeCommandJedn_rej_N = new FbCommand("insert into JEDN_REJ_N(id_id, id_gm, ijr, dtu, dtw, osou, osow, id_obr, gsp, GR, zgoda) values((select gen_id(ID_JEDN_REJ_N, 1)from rdb$database), @id_gm , @Ijr, (select cast('NOW' as timestamp) from rdb$database), (select cast('NOW' as timestamp) from rdb$database), 1, 1, @id_obr, 0,  @gr, 0)", connection);
                progressBarJedn_rej.Maximum = jednTworzZeWspolnoty.Count - 1 - ListaId_PodmId_Jedn_N.Count;
                progressBarJedn_rej.Value = 0;
                if (checkBoxCzyDopisywacDoIstniejaczych.IsChecked == true)
                {
                    progressBarUzdialy.Maximum -= ListaId_PodmId_Jedn_N.Count;
                }
                //select gen_id(ID_JEDN_REJ_N, 1)from rdb$database     -wstawianie unikalnego ID
                Console.WriteLine("z czego");
                //jednTworzZeWspolnoty.ForEach(x => Console.WriteLine(x.IJR_n + " " + x.id_podm));

                //ListaId_PodmId_Jedn_N.ForEach(x =>
                //{
                //    Console.WriteLine("jednostki istniejące " + x.Id_Jedn_N + " " + x.Id_Podm);
                //});


                foreach (var item in jednTworzZeWspolnoty)
                {
                    if (ListaId_PodmId_Jedn_N.Exists(x => x.Id_Podm == item.id_podm) && checkBoxCzyDopisywacDoIstniejaczych.IsChecked == true)
                    {
                      //  Console.WriteLine("jaka jednostke pominięto: " + item.id_podm);

                        item.id_jedn_n = ListaId_PodmId_Jedn_N.Find(x => x.Id_Podm == item.id_podm).Id_Jedn_N;


                        Console.WriteLine("sprawdzam czy można dopisać!" + item.id_jedn_n + " " );
                       continue;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (listaIjrBedaceWBazie.Exists(x => x == pierwszyIjr))
                            {
                                Console.WriteLine(listaIjrBedaceWBazie.Exists(x => x == pierwszyIjr) + " " + pierwszyIjr);
                                pierwszyIjr++;
                            }
                            else
                            {
                                break;
                            }
                        }// wybranie nowego IJR który się nie pokryje z istniejącym 
                        item.IJR_n = pierwszyIjr;
                        item.id_jedn_n = id_idJedn++;
                    }



                    writeCommandJedn_rej_N.Parameters.Add("@id_gm", id_gm);
                    writeCommandJedn_rej_N.Parameters.Add("@Ijr", pierwszyIjr++);
                    writeCommandJedn_rej_N.Parameters.Add("@id_obr", id_obr);
                    writeCommandJedn_rej_N.Parameters.Add("@gr", gr);
                    writeCommandJedn_rej_N.ExecuteNonQuery();
                    writeCommandJedn_rej_N.Parameters.Clear();
                    progressBarJedn_rej.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgressJedn_rej), DispatcherPriority.Background);
                }

                // dodawanie danych do tabeli jedn_SN
                // FbCommand writeCommandJednSN = new FbCommand("insert into JEDN_SN(id_id, id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select first 1  id_id + 1 from JEDN_sn order by id_Id desc), 0, @id_gm, @ud, @ud_nr, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, @wwgsp, @powgsp)", connection);
                FbCommand writeCommandJednSN = new FbCommand("insert into JEDN_SN(id_id, id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select gen_id(ID_JEDN_SN, 1)from rdb$database), 0, @id_gm, @ud, @ud_nr, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, @wwgsp, @powgsp)", connection);
                progressBarJednSN.Maximum = ((jednTworzZeWspolnoty.Count) * listaWybranychJednstek_s.Count) - 1;
                progressBarJednSN.Value = 0;

                foreach (var jedn_n in jednTworzZeWspolnoty)
                {
                        foreach (var wybraneJednWspolnoty in listaWybranychJednstek_s)
                        {

                        writeCommandJednSN.Parameters.Add("@id_gm", id_gm);
                        writeCommandJednSN.Parameters.Add("@ud", jedn_n.ud);
                        writeCommandJednSN.Parameters.Add("@ud_nr", jedn_n.ud_nr);
                        writeCommandJednSN.Parameters.Add("@id_jednN", jedn_n.id_jedn_n);
                        writeCommandJednSN.Parameters.Add("@id_jedns", wybraneJednWspolnoty.ID_ID);

                        decimal wartUdzialu = (decimal)jedn_n.ud_nr * wybraneJednWspolnoty.WartoscJednostki;
                        double PEWudzialu = jedn_n.ud_nr * wybraneJednWspolnoty.PEW;
                        writeCommandJednSN.Parameters.Add("@wwgsp", wartUdzialu);
                        writeCommandJednSN.Parameters.Add("@powgsp", PEWudzialu);
                        writeCommandJednSN.ExecuteNonQuery();
                        writeCommandJednSN.Parameters.Clear();
                        progressBarJedn_rej.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgressJedn_SN), DispatcherPriority.Background);
                        }
                }

                //insert into UDZIALY_N(id_id, id_sti, id_gm, rwd, rwd2, ud, ud_nr, dtu, osou, id_jedn, id_podm, rodzaj, grj) values((select gen_id(ID_UDZIALY_N, 1)from rdb$database), 0, @id_gm, @rwd, @rwd, '1/1', 1, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedn, @id_podm, @rodzaj, @grj
                FbCommand writeCommandUdzialy_N = new FbCommand("insert into UDZIALY_N(id_id, id_sti, id_gm, rwd, rwd2, ud, ud_nr, dtu, osou, id_jedn, id_podm, rodzaj, grj) values((select gen_id(ID_UDZIALY_N, 1)from rdb$database), 0, @id_gm, @rwd, @rwd, '1/1', 1, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedn, @id_podm, @rodzaj, @grj)", connection);
                Object o = new object();
                o = null;

                progressBarUzdialy.Maximum = jednTworzZeWspolnoty.Count - 1;
                if(checkBoxCzyDopisywacDoIstniejaczych.IsChecked== true)
                {
                    progressBarUzdialy.Maximum -= ListaId_PodmId_Jedn_N.Count;
                }
                    progressBarUzdialy.Value = 0;
                foreach (var jedn_n in jednTworzZeWspolnoty)
                {
                    if (ListaId_PodmId_Jedn_N.Exists(x => x.Id_Podm == jedn_n.id_podm) && checkBoxCzyDopisywacDoIstniejaczych.IsChecked == true)
                    {
                        continue;
                    }

                    writeCommandUdzialy_N.Parameters.Add("@id_gm", id_gm);
                    writeCommandUdzialy_N.Parameters.Add("@rwd", RWD_id);

                    writeCommandUdzialy_N.Parameters.Add("@id_jedn", jedn_n.id_jedn_n);
                    writeCommandUdzialy_N.Parameters.Add("@id_podm", jedn_n.id_podm);
                    writeCommandUdzialy_N.Parameters.Add("@rodzaj", 1);
                    writeCommandUdzialy_N.Parameters.Add("@grj", gr);

                    writeCommandUdzialy_N.ExecuteNonQuery();
                    writeCommandUdzialy_N.Parameters.Clear();
                    progressBarJedn_rej.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgressUdzialy), DispatcherPriority.Background);
                }

                connection.Close();

               if(jednTworzZeWspolnoty.Count <= 0)
                {
                    MessageBox.Show("Brak wybranych jednostek.");
                }
                else
                {
                    MessageBox.Show("UTWORZONO JEDNOSTKI!");
                }

                progressBarJednSN.Value = 0;
                progressBarUzdialy.Value = 0;
                progressBarJedn_rej.Value = 0;
                dpTworzenie.Visibility = Visibility.Hidden;
            }

        }

        public static void SprawdzCzyMoznaDopisacDoIsniejacej()
        {
            foreach (var item in jednTworzZeWspolnoty)
            {
                Console.WriteLine(item);



            }
        }

    }
}