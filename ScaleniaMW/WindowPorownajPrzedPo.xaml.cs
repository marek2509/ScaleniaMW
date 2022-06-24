using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ScaleniaMW
{
    public partial class WindowPorownajPrzedPo : Window
    {
        string[] arrayListEdit = {
            nameof(ZsumwaneWartosciZPorownania.PotraceniaPrzed),
            nameof(ZsumwaneWartosciZPorownania.ZerujDoplaty),
            nameof(ZsumwaneWartosciZPorownania.NieDoliczajDoplatyZaDrogi),
            nameof(ZsumwaneWartosciZPorownania.OdchWProgramie),
            nameof(ZsumwaneWartosciZPorownania.ZgodawProgramie) };
        public WindowPorownajPrzedPo()
        {
            InitializeComponent();
            comboBoxEditColumn.ItemsSource = arrayListEdit;
            comboBoxEditColumn.Items.Refresh();


            try
            {
                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

        bool czyPolaczonoZBaza = false;
        private void PolaczZBaza(object sender, RoutedEventArgs e)
        {
            odczytajZSql();
            itemPolaczZBaza.Background = Brushes.SeaGreen;
            StringBuilder stringBuilder = new StringBuilder();
            textBlockLogInfo.Text = "Połączono z bazą FDB. Ilość wczytanych linii: " + dt.Rows.Count + ".";
            itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
            czyPolaczonoZBaza = true;
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
            dgPorownanie.Visibility = Visibility.Visible;
        }

        private void Button_Anuluj(object sender, RoutedEventArgs e)
        {
            panelLogowania.Visibility = Visibility.Hidden;
            dgPorownanie.Visibility = Visibility.Visible;
        }

        public static void aktualizujConnectionStringZPropertis()
        {
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        DataTable dt;
        public static string connectionString;

        List<StanPrzedWartosci> stanPrzedWartoscis;
        List<ZsumwaneWartosciZPorownania> resultPorownanie;
        List<ZsumwaneWartosciZPorownania> zsumwaneWartosciStanPO;



        public void odczytajZSql()
        {
            stanPrzedWartoscis = new List<StanPrzedWartosci>();
            //  listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
            aktualizujConnectionStringZPropertis();
            Console.WriteLine(connectionString);
            using (var connection = new FbConnection(connectionString))
            {
                //connection.OpenAsync();
                connection.Open();

                FbCommand command = new FbCommand();

                FbTransaction transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandText = "select j.ID_ID ID__PO__SCAL, sum(d.ww) WART__PO__SCAL from DZIALKI_N d join JEDN_REJ_N j on j.ID_ID = d.rjdr where id_rd <> 0 and (j.id_sti <> 1 or j.id_sti is null) group by j.id_id";
                FbDataAdapter adapter = new FbDataAdapter(command);
                dt = new DataTable();
                adapter.Fill(dt);

                zsumwaneWartosciStanPO = new List<ZsumwaneWartosciZPorownania>();
                dgPorownanie.ItemsSource = zsumwaneWartosciStanPO;

                dgPorownanie.Items.Refresh();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    zsumwaneWartosciStanPO.Add(new ZsumwaneWartosciZPorownania()
                    {
                        IdPo = Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]),
                        WartPo = Convert.ToDecimal(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1].ToString().Replace(".", ","))
                    });
                }

                command.CommandText = "select ID_ID, ijr from jedn_rej_N where id_sti <> 1 or id_sti is null order by ID_ID";

                adapter = new FbDataAdapter(command);
                dt = new DataTable();

                adapter.Fill(dt);
                List<IdJrStNKRJeNowej> idJrNowejNKRJeNowej = new List<IdJrStNKRJeNowej>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    idJrNowejNKRJeNowej.Add(new IdJrStNKRJeNowej(
                        Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]),
                        Convert.ToInt32(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1]), 0.99
                        ));
                }



                // stan przed 

                command.CommandText = "select replace(d.ww,'.',','), j.ijr, j.nkr, j.idgrp, jsn.id_jednn, replace(jsn.ud_nr,'.',',') from dzialka d join jedn_rej j on j.ID_ID = d.rjdr join jedn_sn jsn on jsn.id_jedns=j.id_id join jedn_rej_n jn on jn.id_id = jsn.id_jednn where jn.id_sti <> 1 or jn.id_sti is null order by idgrp";


                adapter = new FbDataAdapter(command);
                dt = new DataTable();

                adapter.Fill(dt);
                Console.WriteLine("LINE---267");
                //  dgPorownanie.ItemsSource = dt.AsDataView();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    stanPrzedWartoscis.Add(new StanPrzedWartosci(Convert.ToDecimal(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]),
                        Convert.ToInt32(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1]),
                        Convert.ToInt32(dt.Rows[i][2].Equals(System.DBNull.Value) ? null : dt.Rows[i][2]),
                        Convert.ToInt32(dt.Rows[i][3].Equals(System.DBNull.Value) ? null : dt.Rows[i][3]),
                        Convert.ToInt32(dt.Rows[i][4].Equals(System.DBNull.Value) ? null : dt.Rows[i][4]),
                        Convert.ToDouble(dt.Rows[i][5].Equals(System.DBNull.Value) ? null : dt.Rows[i][5])
                        ));
                }

                resultPorownanie = stanPrzedWartoscis.GroupBy(l => l.id_id).Select(cl =>
             new ZsumwaneWartosciZPorownania
             {

                 NKR = (int)cl.First().NKR,
                 IdPo = (int)cl.First().id_id,
                 WartPrzed = cl.Sum(c => (c.Wartosc * (decimal)c.udzialPrzed))
             }).ToList();

                Console.WriteLine("punkt kontrolny 2");


                Console.WriteLine("________________________________________");
                // resultPorownanie.ForEach(x => x.wypiszWConsoli());
                Console.WriteLine("________________________________________");


                foreach (var item in resultPorownanie)
                {

                    if (zsumwaneWartosciStanPO.Exists(x => x.IdPo == item.IdPo))
                    {
                        // Console.WriteLine("if "+ item.IdPo + " " + item.NKR + "<Przed Po>"+ zsumwaneWartosciStanPO.Find((x => x.NKR == idJrNowejNKRJeNowej.Find(xa => xa.idJednNowej == item.IdPo).NKRJednNowej)).NKR);

                        zsumwaneWartosciStanPO.Find(x => x.IdPo == item.IdPo).WartPrzed += item.WartPrzed;
                        if (zsumwaneWartosciStanPO.Find(x => x.IdPo == item.IdPo).NKR == 0)
                        {
                            zsumwaneWartosciStanPO.Find(x => x.IdPo == item.IdPo).NKR = idJrNowejNKRJeNowej.Find(xa => xa.idJednNowej == item.IdPo).NKRJednNowej;
                            zsumwaneWartosciStanPO.Find(x => x.IdPo == item.IdPo).Nkr_Przed = item.NKR;
                        }
                    }
                    else
                    {
                        zsumwaneWartosciStanPO.Add(new ZsumwaneWartosciZPorownania() { Nkr_Przed = item.NKR, IdPo = item.IdPo, WartPrzed = item.WartPrzed, NKR = idJrNowejNKRJeNowej.Find(x => x.idJednNowej == item.IdPo).NKRJednNowej });
                    }
                }

                foreach (var item in zsumwaneWartosciStanPO.FindAll(x => x.NKR == 0))
                {
                    item.NKR = idJrNowejNKRJeNowej.Find(x => x.idJednNowej == item.IdPo).NKRJednNowej;
                }

                //command.CommandText = "select ijr, case when zgoda is null then 0 else zgoda end zgoda, case when odcht is null then 0 else odcht end odcht from jedn_rej_N where id_sti <> 1 or id_sti is null order by ID_ID";
                command.CommandText = "select ijr, case when zgoda is null then 0 else zgoda end zgoda, case when odcht is null then 0 else odcht end odcht, DPLDR, PTR, case when (select first 1  (select ptr from jedn_rej js where js.id_id = sn.id_jedns) from jedn_sn sn where id_jednn = jn.id_id) = 1 then 1 else 0 end ptr_przed from jedn_rej_N jn where id_sti <> 1 or id_sti is null order by ID_ID";
                adapter = new FbDataAdapter(command);
                dt = new DataTable();
                adapter.Fill(dt);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (zsumwaneWartosciStanPO.Exists(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])))
                    {
                        zsumwaneWartosciStanPO.Find(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])).ZgodawProgramie = Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][1]) == 1 ? true : false;
                        zsumwaneWartosciStanPO.Find(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])).OdchWProgramie = Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][2]) == 1 ? true : false;
                        zsumwaneWartosciStanPO.Find(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])).NieDoliczajDoplatyZaDrogi = Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][3]) == 1 ? true : false;
                        zsumwaneWartosciStanPO.Find(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])).ZerujDoplaty = Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][4]) == 1 ? true : false;
                        zsumwaneWartosciStanPO.Find(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])).PotraceniaPrzed = Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][5]) == 1 ? true : false;

                    }
                }

                //dołączenie wartosci gospodarstwa z tabeli jedn_sn, Należy porównać to z moją wartością przed scaleniem bo są babole 
                command.CommandText = "select jn.ijr, sum(round(jsn.wwgsp,2)) from jedn_sn jsn join jedn_rej_n jn on jn.id_id = jsn.id_jednn group by ijr order by ijr";
                adapter = new FbDataAdapter(command);
                dt = new DataTable();
                adapter.Fill(dt);
                Console.WriteLine("PRZED WEJSCIEM DO PETLI");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var tmp = zsumwaneWartosciStanPO.Find(x => x.NKR == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]));
                    if (tmp != null)
                    {
                        tmp.WGSPzJednSN = Convert.ToDecimal(dt.Rows[i][1].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][1]);
                        tmp.RozniceWGSPZJednIWartPrzed = tmp.WartPrzed - tmp.WGSPzJednSN;
                    }
                }

                //dołączenie Właściciela w formie z udziałęm np  1/2-Jan Kowalski; 1/2-Irena Wacławska
                command.CommandText = Constants.SQLIdJNUdWlasciciel;
                adapter = new FbDataAdapter(command);
                dt = new DataTable();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (zsumwaneWartosciStanPO.Exists(x => x.IdPo == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0])))
                    {
                        var tmp = zsumwaneWartosciStanPO.Find(x => x.IdPo == Convert.ToInt32(dt.Rows[i][0].Equals(System.DBNull.Value) ? null : dt.Rows[i][0]));
                        tmp.NazwaWlasnosci = dt.Rows[i][1].ToString();
                    }
                }
                try
                {
                    dgPorownanie.Visibility = Visibility.Visible;
                    dgPorownanie.Items.Refresh();
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp);
                }
                connection.Close();
            }
        }

        private void CheckBoxZawszeNaWierzchu_Checked(object sender, RoutedEventArgs e)
        {
            windowPorownajPrzedPo.Topmost = true;
        }

        private void CheckBoxZawszeNaWierzchu_Unchecked(object sender, RoutedEventArgs e)
        {
            windowPorownajPrzedPo.Topmost = false;
        }

        private void UpdateProgress()
        {
            progresBar.Value += 1;
        }
        private delegate void ProgressBarDelegate();

        private void MenuItem_ClickUstawOdchylkeTechniczna(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {
                var resultat = MessageBox.Show("Czy chcesz rozpocząć ładowanie do bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE JEDN_REJ_N SET ODCHT = @wart01 where ID_ID IN(@ID_ID)", connection);
                        //FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = CASE ID_ID WHEN @IDDZ THEN @RJDRPRZED END WHERE ID_ID = @IDDZ2", connection);
                        progresBar.Value = 0;
                        progresBar.Visibility = Visibility.Visible;
                        Console.WriteLine(zsumwaneWartosciStanPO.Count + "COUNT ZSUMOWANE");
                        foreach (var item in zsumwaneWartosciStanPO)
                        {
                            item.wypiszWConsoli();
                        }
                        if (!zsumwaneWartosciStanPO.Equals(null))
                        {


                            int ilePrzypisacOchTechn = zsumwaneWartosciStanPO.Count;
                            progresBar.Maximum = ilePrzypisacOchTechn;


                            foreach (var item in zsumwaneWartosciStanPO)
                            {


                                if (item.CzyDopOdch__3__proc != null)
                                {


                                    writeCommand.Parameters.Add("@ID_ID", item.IdPo);

                                    if (item.CzyDopOdch__3__proc.ToUpper() == "NIE")
                                    {
                                        int zero = 0;
                                        writeCommand.Parameters.Add("@wart01", zero);
                                        Console.WriteLine("0  IdPo:" + item.IdPo + " NKR:" + item.NKR);
                                    }
                                    else
                                    {
                                        writeCommand.Parameters.Add("@wart01", 1);
                                        Console.WriteLine("1  IdPo:" + item.IdPo + " NKR:" + item.NKR);
                                    }
                                    writeCommand.ExecuteNonQuery();
                                    writeCommand.Parameters.Clear();
                                }
                                else
                                {
                                    textBlockLogInfo.Text += " BŁ.ODCH.: " + item.NKR;
                                }


                                progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                            }
                            connection.Close();
                            MessageBox.Show("Przypisano pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                            progresBar.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }

        private void MenuItem_ClickUsunWszystkieOchylkiTechniczne(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz usunąć wcześniej zaznaczone ''Odchyłki techniczne'' z bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujConnectionStringZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE jedn_rej_N SET ODCHT = 0", connection);
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("Usunięto zaznaczenie pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                    }
                }
            }
            catch
            {

            }
        }

        private void MenuItem_ClickUstawZgodeNaPowiekszGosp(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {
                var resultat = MessageBox.Show("Czy chcesz rozpocząć ładowanie do bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE JEDN_REJ_N SET zgoda = @zgoda01 where ID_ID IN(@ID_ID)", connection);
                        //FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = CASE ID_ID WHEN @IDDZ THEN @RJDRPRZED END WHERE ID_ID = @IDDZ2", connection);
                        progresBar.Value = 0;
                        progresBar.Visibility = Visibility.Visible;
                        Console.WriteLine(zsumwaneWartosciStanPO.Count + "COUNT ZSUMOWANE");
                        foreach (var item in zsumwaneWartosciStanPO)
                        {
                            item.wypiszWConsoli();
                        }
                        if (!zsumwaneWartosciStanPO.Equals(null))
                        {
                            int ilePrzypisacOchTechn = zsumwaneWartosciStanPO.Count;
                            progresBar.Maximum = ilePrzypisacOchTechn;

                            foreach (var item in zsumwaneWartosciStanPO)
                            {
                                if (item.CzyDopOdch__3__proc != null)
                                {
                                    writeCommand.Parameters.Add("@ID_ID", item.IdPo);

                                    if (item.CzyDopOdch__3__proc.ToUpper() == "NIE" && item.Roznice != 0)
                                    {
                                        writeCommand.Parameters.Add("@zgoda01", 1);
                                        Console.WriteLine("1  IdPo:" + item.IdPo + " NKR:" + item.NKR);
                                    }
                                    else
                                    {
                                        int zero = 0;
                                        writeCommand.Parameters.Add("@zgoda01", zero);
                                        Console.WriteLine("0  IdPo:" + item.IdPo + " NKR:" + item.NKR);
                                    }

                                    writeCommand.ExecuteNonQuery();
                                    writeCommand.Parameters.Clear();

                                }
                                else
                                {
                                    textBlockLogInfo.Text += " BŁ.ZG.: " + item.NKR;
                                }

                                progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                            }
                            connection.Close();
                            MessageBox.Show("Przypisano pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                            progresBar.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }

        private void MenuItem_ClickUsunWszystkieZgody(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz usunąć wcześniej zaznaczone ''Zgody na powiększenie gospodarstwa'' z bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujConnectionStringZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE jedn_rej_N SET ZGODA = 0", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("Usunięto zaznaczenie pomyślnie.", "SUKCES!", MessageBoxButton.OK);

                    }
                }
            }
            catch
            {

            }
        }

        private void MenuItem_Click_ustawoOdchylkeWszystkim(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz zaznaczyć wszystkim ''Odchyłki techniczne''?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujConnectionStringZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE jedn_rej_N SET ODCHT = 1", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("Zaznaczono pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                    }
                }
            }
            catch
            {

            }
        }

        private void MenuItem_ClickUstawZgodeNaPowGospWszystkim(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz zaznaczyć wszystkim ''Zgodę na powiększenie gospodarstwa''?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujConnectionStringZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE jedn_rej_N SET ZGODA = 1", connection);
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("Zaznaczono pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                    }
                }
            }
            catch
            {

            }
        }


        private void DgPorownanie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var selectedValueComboBox = comboBoxEditColumn.SelectedValue.ToString();
                //var oneZwzp = dgPorownanie.SelectedItem as ZsumwaneWartosciZPorownania;
                var multiply = dgPorownanie.SelectedItems;


                foreach (var oneZwzp in multiply)
                {
                    if (nameof(ZsumwaneWartosciZPorownania.OdchWProgramie) == selectedValueComboBox)
                    {
                        Console.WriteLine("Odchyłka");
                    }
                    else if (nameof(ZsumwaneWartosciZPorownania.ZgodawProgramie) == selectedValueComboBox)
                    {
                        Console.WriteLine("Zgoda");
                    }

                    //    oneZwzp.OdchWProgramie = oneZwzp.OdchWProgramie == true ? false : true;
                    //var a = multiply.Where(x => x == oneZwzp).Select(x => new { x.OdchWProgramie }).First();

                    //Console.WriteLine($"{oneZwzp.IdPo.ToString()}, {oneZwzp.ZgodawProgramie.ToString()}, {oneZwzp.OdchWProgramie.ToString()}");
                }






                dgPorownanie.Items.Refresh();
            }
        }
    }
}
