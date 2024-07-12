using EWOPISMW.Infrstruktura;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScaleniaMW.EWOPIS.Widoki
{
    /// <summary>
    /// Logika interakcji dla klasy WindowDzialkiDoEwopis.xaml
    /// </summary>
    public partial class WindowDzialkiDoEwopis : Window
    {
        public WindowDzialkiDoEwopis()
        {
            InitializeComponent();
            Infrstruktura.Plik.windowDzialkiDoEwopis = windowDzialkiDoEwopis;
            Infrstruktura.Plik.OdswierzScierzkeWPasku();

            if (Properties.Settings.Default.EwopisPort3050 == true)
            {
                radioPort3050.IsChecked = true;
            }
            else
            {
                radioPort3051.IsChecked = true;
            }
        }


        private void MenuItem_ClickUstawSciezke(object sender, RoutedEventArgs e)
        {
            Infrstruktura.Plik.PobierzSciezke();
        }
        List<TabelaKolumna> listaTabeleKolumny = new List<TabelaKolumna>();
        List<string> listaTabele = new List<string>();

        private void MenuItem_ClickPolaczZBaza(object sender, RoutedEventArgs e)
        {
            string pytanieSql = "select f.rdb$relation_name, f.rdb$field_name from rdb$relation_fields f join rdb$relations r on f.rdb$relation_name = r.rdb$relation_name and r.rdb$view_blr is null and(r.rdb$system_flag is null or r.rdb$system_flag = 0) order by 1, f.rdb$field_position";

            if (Infrstruktura.BazaFB.Get_DataTable(pytanieSql) == (null))
            {
                polaczMenu.Background = Brushes.Transparent;
            }
            else
            {

                DataTable dt = new DataTable();
                dt = Infrstruktura.BazaFB.Get_DataTable(pytanieSql);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listaTabeleKolumny.Add(new TabelaKolumna { Tabela = dt.Rows[i][0].ToString(), Kolumna = dt.Rows[i][1].ToString() });
                }

                listaTabele = listaTabeleKolumny.Select(x => x.Tabela).ToList().Distinct().ToList();

                polaczMenu.Background = Brushes.GreenYellow;
            }
        }

        private void RadioPort3050_Checked(object sender, RoutedEventArgs e)
        {
            radioPort3050.IsChecked = true;
            Properties.Settings.Default.EwopisPort3050 = true;
            Properties.Settings.Default.Save();
        }

        private void RadioPort3051_Checked(object sender, RoutedEventArgs e)
        {
            radioPort3051.IsChecked = true;
            Properties.Settings.Default.EwopisPort3050 = false;
            Properties.Settings.Default.Save();
        }

        public class TabelaKolumna
        {
            public string Tabela { get; set; }
            public string Kolumna { get; set; }
        }


        List<Modele.ModelDzialkiParametry> modelDzialkiParametry = new List<Modele.ModelDzialkiParametry>();
        private void Button_ClickOdczytajParametry(object sender, RoutedEventArgs e)
        {
            modelDzialkiParametry = Infrstruktura.Plik.odczytajParametry();

            dgParametry.ItemsSource = modelDzialkiParametry;
        }



        private void Button_Click_base_load(object sender, RoutedEventArgs e)
        {
            List<Modele.ModelObrebu> obrebyZBazy = new List<Modele.ModelObrebu>();
            DataTable dt = new DataTable();
            dt = Infrstruktura.BazaFB.Get_DataTable("select id, teryt from  obreby");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                obrebyZBazy.Add(new Modele.ModelObrebu(Convert.ToInt32(dt.Rows[i][0]), dt.Rows[i][1].ToString()));
            }

            using (var connection = new FbConnection(Infrstruktura.BazaFB.PobierzConnectionString()))
            {
                List<int> jakichObrebowNieMaWBazie = new List<int>();
                connection.Open();

                //UPDATE DZIALKI_N SET KW = CASE Id_Id  WHEN 660 THEN 'BI100000' else KW END where Id_Id IN(660)
                // FbCommand writeCommand = new FbCommand("INSERT INTO DZIALKA(rjdr, uid, id, idd, idobr, sidd, teryt, CTRL, status, osou, wrt, m2, dtu) VALUES((select id from jedn_rej jr where jr.ijr =@NrjednRej and ID_OBR =@nrObr), (select gen_id(UID_DZIALKA_G, 1)from rdb$database), (select gen_id(DZIALKI_G, 1)from rdb$database), @NrDz , @nrObr, '      .   ' || @NrDz || '/      ;      ', @TERYT, @NrDz, 0, 1, 0, 1, (select cast('NOW' as timestamp) from rdb$database ))", connection);
                FbCommand writeCommand = new FbCommand("INSERT INTO DZIALKA(rjdr,uid,id, idd, idobr, sidd, teryt, CTRL, STATUS, dtu, wrt, m2, DOWU, ZMA) VALUES((select first 1 id from jedn_rej jr where jr.ijr= @rjdr and ID_OBR= @idobr and (sti =0 or sti = 2)), (select gen_id(UID_DZIALKA_G, 1)from rdb$database), (select gen_id(DZIALKI_G, 1)from rdb$database), @idd, @idobr, @sidd, @teryt, @CTRL, @STATUS, (select cast('NOW' as timestamp) from rdb$database), @wrt, @m2, @DOWUZMA, @DOWUZMA)", connection);
                writeCommand.CommandType = CommandType.Text;
                int ileDzialekWczytano = 0;
                try
                {
                    foreach (var item in modelDzialkiParametry)
                    {
                        writeCommand.Parameters.Clear();
                        int nrJednRej = item.NrJednRej;
                        int nrObrebu = item.IdObr;
                        string nrDz = item.Nrdz;
                        string teryt = null;

                        if (obrebyZBazy.Exists(obreb => obreb.Idobr == nrObrebu))
                        {
                            teryt = obrebyZBazy.Find(obreb => obreb.Idobr == nrObrebu).Teryt;
                        }
                        else
                        {
                            if (!jakichObrebowNieMaWBazie.Exists(x => x == nrJednRej))
                            {
                                jakichObrebowNieMaWBazie.Add(nrObrebu);
                                MessageBox.Show("Brak w bazie obrebu nr " + nrObrebu.ToString());
                            }
                        }

                        writeCommand.Parameters.Add("@rjdr", nrJednRej);
                        writeCommand.Parameters.Add("@idd", nrDz);
                        writeCommand.Parameters.Add("@idobr", nrObrebu);
                        writeCommand.Parameters.Add("@sidd", ModyfikacjaSIDD.GenerujSIDD(nrDz));

                        writeCommand.Parameters.Add("@teryt", teryt);
                        writeCommand.Parameters.Add("@CTRL", nrDz);

                        int wartZero = 0;
                        writeCommand.Parameters.Add("@wrt", wartZero);
                        writeCommand.Parameters.Add("@m2", 1);
                        int? idZmiany = Modele.ModelZmiana.pobierzIdDo(listboxZmiany.SelectedValue == null ? "" : listboxZmiany.SelectedValue.ToString());

                        int StatusZero = 0;
                        if (checkBoxZmiana.IsChecked == true)
                        {
                            StatusZero = 2;
                        }

                        writeCommand.Parameters.Add("@DOWUZMA", idZmiany);
                        writeCommand.Parameters.Add("@STATUS", StatusZero);
                        //int? idZmiany = Modele.ModelZmiana.pobierzIdDo(listboxZmiany.SelectedValue == null ? null : listboxZmiany.SelectedValue.ToString());

                        writeCommand.ExecuteNonQuery();
                        ileDzialekWczytano++;
                    }


                    connection.Close();
                    MessageBox.Show("WŁALA. Załadowano działek: " + ileDzialekWczytano);
                }
                catch (Exception s)
                {
                    MessageBox.Show(s.Message);
                }
            }
        }

        List<Modele.ModelZmiana> listaModelZmian = new List<Modele.ModelZmiana>();
        private void ButtonPobierzZmiany_Click(object sender, RoutedEventArgs e)
        {
            listaModelZmian.Clear();
            try
            {
                DataTable dt = Infrstruktura.BazaFB.Get_DataTable("select id, lp || '/' || rok || ' ' || sygn  from ZMIANY where zam  is null or zam = ''");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listaModelZmian.Add(new Modele.ModelZmiana() { idZmiany = Convert.ToInt32(dt.Rows[i][0]), NazwaZmiany = dt.Rows[i][1].ToString() });
                }
                listboxZmiany.ItemsSource = listaModelZmian.Select(x => x.PobierzDoListy());
                if (listaModelZmian.Select(x => x.PobierzDoListy()).ToList().Count == 0)
                {
                    MessageBox.Show("Brak niezatwierdzonych zmian w programie EWOPIS.", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    checkBoxZmiana.IsChecked = false;
                }
                Infrstruktura.BazaFB.Close_DB_Connection();
            }
            catch (Exception exs)
            {
                Console.WriteLine("zmiany: " + exs.Message);
            }
        }

        private void Button_Click_pokaz_format_importu(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Kolumna: 1[obręb], 2[działka], 3[jedn. rej.]\n jak w przykładzie poniżej: \n\n\t4\t1001\t1\n\t4\t1002\t2\n\t4\t1003\t3\n\t4\t1004\t4\n", "FORMAT IMPORTU", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CheckBoxZmiana_Checked(object sender, RoutedEventArgs e)
        {
            ButtonPobierzZmiany_Click(sender, e);
        }

        private void MenuItem_ClicUstawLoginIHaslok(object sender, RoutedEventArgs e)
        {
            Infrstruktura.BazaFB.przejdzDoUstawLoginIHaslo();
        }
    }
}
