using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using ScaleniaMW.Entities;
using ScaleniaMW.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy WindowPrzypiszRejGr.xaml
    /// </summary>
    public partial class WindowPrzypiszRejGr : Window
    {

        public WindowPrzypiszRejGr()
        {
            InitializeComponent();
            try
            {
                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
                Console.WriteLine("ASSMBLY VERSJA: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                windowPrzypiszRejGr.Title += " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + " problem z oknem");
            }

        }

        List<DzialkaEDZ> listaZEDZ = new List<DzialkaEDZ>();
        List<DzialkaNkrZSQL> listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
        DataTable dt;
        public static string connectionString;

        public static void aktualizujSciezkeZPropertis()
        {
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource= localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";

            //connectionString = $@"User={Properties.Settings.Default.Login};Password={Properties.Settings.Default.Haslo};Database={Properties.Settings.Default.PathFDB}; DataSource=localhost; Port= { Constants.PortFB } ;Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
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

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    ustawProperties(dlg.FileName);
                    czyPolaczonoZBaza = false;
                    itemImportJednostkiSN.Background = Brushes.Transparent;
                    itemImportJednostkiSN.Header = "Baza.fdb";

                    listaDopasowJednos_CzyLadowac = new List<DopasowanieJednostek>();
                    listaDopasowJednos = new List<DopasowanieJednostek>();

                    dgNiedopJednostki.ItemsSource = null;
                    listBoxDzialkiNowe.ItemsSource = null;
                    listBoxNkr.ItemsSource = null;
                    listBoxNrRej.ItemsSource = null;

                    RefresUI();
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


        public void RefresUI()
        {
            listBoxDzialkiNowe.Items.Refresh();
            listBoxNkr.Items.Refresh();
            listBoxNrRej.Items.Refresh();
            dgNiedopJednostki.Items.Refresh();
        }

        public void SetSelectedIndex0()
        {
            listBoxNkr.SelectedIndex = 0;
            listBoxDzialkiNowe.SelectedIndex = 0;
            listBoxNrRej.SelectedIndex = 0;
        }


        MainDbContext dbContext;
        AssigedUnitService assigedUnitService;
        List<DopasowanieJednostek> listaDopasowJednos = new List<DopasowanieJednostek>();
        List<DopasowanieJednostek> listaDopasowJednos_CzyLadowac = new List<DopasowanieJednostek>();
        bool czyPolaczonoZBaza = false;
        private void ItemImportJednostkiSN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                aktualizujSciezkeZPropertis();
                this.dbContext = new MainDbContext(connectionString);
                assigedUnitService = new AssigedUnitService(dbContext);
                assigedUnitService.FillUI((WindowPrzypiszRejGr)windowPrzypiszRejGr);
                SetSelectedIndex0();

            }
            catch (Exception ex)
            {

                itemImportJednostkiSN.Background = Brushes.Red;
                itemImportJednostkiSN.Header = "Baza.fdb";
                textBlockLogInfo.Text = "Problem z połączeniem z bazą FDB " + ex.Message;
                Console.WriteLine(ex);
                if (ex.Message.ToLower().Contains("password"))
                {
                    UstawLoginIHaslo();
                }
            }
        }

        private void Button_PrzypiszZaznJedn(object sender, RoutedEventArgs e)
        {
            bool connected = dbContext == null ? false : dbContext.Database.Exists();
            if (connected)
            {
                assigedUnitService.AssignedSelected((WindowPrzypiszRejGr)windowPrzypiszRejGr);
                RefresUI();
            }
        }


        private void Button_ZaladujDoBazy(object sender, RoutedEventArgs e)
        {
            bool connected = dbContext == null ? false : dbContext.Database.Exists();

            if (connected)
            {
                var toDatabase = assigedUnitService.DataToLoadDb();

                foreach (var parcel in toDatabase)
                {
                    dbContext.Dzialki_nowe.FirstOrDefault(x => x.ID_ID == parcel.id_parcel).RJDRPRZED = parcel.idJednPrzed;
                    Console.WriteLine(string.Join(", ", parcel.NKR, parcel.Nrdz, parcel.IJR));
                }

                dbContext.SaveChanges();
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }

        private void MenuItem_AutoPrzypiszJednostki(object sender, RoutedEventArgs e)
        {
            bool connected = dbContext == null ? false : dbContext.Database.Exists();

            if (connected)
            {
                assigedUnitService.AutoAssignment();
                assigedUnitService.FillUI(windowPrzypiszRejGr);
                RefresUI();
            }
        }

        private void UpdateProgress()
        {
            progresBar.Value += 1;
        }

        private delegate void ProgressBarDelegate();

        void UstawLoginIHaslo()
        {
            textBoxLogin.Text = Properties.Settings.Default.Login;
            passwordBoxLogowanie.Password = Properties.Settings.Default.Haslo;
            panelLogowania2.Visibility = Visibility.Visible;
            tabItemNiedopasowJedn.Visibility = Visibility.Hidden;
        }

        private void UstawLoginIHaslo2(object sender, RoutedEventArgs e)
        {
            UstawLoginIHaslo();
        }

        private void ButtonZapiszLogIHaslo2(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Haslo != passwordBoxLogowanie.Password)
            {

                czyPolaczonoZBaza = false;
                itemImportJednostkiSN.Background = Brushes.Transparent;
                itemImportJednostkiSN.Header = "Baza.fdb";

                listaDopasowJednos_CzyLadowac = new List<DopasowanieJednostek>();
                listaDopasowJednos = new List<DopasowanieJednostek>();

                dgNiedopJednostki.ItemsSource = null;
                listBoxDzialkiNowe.ItemsSource = null;
                listBoxNkr.ItemsSource = null;
                listBoxNrRej.ItemsSource = null;

                listBoxDzialkiNowe.Items.Refresh();
                listBoxNkr.Items.Refresh();
                listBoxNrRej.Items.Refresh();
                dgNiedopJednostki.Items.Refresh();
            }

            Properties.Settings.Default.Login = textBoxLogin.Text;
            Properties.Settings.Default.Haslo = passwordBoxLogowanie.Password;
            Properties.Settings.Default.Save();
            // textBoxHaslo.Text = "";
            panelLogowania2.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            tabItemNiedopasowJedn.Visibility = Visibility.Visible;
        }

        private void Button_Anuluj2(object sender, RoutedEventArgs e)
        {
            panelLogowania2.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            tabItemNiedopasowJedn.Visibility = Visibility.Visible;
        }

        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowPrzypiszRejGr.Close();
        }

        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ItemUsunPrzypisaneJednostkiZBazy(object sender, RoutedEventArgs e)
        {
            bool connected = dbContext == null ? false : dbContext.Database.Exists();

            if (connected)
            {
                var resultat = MessageBox.Show("Czy chcesz usunąć wcześniej przypisane jednostki z bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {

                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = null", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("Jednostki usunięto pomyślnie.", "SUKCES!", MessageBoxButton.OK);

                        ItemImportJednostkiSN_Click(sender, e);
                    }
                }
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }

        private void CheckBoxZawszeNaWierzchu_Checked(object sender, RoutedEventArgs e)
        {
            windowPrzypiszRejGr.Topmost = true;
        }

        private void CheckBoxZawszeNaWierzchu_Unchecked(object sender, RoutedEventArgs e)
        {
            windowPrzypiszRejGr.Topmost = false;
        }

        private void DgNiedopJednostki_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                //  Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej);

                Console.WriteLine("CHANGED ");

                listBoxNkr.Items.Refresh();
                listBoxNrRej.Items.Refresh();
                listBoxDzialkiNowe.Items.Refresh();
                dgNiedopJednostki.Items.Refresh();
            }
            catch (Exception)
            {

                Console.WriteLine(" catch DgNiedopJednostki_CurrentCellChanged");
            }

        }

        private void ButtonPrzypisz_MouseEnter(object sender, MouseEventArgs e)
        {
            buttonPrzypisz.Foreground = Brushes.Black;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/pen.png", UriKind.Relative);
            bi3.EndInit();
            imageHand.Stretch = Stretch.Fill;
            imageHand.Source = bi3;
        }

        private void ButtonPrzypisz_MouseLeave(object sender, MouseEventArgs e)
        {
            buttonPrzypisz.Foreground = Brushes.White;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/PenWhite.png", UriKind.Relative);
            bi3.EndInit();
            imageHand.Stretch = Stretch.Fill;
            imageHand.Source = bi3;
        }

        private void NadajNKrWStaniePo(object sender, RoutedEventArgs e)
        {
            try
            {
                aktualizujSciezkeZPropertis();
                using (var connection = new FbConnection(connectionString))
                {
                    connection.Open();
                    FbCommand command = new FbCommand();
                    FbTransaction transaction = connection.BeginTransaction();

                    command.Connection = connection;
                    command.Transaction = transaction;
                    // działające zapytanie na nrobr-nrdz NKR 
                    //  command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case WHEN JEDN_REJ.nkr is null then obreby.id * 1000 + JEDN_REJ.grp else JEDN_REJ.nkr end as NKR_Z_GRUPAMI from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ on dzialka.rjdr = JEDN_REJ.id_id order by NKR_Z_GRUPAMI";
                    // command.CommandText = "select sn.id_jednn, sn.id_jedns, js.ijr stara_jedn_ewop, jn.ijr nowy_nkr from JEDN_SN sn join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn order by id_jednn";
                    // command.CommandText = "select  j.id_id from jedn_rej_N j join obreby o on o.id_ID = j.id_obr join gminy g on g.id_id = j.id_gm where j.id_sti <> 1 or j.id_sti is null order by g.teryt, o.id, j.ijr";
                    command.CommandText = "select distinct j.id_id from jedn_rej_N j left join dzialki_n dn on dn.rjdr = j.iD_ID left join obreby o on o.id_ID = j.id_obr left join gminy g on g.id_id = j.id_gm where j.id_sti <> 1 or j.id_sti is null order by j.ijr";
                    FbDataAdapter adapter = new FbDataAdapter(command);
                    dt = new DataTable();

                    adapter.Fill(dt);

                    Console.WriteLine("row count:" + dt.Rows.Count);
                    Console.WriteLine("column count:" + dt.Columns.Count);

                    if (0 == dt.Rows.Count)
                    {
                        textBlockLogInfo.Text = "Brak danych";
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Console.WriteLine(dt.Rows[i][0]);
                    }

                    connection.Close();
                    itemImportJednostkiSN.Background = Brushes.LightSeaGreen;
                    itemImportJednostkiSN.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
                    dgNiedopJednostki.Items.Refresh();
                    czyPolaczonoZBaza = true;
                }
                try
                {
                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE JEDN_REJ_N SET nkr= @NKR where Id_Id IN(@IDID)", connection);
                        //FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = CASE ID_ID WHEN @IDDZ THEN @RJDRPRZED END WHERE ID_ID = @IDDZ2", connection);

                        progresBar.Value = 0;
                        progresBar.Visibility = Visibility.Visible;
                        progresBar.Maximum = dt.Rows.Count;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            Console.WriteLine(dt.Rows[i][0]);

                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            writeCommand.Parameters.Add("@NKR", i + 1);
                            writeCommand.Parameters.Add("@IDID", dt.Rows[i][0]);
                            writeCommand.ExecuteNonQuery();
                            writeCommand.Parameters.Clear();
                            progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                        }
                        connection.Close();
                        MessageBox.Show("NKR przypisano pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                        progresBar.Visibility = Visibility.Hidden;
                    }
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                itemImportJednostkiSN.Background = Brushes.Red;
                itemImportJednostkiSN.Header = "Baza.fdb";
                textBlockLogInfo.Text = "Problem z połączeniem z bazą FDB " + ex.Message;
            }
        }

        private void MenuItem_ClickWyczyscNKRwStaniePo(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz usunąć wcześniej przypisane NKRy z bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE jedn_rej_N SET NKR = null", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("NKRy usunięto pomyślnie.", "SUKCES!", MessageBoxButton.OK);

                    }
                }
            }
            catch
            {

            }
        }

        //private void MenuItem_Click_PrzejdzDoWyboryRodzajuNumeracjiNKRu(object sender, RoutedEventArgs e)
        //{
        //    StackPanelWyboruRodzajuNumeracji.Visibility = Visibility.Visible;
        //}

        //private void Button_Click_AnulujAutonumeracje(object sender, RoutedEventArgs e)
        //{
        //    StackPanelWyboruRodzajuNumeracji.Visibility = Visibility.Hidden;
        //}

        //private void Button_Click_KontynuujAutonumerowanie(object sender, RoutedEventArgs e)
        //{
        //    int? constDoWyciagnieciaNrRej = null;
        //    StackPanelWyboruRodzajuNumeracji.Visibility = Visibility.Hidden;
        //    if ((bool)radioButtonNRJ.IsChecked)
        //    {
        //        constDoWyciagnieciaNrRej = 0;
        //    }
        //    else if ((bool)radioButton1000_NRJ.IsChecked)
        //    {
        //        constDoWyciagnieciaNrRej = 1000;
        //    }
        //    else if ((bool)radioButton10000_NRJ.IsChecked)
        //    {
        //        constDoWyciagnieciaNrRej = 10000;
        //    }
        //    else if ((bool)radioButton100000_NRJ.IsChecked)
        //    {
        //        constDoWyciagnieciaNrRej = 100000;
        //    }

        //    if (czyPolaczonoZBaza)
        //    {
        //        Console.WriteLine(sender.GetHashCode());
        //        Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej, "AutoPrzypiszJednostkiZDoborem", constDoWyciagnieciaNrRej);
        //        dgNiedopJednostki.Items.Refresh();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
        //    }
        //}

        private void MenuItem_ClickPrzypiszDoNKRPoIjrPrzed(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz przypisać [IJR przed] do [NKR po]?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE JEDN_REJ_N JN SET NKR = (select IJR from JEDN_REJ JS where JS.NKR = JN.IJR) WHERE NKR is null", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("NKRy usunięto pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Coś poszło nie tak", "Smuteczek :(", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItem_ClickPrzypiszGrRej(object sender, RoutedEventArgs e)
        {
            try
            {
                var resultat = MessageBox.Show("Czy chcesz przypisać grupę rejestrową w stanie po scaleniu na podstawie stanu 'przed'?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);
                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("update UDZIALY_N set grj =( SELECT first 1 grj FROM udzialy u  WHERE udzialy_n.id_podm = u.id_podm and u.grj is not null and (udzialy_n.id_podm is not null or udzialy_n.id_podm not like ''))", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("Grupy rejestrowe przypisano pomyślnie!", "SUKCES!", MessageBoxButton.OK);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Coś poszło nie tak", "Smuteczek!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBoxNkr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            assigedUnitService.FillUI(windowPrzypiszRejGr);
            listBoxNrRej.SelectedIndex = 0;
            listBoxDzialkiNowe.SelectedIndex = 0;
        }
        //koniec klasy
    }
}

