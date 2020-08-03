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

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy WindowPorownajPrzedPo.xaml
    /// </summary>
    public partial class WindowPorownajPrzedPo : Window
    {
        public WindowPorownajPrzedPo()
        {

            InitializeComponent();
            try
            {
                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
            }
            catch
            {

            }

        }

        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowPorownajPrzedPo.Close();

        }

        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void ustawProperties(string FileName)
        {
            Properties.Settings.Default.PathFDB = FileName;
            textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
            Properties.Settings.Default.Save();
        }

        private void ustawSciezkeFDB(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (!(Properties.Settings.Default.PathFDB.Equals("") || Properties.Settings.Default.PathFDB.Equals(null)))
            {
                dlg.InitialDirectory = Properties.Settings.Default.PathFDB.ToString().Substring(0, Properties.Settings.Default.PathFDB.LastIndexOf("\\"));
            }

            //dlg.InitialDirectory = @"C:\";
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";




            Nullable<bool> resulDialog = dlg.ShowDialog();
            if (resulDialog == true)
            {
                try
                {
                    ustawProperties(dlg.FileName);

                    itemPolaczZBaza.Background = Brushes.Transparent;
                    itemPolaczZBaza.Header = "Połącz z bazą";
                    stanPrzedWartoscis = new List<StanPrzedWartosci>();
                    resultPorownanie = new List<ZsumwaneWartosciZPorownania>();
                    dgPorownanie.ItemsSource = resultPorownanie;
                }
                catch (Exception esa)
                {
                    var resultat = MessageBox.Show(esa.ToString() + " Przerwać?", "ERROR", MessageBoxButton.YesNo);

                    if (resultat == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }

                    Console.WriteLine(esa + "Błędny format importu działek");
                }
            }
        }

        void przejdzDoUstawLoginIHaslo()
        {
            textBoxLogin.Text = Properties.Settings.Default.Login;
            textBoxHaslo.Password = Properties.Settings.Default.Haslo;
            panelLogowania.Visibility = Visibility.Visible;
            dgPorownanie.Visibility = Visibility.Hidden;
        }


        private void PolaczZBaza(object sender, RoutedEventArgs e)
        {
            try
            {
                odczytajZSql();
                itemPolaczZBaza.Background = Brushes.SeaGreen;
                StringBuilder stringBuilder = new StringBuilder();
                textBlockLogInfo.Text = "Połączono z bazą FDB. Ilość wczytanych linii: " + dt.Rows.Count + ".";
                itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);


            }
            catch (Exception ex)
            {

                itemPolaczZBaza.Background = Brushes.Red;
                itemPolaczZBaza.Header = "Połącz z bazą";
                textBlockLogInfo.Text = "Problem z połączeniem z bazą FDB " + ex.Message;
                przejdzDoOknaLogowania(ex.Message);
            }
        }

        void przejdzDoOknaLogowania(string s)
        {
            if (s.Contains("password"))
            {
                przejdzDoUstawLoginIHaslo();
            }
        }
        private void UstawLoginIHaslo(object sender, RoutedEventArgs e)
        {
            przejdzDoUstawLoginIHaslo();
        }

        private void ButtonZapiszLogIHaslo(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Login = textBoxLogin.Text;
            Properties.Settings.Default.Haslo = textBoxHaslo.Password;
            Properties.Settings.Default.Save();
            panelLogowania.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            dgPorownanie.Visibility = Visibility.Visible;
        }

        private void Button_Anuluj(object sender, RoutedEventArgs e)
        {
            panelLogowania.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            dgPorownanie.Visibility = Visibility.Visible;
        }

        public static void aktualizujSciezkeZPropertis()
        {
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=3050;Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        DataTable dt;

        public static string connectionString;

        List<StanPrzedWartosci> stanPrzedWartoscis;
        List<ZsumwaneWartosciZPorownania> resultPorownanie;

        public void odczytajZSql()
        {
            stanPrzedWartoscis = new List<StanPrzedWartosci>();
            //  listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
            aktualizujSciezkeZPropertis();
            Console.WriteLine(connectionString);
            using (var connection = new FbConnection(connectionString))
            {
                connection.Open();

                FbCommand command = new FbCommand();

                FbTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;
                // działające zapytanie na nrobr-nrdz NKR 
                //  command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case WHEN JEDN_REJ.nkr is null then obreby.id * 1000 + JEDN_REJ.grp else JEDN_REJ.nkr end as NKR_Z_GRUPAMI from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ on dzialka.rjdr = JEDN_REJ.id_id order by NKR_Z_GRUPAMI";
                command.CommandText = "select  j.ijr NKR__PO__SCAL, sum(d.ww) WART__PO__SCAL from DZIALKI_N d join JEDN_REJ_N j on j.ID_ID = d.rjdr group by ijr";


                //  

                FbDataAdapter adapter = new FbDataAdapter(command);
                dt = new DataTable();

                adapter.Fill(dt);
                //foreach (var item in dt.Columns)
                //{

                //    Console.Write(item + " << ");
                //}
                Console.WriteLine("row count:" + dt.Rows.Count);
                Console.WriteLine("column count:" + dt.Columns.Count);

                List<ZsumwaneWartosciZPorownania> zsumwaneWartosciStanPO = new List<ZsumwaneWartosciZPorownania>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    zsumwaneWartosciStanPO.Add(new ZsumwaneWartosciZPorownania(Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]),
                        0,
                        Convert.ToDecimal(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1].ToString().Replace(".", ","))));
                }

                // pobranie tabeli idStare i NKR NOWY

                // command.CommandText = "select id_jedns, jr.ijr from JEDN_SN join jedn_rej_N jr on jr.id_id = jedn_sn.id_jednn order by jr.ijr";
                command.CommandText = "select ID_ID, ijr from jedn_rej_N order by ID_ID";

                adapter = new FbDataAdapter(command);
                dt = new DataTable();

                adapter.Fill(dt);
                List<IdJrStNKRJeNowej> idJrNowejNKRJeNowej = new List<IdJrStNKRJeNowej>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    idJrNowejNKRJeNowej.Add(new IdJrStNKRJeNowej(Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]),
                        Convert.ToInt32(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1])));
                }



                // stan przed 


                // command.CommandText = "select replace(d.ww,'.',','), j.ijr, j.nkr, j.grp, j.idgrp, j.ID_ID from dzialka d join jedn_rej j on j.ID_ID = d.rjdr order by idgrp";

                // dodano do tabeli id jednN
                command.CommandText = "select replace(d.ww,'.',','), j.ijr, j.nkr, j.grp, j.idgrp, jsn.id_jednn from dzialka d join jedn_rej j on j.ID_ID = d.rjdr join jedn_sn jsn on jsn.id_jedns=j.id_id order by idgrp";
                /*
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    stanPrzedWartoscis.Add(new StanPrzedWartosci((double)dt.Rows[i][0], (int)dt.Rows[i][1], (int)dt.Rows[i][2], (int)dt.Rows[i][3], (int)dt.Rows[i][4]));
                                }*/

                adapter = new FbDataAdapter(command);
                dt = new DataTable();

                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    stanPrzedWartoscis.Add(new StanPrzedWartosci(Convert.ToDecimal(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]),
                        Convert.ToInt32(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1]),
                        Convert.ToInt32(dt.Rows[i][2].Equals(System.DBNull.Value) ? null : dt.Rows[i][2]),
                        Convert.ToInt32(dt.Rows[i][3].Equals(System.DBNull.Value) ? null : dt.Rows[i][3]),
                        Convert.ToInt32(dt.Rows[i][4].Equals(System.DBNull.Value) ? null : dt.Rows[i][4]),
                        Convert.ToInt32(dt.Rows[i][5].Equals(System.DBNull.Value) ? null : dt.Rows[i][5])));
                }


                //  stanPrzedWartoscis.ForEach(x => Console.WriteLine(x.NKR));

                //foreach (var item in dt.Columns)
                //{

                //    Console.Write(item + " << ");
                //}
                Console.WriteLine("row count:" + dt.Rows.Count);
                Console.WriteLine("column count:" + dt.Columns.Count);
                /*
                while (stanPrzedWartoscis.Exists(x => x.NKR == 0))
                {
                    StanPrzedWartosci stanPrzed = new StanPrzedWartosci();
                    stanPrzed = stanPrzedWartoscis.Find(x => x.NKR == 0);
                    //  stanPrzed.wypiszwConsoli();
                    StanPrzedWartosci stanPrzedZnalezionyZNKREM = new StanPrzedWartosci();
                    stanPrzedZnalezionyZNKREM = stanPrzedWartoscis.Find(x => x.IDGRP == stanPrzed.IDGRP && x.NKR != 0);
                    //stanPrzedZnalezionyZNKREM.wypiszwConsoli();
                    stanPrzed.NKR = stanPrzedZnalezionyZNKREM.NKR;
                    //  stanPrzedWartoscis.Find(x => x.Equals(stanPrzed)).wypiszwConsoli();
                    // break;
                }
                */
                resultPorownanie = stanPrzedWartoscis.GroupBy(l => l.id_id).Select(cl =>
             new ZsumwaneWartosciZPorownania
             {
                 NKR = (int)cl.First().NKR,
                 IdPo = (int)cl.First().id_id,
                 WartPrzed = cl.Sum(c => c.Wartosc)
             }).ToList();

                Console.WriteLine("punkt kontrolny 2");

                //foreach (var item in zsumwaneWartosciStanPO)
                //{

                //    if (resultPorownanie.Exists(x => x.IdPo == idJrNowejNKRJeNowej.Find(xa => xa.NKRJednNowej == item.NKR).idJednNowej))
                //    {

                //       item.WartPrzed += resultPorownanie.Find(x => x.IdPo == idJrNowejNKRJeNowej.Find(xa => xa.NKRJednNowej == item.NKR).idJednNowej).WartPrzed;

                //        item.IdPo = resultPorownanie.Find(x => x.NKR == item.NKR).IdPo;
                //    }

                //}

                foreach (var item in resultPorownanie)
                {


                    if (zsumwaneWartosciStanPO.Exists(x => x.NKR == idJrNowejNKRJeNowej.Find(xa => xa.idJednNowej == item.IdPo).NKRJednNowej))
                    {

                        zsumwaneWartosciStanPO.Find((x => x.NKR == idJrNowejNKRJeNowej.Find(xa => xa.idJednNowej == item.IdPo).NKRJednNowej)).WartPrzed = item.WartPrzed;
                    }
                    else
                    {
                        zsumwaneWartosciStanPO.Add(item);
                    }



                }



                //opcja do przerobienia  błędnie przipisuje
                /*
                foreach (var item in resultPorownanie)
                {

                    if (idJrStNKRJeNowej.Exists(x => x.idJednStarej == item.IdPrzed))
                    {
                        IdJrStNKRJeNowej tmpIdJednSTNO = idJrStNKRJeNowej.Find(x => x.idJednStarej == item.IdPrzed);



                        if (zsumwaneWartosciStanPO.Exists(x => x.NKR == tmpIdJednSTNO.NKRJednNowej))
                        {
                            //ZsumwaneWartosciZPorownania tmpZsumowaneWartPrzed = new ZsumwaneWartosciZPorownania();
                            //tmpZsumowaneWartPrzed = item.Exists(x => x.idJednStarej == item.IdPrzed)
                            item.WartPo = zsumwaneWartosciStanPO.Find(x => x.NKR == tmpIdJednSTNO.NKRJednNowej).WartPo;
                        }
                    }
                }


                foreach (var item in zsumwaneWartosciStanPO)
                {
                    if (!resultPorownanie.Exists(x => x.NKR == item.NKR))
                    {
                        Console.WriteLine("BRAK NKR: result:  " + item.NKR);

                        for (int i = 0; i < resultPorownanie.Count; i++)
                        {
                           if(resultPorownanie[i].NKR > item.NKR)
                            {
                                Console.WriteLine("result[i].NKR > item.NKR :" + resultPorownanie[i].NKR + ">" + item.NKR);
                                resultPorownanie.Insert(i, item);
                                break;
                            }
                        }
                    }
                }

            */
                /*
                    resultPorownanie.Sort(delegate (ZsumwaneWartosciZPorownania x, ZsumwaneWartosciZPorownania y)
                    {
                        if (x.NKR == 0 && y.NKR == 0) return 0;
                        else if (x.NKR == 0) return -1;
                        else if (y.NKR == 0) return 1;
                        else return x.NKR.CompareTo(y.NKR);
                    });

        */
                //foreach (var item in result)
                //{
                //    Console.WriteLine(item.NKR + " " + item.WartPrzed + " " + item.WartPo);
                //}


                try
                {
                    //dataGrid.ItemsSource = dt.AsDataView();
                    //dataGrid.Visibility = Visibility.Visible;
                    //dataGrid.Items.Refresh();
                    dgPorownanie.ItemsSource = zsumwaneWartosciStanPO;
                    dgPorownanie.Visibility = Visibility.Visible;
                    dgPorownanie.Items.Refresh();

                    Console.WriteLine("ustawiam SOURCE");
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp);
                }

                //Console.WriteLine(i+ " " + dt.Rows[i][0]); // "" + dt.Rows[i][1] + " " );


                connection.Close();
            }

        }

    }
}
