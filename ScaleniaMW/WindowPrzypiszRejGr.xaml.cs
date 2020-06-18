using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
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
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=3050;Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
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

                    listBoxDzialkiNowe.Items.Refresh();
                    listBoxNkr.Items.Refresh();
                    listBoxNrRej.Items.Refresh();
                    dgNiedopJednostki.Items.Refresh();
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


        List<DopasowanieJednostek> listaDopasowJednos = new List<DopasowanieJednostek>();
        List<DopasowanieJednostek> listaDopasowJednos_CzyLadowac = new List<DopasowanieJednostek>();
        bool czyPolaczonoZBaza = false;
        private void ItemImportJednostkiSN_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                listBoxDzialkiNowe.Items.Refresh();
                listBoxNkr.Items.Refresh();
                listBoxNrRej.Items.Refresh();
                dgNiedopJednostki.Items.Refresh();
                aktualizujSciezkeZPropertis();
                using (var connection = new FbConnection(connectionString))
                {
                    if (itemImportJednostkiSN.Background.Equals(Brushes.LightSeaGreen))
                    {
                        var resultat = MessageBox.Show("Czy chcesz pobrać dane z bazy ponownie\n i nadpisać obecnie załadowany plik?", "UWAGA!", MessageBoxButton.YesNo);

                        if (resultat == MessageBoxResult.No)
                        {
                            goto koniec;
                        }
                        else
                        {
                            listaDopasowJednos_CzyLadowac = new List<DopasowanieJednostek>();
                            listaDopasowJednos = new List<DopasowanieJednostek>();

                            listBoxDzialkiNowe.Items.Refresh();
                            listBoxNkr.Items.Refresh();
                            listBoxNrRej.Items.Refresh();
                            dgNiedopJednostki.Items.Refresh();
                        }

                    }

                    connection.Open();

                    FbCommand command = new FbCommand();

                    FbTransaction transaction = connection.BeginTransaction();

                    command.Connection = connection;
                    command.Transaction = transaction;
                    // działające zapytanie na nrobr-nrdz NKR 
                    //  command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case WHEN JEDN_REJ.nkr is null then obreby.id * 1000 + JEDN_REJ.grp else JEDN_REJ.nkr end as NKR_Z_GRUPAMI from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ on dzialka.rjdr = JEDN_REJ.id_id order by NKR_Z_GRUPAMI";
                    // command.CommandText = "select sn.id_jednn, sn.id_jedns, js.ijr stara_jedn_ewop, jn.ijr nowy_nkr from JEDN_SN sn join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn order by id_jednn";
                    command.CommandText = "select sn.id_jednn, sn.id_jedns, js.ijr stara_jedn_ewop, jn.ijr nowy_nkr, dn.idd, dn.id_id, dn.rjdrprzed, " +
                        "js.NKR stary_nkr,  dn.pew/10000, dn.ww from JEDN_SN sn " +
                        "join JEDN_REJ js on js.ID_ID = sn.id_jedns join JEDN_REJ_N jn on jn.ID_ID = sn.id_jednn join dzialki_n dn on dn.rjdr = jn.id_id order by id_jednn";
                    FbDataAdapter adapter = new FbDataAdapter(command);
                    dt = new DataTable();

                    adapter.Fill(dt);
                    foreach (var item in dt.Columns)
                    {

                        Console.Write(item + " << ");
                    }

                    Console.WriteLine("row count:" + dt.Rows.Count);
                    Console.WriteLine("column count:" + dt.Columns.Count);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        listaDopasowJednos_CzyLadowac.Add(new DopasowanieJednostek((int)dt.Rows[i][0], (int)dt.Rows[i][1], (int)dt.Rows[i][2], (int)dt.Rows[i][3], dt.Rows[i][4].ToString(), (int)dt.Rows[i][5], dt.Rows[i][6]));
                        listaDopasowJednos.Add(new DopasowanieJednostek((int)dt.Rows[i][0], (int)dt.Rows[i][1], (int)dt.Rows[i][2], (int)dt.Rows[i][3], dt.Rows[i][4].ToString(), (int)dt.Rows[i][5], dt.Rows[i][6]));
                    }
                    try
                    {
                        //dataGrid.ItemsSource = dt.AsDataView();
                        //dataGrid.Visibility = Visibility.Visible;
                        //dataGrid.Items.Refresh();
                        //dgNkrFDB.ItemsSource = dt.AsDataView();

                        dgNiedopJednostki.ItemsSource = listaDopasowJednos;
                        dgNiedopJednostki.Items.Refresh();

                        Console.WriteLine("ustawiam SOURCE");
                    }
                    catch (Exception excp)
                    {
                        Console.WriteLine(excp);
                    }

                    if (0 == dt.Rows.Count)
                    {
                        textBlockLogInfo.Text = "Brak danych";
                    }

                    Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej);

                    connection.Close();
                    itemImportJednostkiSN.Background = Brushes.LightSeaGreen;
                    itemImportJednostkiSN.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
                    dgNiedopJednostki.Items.Refresh();
                    czyPolaczonoZBaza = true;

                    koniec:;
                }
            }
            catch (Exception ex)
            {

                itemImportJednostkiSN.Background = Brushes.Red;
                itemImportJednostkiSN.Header = "Baza.fdb";
                textBlockLogInfo.Text = "Problem z połączeniem z bazą FDB " + ex.Message;
            }
        }

        private void ListBoxNkr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej);
           listBoxDzialkiNowe.SelectedIndex = listBoxDzialkiNowe.SelectedIndex >= 0 && listBoxDzialkiNowe.SelectedIndex < listBoxDzialkiNowe.Items.Count ? listBoxDzialkiNowe.SelectedIndex : 0;

        }

        private void ListBoxDzialkiNowe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej);
            listBoxNrRej.SelectedIndex = listBoxNrRej.SelectedIndex >= 0 && listBoxNrRej.SelectedIndex < listBoxNrRej.Items.Count ? listBoxNrRej.SelectedIndex : 0;
        }

        private void Button_PrzypiszZaznJedn(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {


                Console.WriteLine(" super ");
                Console.WriteLine(sender.GetHashCode());
                Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej, "PrzypiszZaznJedn");

                listBoxNkr.Items.Refresh();
                listBoxDzialkiNowe.Visibility = Visibility.Hidden;
                listBoxDzialkiNowe.Visibility = Visibility.Visible;
                listBoxDzialkiNowe.Items.Refresh();
                listBoxNrRej.Items.Refresh();

                tabItemNiedopasowJedn.Visibility = Visibility.Hidden;
                tabItemNiedopasowJedn.Visibility = Visibility.Visible;
                dgNiedopJednostki.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }

        private void Zapisz_Dopasowanie_Jedn_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = ".txt";
            svd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (svd.ShowDialog() == true)
            {
                using (Stream s = File.Open(svd.FileName, FileMode.Create))
                //  using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                    try
                    {
                        try
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("[IddN] [idJednS] [NKRN] [NrDzN] [NrRejGr]");
                            foreach (var item in listaDopasowJednos)
                            {

                                stringBuilder.AppendLine(item.IdDz + "\t" + item.IdJednS + "\t" + item.NowyNKR + "\t" + item.NrDzialki + "\t" + item.PrzypisanyNrRej);

                            }

                            sw.Write(stringBuilder.ToString());

                            sw.Close();
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.ToString() + "  problem z plikiem");
                        }
                    }
                    catch (Exception ex)
                    {
                        var resultat = MessageBox.Show(ex.ToString() + " Przerwać?", "ERROR", MessageBoxButton.YesNo);

                        if (resultat == MessageBoxResult.Yes)
                        {
                            Application.Current.Shutdown();
                        }
                    }

            }
        }

        private void Button_ZaladujDoBazy(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {


                var resultat = MessageBox.Show("Czy chcesz rozpocząć ładowanie do bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {

                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = CASE Id_Id  WHEN @IDDZ  THEN @RJDRPRZED else RJDRPRZED END where Id_Id IN(@IDDZ)", connection);
                        //FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = CASE ID_ID WHEN @IDDZ THEN @RJDRPRZED END WHERE ID_ID = @IDDZ2", connection);
                        List<int> tmpListaIdDz = new List<int>();
                        tmpListaIdDz = listaDopasowJednos.GroupBy(g => g.IdDz).Select(x => x.Key).ToList();

                        tmpListaIdDz.Sort();
                        Console.WriteLine(tmpListaIdDz.Count);
                        progresBar.Value = 0;
                        progresBar.Visibility = Visibility.Visible;
                        progresBar.Maximum = listaDopasowJednos.FindAll(x => x.PrzypisanyNrRej != null).GroupBy(x => x.IdDz).ToList().Count - listaDopasowJednos_CzyLadowac.FindAll(x => x.PrzypisanyNrRej != null).GroupBy(x => x.IdDz).ToList().Count;
                        for (int i = 0; i <= tmpListaIdDz.Count - 1; i++)
                        {
                            if (!listaDopasowJednos_CzyLadowac.Find(x => x.IdDz.Equals(tmpListaIdDz[i])).PrzypisanyNrRej.HasValue)
                            {
                                if (listaDopasowJednos.Find(x => x.IdDz.Equals(tmpListaIdDz[i])).PrzypisanyNrRej.HasValue)
                                {
                                    writeCommand.Parameters.Add("@IDDZ", tmpListaIdDz[i]);
                                    writeCommand.Parameters.Add("@RJDRPRZED", listaDopasowJednos.Find(x => x.IdDz.Equals(tmpListaIdDz[i])).PrzypisanyNrRej);
                                    writeCommand.ExecuteNonQuery();

                                    writeCommand.Parameters.Clear();
                                    Console.WriteLine(i + " " + listaDopasowJednos.Find(x => x.IdDz.Equals(tmpListaIdDz[i])).PrzypisanyNrRej + " " + listaDopasowJednos.Find(x => x.IdDz.Equals(tmpListaIdDz[i])).NrDzialki);
                                    progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                                }
                            }
                        }
                        connection.Close();
                        MessageBox.Show("Jednostki przypisano pomyślnie.", "SUKCES!", MessageBoxButton.OK);
                        progresBar.Visibility = Visibility.Hidden;
                        ItemImportJednostkiSN_Click(sender, e);
                    }
                }
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }

        private void MenuItem_AutoPrzypiszJednostki(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {
                Console.WriteLine(sender.GetHashCode());
                Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej, "AutoPrzypiszJednostki");
                dgNiedopJednostki.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Najpierw połącz z bazą!", "UWAGA!", MessageBoxButton.OK);
            }
        }
        private void UpdateProgress()
        {
            progresBar.Value += 1;
        }
        private delegate void ProgressBarDelegate();



        private void UstawLoginIHaslo2(object sender, RoutedEventArgs e)
        {
            textBoxLogin.Text = Properties.Settings.Default.Login;
            passwordBoxLogowanie.Password = Properties.Settings.Default.Haslo;
            panelLogowania2.Visibility = Visibility.Visible;
            tabItemNiedopasowJedn.Visibility = Visibility.Hidden;
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
          //  textBoxHaslo.Text = "";
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
            if (czyPolaczonoZBaza)
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


        private void DgNiedopJednostki_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            Console.WriteLine(listaDopasowJednos[e.Row.GetIndex()].PrzypisanyNrRej);
            Console.WriteLine(((TextBox)e.EditingElement).Text.GetType() + "xx");

            if (e.Column.DisplayIndex == kolumnaJednRej.DisplayIndex)
            {
                bool czyPasi = false;
                foreach (var item in listaDopasowJednos.FindAll(x => x.IdDz.Equals(listaDopasowJednos[e.Row.GetIndex()].IdDz)))
                {
                    Console.WriteLine(item.IdDz + " " + item.NrJednEwopis +  " " + item.PrzypisanyNrRej);
                    int tmpNrRej;
                    int.TryParse(((TextBox)e.EditingElement).Text, out tmpNrRej);
                    if (item.IdJednS.Equals(tmpNrRej))
                    {
                        czyPasi = true;
                    }
                }

                foreach (var item in listaDopasowJednos.FindAll(x => x.IdDz.Equals(listaDopasowJednos[e.Row.GetIndex()].IdDz)))
                {
                    if (czyPasi)
                    {
                        Console.WriteLine(czyPasi + " czy pasi");
                        int tmpNrRej;
                        int.TryParse(((TextBox)e.EditingElement).Text,out tmpNrRej);
                        item.PrzypisanyNrRej = tmpNrRej;
                    }
                    else
                    {
                        Console.WriteLine(czyPasi + " czy pasi");
                        ((TextBox)e.EditingElement).Text = null;
                        item.PrzypisanyNrRej = null;
                    }
                }
                foreach (var item in listaDopasowJednos.FindAll(x => x.IdDz.Equals(listaDopasowJednos[e.Row.GetIndex()].IdDz)))
                {
                    Console.WriteLine("pip: " + item.PrzypisanyNrRej);
                }
               
                listBoxNkr.Items.Refresh();
                listBoxNrRej.Items.Refresh();
                listBoxDzialkiNowe.Items.Refresh();
                Obliczenia.DopasujNrRejDoNowychDzialek(ref listaDopasowJednos, listBoxNkr, listBoxDzialkiNowe, listBoxNrRej);
            }
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
        
    }
}

