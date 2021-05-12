﻿using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        }

        public static List<int> listaIjrBedaceWBazie = new List<int>();
        public static List<Gmina> listGminy = new List<Gmina>();
        public static List<Obreb> listObreby = new List<Obreb>();
        public static List<Jednostki_s> listaJednostki_s = new List<Jednostki_s>();
        public static List<Jednostki_s> listaWybranychJednstek_s = new List<Jednostki_s>();
        public static List<ModelJednostkiTworzonejZeWspolnowy> jednTworzZeWspolnoty = new List<ModelJednostkiTworzonejZeWspolnowy>();

        //wstaw w tabele jedn_sn
        //insert into JEDN_SN(id_id,id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select first 1  id_id + 1 from JEDN_sn order by id_Id desc),0, @id_gm , @ud, @ud_nr(select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, 0, 0)

        public static int ileJednostekTrzebaUtworzyc()
        { //select id_podm, ud , ud_nr from udzialy_n u      join jedn_rej_n j on j.id_id = u.id_jedn  where  (j.id_obr = 1 and j.ijr = 11000) or(j.ijr = 20007 and j.id_obr = 2) or(j.ijr = 30002 and  j.id_obr = 3)   group by id_podm, ud , ud_nr
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
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jednTworzZeWspolnoty.Add(new ModelJednostkiTworzonejZeWspolnowy { id_podm = Convert.ToInt32(dt.Rows[i][0]), ud = dt.Rows[i][1].ToString(), ud_nr = Convert.ToDouble(dt.Rows[i][2]) });
            }
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

        public static void pobierzJednostki_n()
        {
            listaIjrBedaceWBazie.Clear();
            listaWybranychJednstek_s.Clear();
            listaJednostki_s.Clear();
           // var jedn = BazaFB.Get_DataTable("select id_id, ijr, id_obr from jedn_rej where id_sti <> 1 or id_sti is null");
          //  var jedn = BazaFB.Get_DataTable(@"select j.id_id, j.ijr, j.id_obr, sum(d.ww) Wartosc, sum(d.pew) PEWm2 from jedn_rej j join dzialka d on d.rjdr = j.id_id where j.id_sti <> 1 or j.id_sti is null group by j.id_id, j.ijr, j.id_obr");
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

        }

        public static void TworzenieJednostek(int pierwszyIjr, int id_gm, int id_obr, string gr = "null")
        {
            int id_idJedn = Convert.ToInt32(BazaFB.Get_DataTable("select first 1 id_id+1 from JEDN_REJ_N order by id_Id desc").Rows[0][0]);
            Console.WriteLine();

            gr = gr == "null" ? "null" :  gr;
            using (var connection = new FbConnection(BazaFB.connectionString()))
            {
                connection.Open();
               // FbCommand writeCommandJedn_rej_N = new FbCommand("insert into JEDN_REJ_N(id_id, id_gm, ijr, dtu, dtw, osou, osow, id_obr, gsp, GR, zgoda) values((select first 1  id_id + 1 from JEDN_REJ_N order by id_Id desc), @id_gm , @Ijr, (select cast('NOW' as timestamp) from rdb$database), (select cast('NOW' as timestamp) from rdb$database), 1, 1, @id_obr, 0,  @gr, 0)", connection);
                FbCommand writeCommandJedn_rej_N = new FbCommand("insert into JEDN_REJ_N(id_id, id_gm, ijr, dtu, dtw, osou, osow, id_obr, gsp, GR, zgoda) values((select gen_id(ID_JEDN_REJ_N, 1)from rdb$database), @id_gm , @Ijr, (select cast('NOW' as timestamp) from rdb$database), (select cast('NOW' as timestamp) from rdb$database), 1, 1, @id_obr, 0,  @gr, 0)", connection);


                //select gen_id(ID_JEDN_REJ_N, 1)from rdb$database     -wstawianie unikalnego ID
                foreach (var item in jednTworzZeWspolnoty)
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
                    Console.WriteLine(gr);
                    writeCommandJedn_rej_N.Parameters.Add("@id_gm", id_gm);
                    writeCommandJedn_rej_N.Parameters.Add("@Ijr", pierwszyIjr++);
                    writeCommandJedn_rej_N.Parameters.Add("@id_obr", id_obr);
                    writeCommandJedn_rej_N.Parameters.Add("@gr", gr);
                    writeCommandJedn_rej_N.ExecuteNonQuery();
                    writeCommandJedn_rej_N.Parameters.Clear();
                }

                // dodawanie danych do tabeli jedn_SN
                // FbCommand writeCommandJednSN = new FbCommand("insert into JEDN_SN(id_id, id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select first 1  id_id + 1 from JEDN_sn order by id_Id desc), 0, @id_gm, @ud, @ud_nr, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, @wwgsp, @powgsp)", connection);
                FbCommand writeCommandJednSN = new FbCommand("insert into JEDN_SN(id_id, id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select gen_id(ID_JEDN_SN, 1)from rdb$database), 0, @id_gm, @ud, @ud_nr, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, @wwgsp, @powgsp)", connection);
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
                    }
                }

                //insert into UDZIALY_N(id_id, id_sti, id_gm, rwd, rwd2, ud, ud_nr, dtu, osou, id_jedn, id_podm, rodzaj, grj) values((select gen_id(ID_UDZIALY_N, 1)from rdb$database), 0, @id_gm, @rwd, @rwd, '1/1', 1, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedn, @id_podm, @rodzaj, @grj
                FbCommand writeCommandUdzialy_N = new FbCommand("insert into UDZIALY_N(id_id, id_sti, id_gm, rwd, rwd2, ud, ud_nr, dtu, osou, id_jedn, id_podm, rodzaj, grj) values((select gen_id(ID_UDZIALY_N, 1)from rdb$database), 0, @id_gm, @rwd, @rwd, '1/1', 1, (select cast('NOW' as timestamp) from rdb$database), 1, @id_jedn, @id_podm, @rodzaj, @grj)", connection);

                foreach (var jedn_n in jednTworzZeWspolnoty)
                {
                    writeCommandUdzialy_N.Parameters.Add("@id_gm", id_gm);
                    writeCommandUdzialy_N.Parameters.Add("@rwd", 11);
                    writeCommandUdzialy_N.Parameters.Add("@id_jedn", jedn_n.id_jedn_n);
                    writeCommandUdzialy_N.Parameters.Add("@id_podm", jedn_n.id_podm);
                    writeCommandUdzialy_N.Parameters.Add("@rodzaj", 1);
                    writeCommandUdzialy_N.Parameters.Add("@grj", gr);
                    
                    writeCommandUdzialy_N.ExecuteNonQuery();
                    writeCommandUdzialy_N.Parameters.Clear(); 
                }




                connection.Close();
                MessageBox.Show("UTWORZONO JEDNOSTKI!");
            }

        }



    }
}