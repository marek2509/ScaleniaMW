using FirebirdSql.Data.FirebirdClient;
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
        public static List<int> listaIjrBedaceWBazie = new List<int>();
        public static List<Gmina> listGminy = new List<Gmina>();
        public static List<Obreb> listObreby = new List<Obreb>();
        public static List<Jednostki_N> listaJednostki_N = new List<Jednostki_N>();
        public static List<Jednostki_N> listaWybranychJednstek_N = new List<Jednostki_N>();
        public static List<ModelJednostkiTworzonejZeWspolnowy> jednTworzZeWspolnoty = new List<ModelJednostkiTworzonejZeWspolnowy>();

        //wstaw w tabele jedn_sn
        //insert into JEDN_SN(id_id,id_sti, id_gm, ud, ud_nr, dtu, osou, id_jedns, id_jednn, wwgsp, powwgsp) values((select first 1  id_id + 1 from JEDN_sn order by id_Id desc),0, @id_gm , @ud, @ud_nr(select cast('NOW' as timestamp) from rdb$database), 1, @id_jedns, @id_jednN, 0, 0)


        public static int ileJednostekTrzebaUtworzyc()
        { //select id_podm, ud , ud_nr from udzialy_n u      join jedn_rej_n j on j.id_id = u.id_jedn  where  (j.id_obr = 1 and j.ijr = 11000) or(j.ijr = 20007 and j.id_obr = 2) or(j.ijr = 30002 and  j.id_obr = 3)   group by id_podm, ud , ud_nr


            string s = "select id_podm, ud , ud_nr from udzialy u      join jedn_rej j on j.id_id = u.id_jedn  where ";


            for (int i = 0; i < listaWybranychJednstek_N.Count; i++)
            {
                if (i == 0)
                {
                    s += " (j.id_obr = " + listaWybranychJednstek_N[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_N[i].IJR + ") ";
                }
                else
                {
                    s += "or (j.id_obr = " + listaWybranychJednstek_N[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_N[i].IJR + ") ";
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


            for (int i = 0; i < listaWybranychJednstek_N.Count; i++)
            {
                if (i == 0)
                {
                    s += " (j.id_obr = " + listaWybranychJednstek_N[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_N[i].IJR + ") ";
                }
                else
                {
                    s += "or (j.id_obr = " + listaWybranychJednstek_N[i].ID_Obr + " and j.ijr = " + listaWybranychJednstek_N[i].IJR + ") ";
                }
            }

            //   (j.id_obr = 1 and j.ijr = 11000) or(j.ijr = 20007 and j.id_obr = 2) or(j.ijr = 30002 and  j.id_obr = 3)
            s += " group by id_podm, ud having  count(id_podm) <>" + listaWybranychJednstek_N.Count;

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
            listaWybranychJednstek_N.Clear();
            listaJednostki_N.Clear();
            var jedn = BazaFB.Get_DataTable("select id_id, ijr, id_obr from jedn_rej where id_sti <> 1 or id_sti is null");

            for (int i = 0; i < jedn.Rows.Count; i++)
            {
                listaIjrBedaceWBazie.Add(Convert.ToInt32(jedn.Rows[i][1]));
                listaJednostki_N.Add(new Jednostki_N { ID_ID = Convert.ToInt32(jedn.Rows[i][0]), IJR = Convert.ToInt32(jedn.Rows[i][1]), ID_Obr = jedn.Rows[i][2].Equals(DBNull.Value) ? (int?)null : Convert.ToInt32(jedn.Rows[i][2]) });
            }
        }


        public static void TworzenieJednostek(int pierwszyIjr, int id_gm, int id_obr, string gr = "null")
        {
            int id_idJedn = Convert.ToInt32(BazaFB.Get_DataTable("select first 1 id_id+1 from JEDN_REJ_N order by id_Id desc").Rows[0][0]);
            Console.WriteLine();

            gr = gr == "null" ? "null" : "'" + gr + "'";
            using (var connection = new FbConnection(BazaFB.connectionString()))
            {
                connection.Open();
                FbCommand writeCommand = new FbCommand("insert into JEDN_REJ_N(id_id, id_gm, ijr, dtu, dtw, osou, osow, id_obr, gsp, GR, zgoda) values((select first 1  id_id + 1 from JEDN_REJ_N order by id_Id desc), @id_gm , @Ijr, (select cast('NOW' as timestamp) from rdb$database), (select cast('NOW' as timestamp) from rdb$database), 1, 1, @id_obr, 0,  @gr, 0)", connection);
                foreach (var item in jednTworzZeWspolnoty)
                {
                    for (; ; )
                    {
                        if (listaIjrBedaceWBazie.Exists(x => x == pierwszyIjr))
                        {
                            pierwszyIjr++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    item.IJR_n = pierwszyIjr;
                    item.id_jedn_n = id_idJedn++;

                    writeCommand.Parameters.Add("@id_gm", id_gm);
                    writeCommand.Parameters.Add("@Ijr", pierwszyIjr++);
                    writeCommand.Parameters.Add("@id_obr", id_obr);
                    writeCommand.Parameters.Add("@gr", gr);

                    writeCommand.ExecuteNonQuery();


                    writeCommand.Parameters.Clear();

                    Console.WriteLine("nkr, id_id:" + item.IJR_n + ", " + item.id_jedn_n);

                }
                connection.Close();
                MessageBox.Show("UTWORZONO JEDNOSTKI!");
            }

        }

        public static void pobierzWartDlaJednostekPrzedScaleniem()
        {
       
        }

        public class Jednostki_N
        {
            public int ID_ID { get; set; }
            public int IJR { get; set; }
            public int? ID_Obr;

            public string NazwaObrebu
            {

                get => Wspolnota.listObreby.Exists(x => x.IdObrebu == (ID_Obr.Equals(DBNull.Value) ? -1 : ID_Obr)) ? Wspolnota.listObreby.Find(x => x.IdObrebu == (ID_Obr.Equals(DBNull.Value) ? -1 : ID_Obr)).Nazwa : "BRAK_OBREBU";

                private set
                {
                }
            }
            public int NrObr
            {
                get => Wspolnota.listObreby.Exists(x => x.IdObrebu == ID_Obr) ? Wspolnota.listObreby.Find(x => x.IdObrebu == ID_Obr).NrObrebu : 0;

                private set
                {
                }
            }
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
    }
}