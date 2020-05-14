﻿using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy WindowPrzypiszKW.xaml
    /// </summary>
    public partial class WindowPrzypiszKW : Window
    {
        public WindowPrzypiszKW()
        {
            InitializeComponent();
            try
            {
                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
                Console.WriteLine("ASSMBLY VERSJA: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                windowPrzypiszKW.Title += " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + " problem z oknem");
            }
        }

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

            // dlg.DefaultExt = ".edz";
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

                    listaDopasowKW_CzyLadowac = new List<DopasowanieKW>();
                    listaDopasowKW = new List<DopasowanieKW>();

                    dgNrKwZSQL.ItemsSource = null;
                    listBoxDzialkiNowe.ItemsSource = null;
                    listBoxNkr.ItemsSource = null;
                    listBoxNrKW.ItemsSource = null;

                    listBoxDzialkiNowe.Items.Refresh();
                    listBoxNkr.Items.Refresh();
                    listBoxNrKW.Items.Refresh();
                    dgNrKwZSQL.Items.Refresh();
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

        public bool czyJestTakiWierszW(List<DopasowanieKW> dopasowanieKWs, DopasowanieKW dkw)
        {
            foreach (var item in dopasowanieKWs)
            {
                if (dkw.IdDzN == item.IdDzN && dkw.IdJednN == item.IdJednN && dkw.IdJednS == item.IdJednS && dkw.KWPoDopasowane == item.KWPoDopasowane && dkw.KWprzed == item.KWprzed && dkw.NKRn == item.NKRn && dkw.NrDZ == item.NrDZ)
                {
                    return true;
                }
            }

            return false;
        }

        List<DopasowanieKW> listaDopasowKW = new List<DopasowanieKW>();
        List<DopasowanieKW> listaDopasowKW_CzyLadowac = new List<DopasowanieKW>();
        bool czyPolaczonoZBaza = false;
        private void ItemImportJednostkiSN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listBoxDzialkiNowe.Items.Refresh();
                listBoxNkr.Items.Refresh();
                listBoxNrKW.Items.Refresh();
                dgNrKwZSQL.Items.Refresh();
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
                            listaDopasowKW_CzyLadowac = new List<DopasowanieKW>();
                            listaDopasowKW = new List<DopasowanieKW>();

                            listBoxDzialkiNowe.Items.Refresh();
                            listBoxNkr.Items.Refresh();
                            listBoxNrKW.Items.Refresh();
                            dgNrKwZSQL.Items.Refresh();
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
                    command.CommandText = "select dn.id_id, dn.idd, ds.kw, dn.kw, jn.ijr NKR, jn.id_id id_NKR, sn.id_jedns from dzialki_N dn " +
                        "join JEDN_REJ_N jn on jn.ID_ID = dn.rjdr join JEDN_SN sn on sn.ID_jednn = dn.rjdr " +
                        "join dzialka ds on ds.rjdr = sn.id_jedns order by dn.rjdr";

                    FbDataAdapter adapter = new FbDataAdapter(command);
                    dt = new DataTable();

                    adapter.Fill(dt);
                    //dgNrKwZSQL.ItemsSource = dt.AsDataView();
                    foreach (var item in dt.Columns)
                    {

                        Console.Write(item + " << ");
                    }

                    Console.WriteLine("row count:" + dt.Rows.Count);
                    Console.WriteLine("column count:" + dt.Columns.Count);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //  listaDopasowJednos_CzyLadowac.Add(new DopasowanieJednostek((int)dt.Rows[i][0], (int)dt.Rows[i][1], (int)dt.Rows[i][2], (int)dt.Rows[i][3], dt.Rows[i][4].ToString(), (int)dt.Rows[i][5], dt.Rows[i][6]));


                        //Console.Write(czyJestTakiWierszW(listaDopasowKW, new DopasowanieKW((int)dt.Rows[i][0], dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3], (int)dt.Rows[i][4], (int)dt.Rows[i][5], (int)dt.Rows[i][6])));
                        //Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString(), dt.Rows[i][5].ToString(), dt.Rows[i][6].ToString());
                        if (!czyJestTakiWierszW(listaDopasowKW, new DopasowanieKW((int)dt.Rows[i][0], dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3], (int)dt.Rows[i][4], (int)dt.Rows[i][5], (int)dt.Rows[i][6])))
                        {
                            listaDopasowKW.Add(new DopasowanieKW((int)dt.Rows[i][0], dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3], (int)dt.Rows[i][4], (int)dt.Rows[i][5], (int)dt.Rows[i][6]));
                            listaDopasowKW_CzyLadowac.Add(new DopasowanieKW((int)dt.Rows[i][0], dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3], (int)dt.Rows[i][4], (int)dt.Rows[i][5], (int)dt.Rows[i][6]));
                        }

                    }
                    try
                    {
                        //dataGrid.ItemsSource = dt.AsDataView();
                        //dataGrid.Visibility = Visibility.Visible;
                        //dataGrid.Items.Refresh();
                        //dgNkrFDB.ItemsSource = dt.AsDataView();
                        Console.WriteLine(listaDopasowKW.Count);
                        dgNrKwZSQL.ItemsSource = listaDopasowKW;
                        dgNrKwZSQL.Items.Refresh();

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

                    Obliczenia.DopasujNrKWDoNowychDzialek(ref listaDopasowKW, listBoxNkr, listBoxDzialkiNowe, listBoxNrKW);

                    connection.Close();
                    itemImportJednostkiSN.Background = Brushes.LightSeaGreen;
                    itemImportJednostkiSN.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
                    dgNrKwZSQL.Items.Refresh();
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
            Obliczenia.DopasujNrKWDoNowychDzialek(ref listaDopasowKW, listBoxNkr, listBoxDzialkiNowe, listBoxNrKW);
            listBoxDzialkiNowe.SelectedIndex = 0;

        }

        private void ListBoxDzialkiNowe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Obliczenia.DopasujNrKWDoNowychDzialek(ref listaDopasowKW, listBoxNkr, listBoxDzialkiNowe, listBoxNrKW);
            // listBoxNrKW.SelectedIndex = 0;

        }

        private void Button_PrzypiszZaznJedn(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {
                Console.WriteLine(" super ");
                Console.WriteLine(sender.GetHashCode());
                Obliczenia.DopasujNrKWDoNowychDzialek(ref listaDopasowKW, listBoxNkr, listBoxDzialkiNowe, listBoxNrKW, "PrzypiszZaznJedn");

                listBoxNkr.Items.Refresh();
                listBoxDzialkiNowe.Items.Refresh();
                listBoxNrKW.Items.Refresh();
                dgNrKwZSQL.Items.Refresh();
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
                            stringBuilder.AppendLine("[IdDZ] [NrDz] [NKRN] [KW]");
                            foreach (var item in listaDopasowKW)
                            {
                                stringBuilder.AppendLine(item.IdDzN + "\t" + item.NrDZ + "\t" + item.NKRn + "\t" + item.KWPoDopasowane);
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


                var resultat = MessageBox.Show("Czy chcesz rozpocząć ładowanie KW do bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {

                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open(); //UPDATE DZIALKI_N SET KW = CASE Id_Id  WHEN 660 THEN 'BI100000' else KW END where Id_Id IN(660)
                        FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET KW = CASE Id_Id  WHEN @IDDZ  THEN @KW else KW END where Id_Id IN(@IDDZ)", connection);
                        //FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET RJDRPRZED = CASE ID_ID WHEN @IDDZ THEN @RJDRPRZED END WHERE ID_ID = @IDDZ2", connection);
                        List<int> tmpListaIdDz = new List<int>();
                        tmpListaIdDz = listaDopasowKW.GroupBy(g => g.IdDzN).Select(x => x.Key).ToList();

                        tmpListaIdDz.Sort();
                        // Console.WriteLine(tmpListaIdDz.Count + "count ");
                        progresBar.Value = 0;
                        progresBar.Visibility = Visibility.Visible;

                        //Console.WriteLine(listaDopasowKW.FindAll(x => x.KWPoDopasowane != null).GroupBy(x => x.IdDzN).ToList().Count);

                        // Console.WriteLine(listaDopasowKW_CzyLadowac.FindAll(x => x.KWPoDopasowane != null).GroupBy(x => x.IdDzN).ToList().Count);
                        progresBar.Maximum = 0;
                        for (int i = 0; i <= tmpListaIdDz.Count - 1; i++)
                        {
                            if (listaDopasowKW_CzyLadowac.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane != listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane)
                            {
                                progresBar.Maximum++;
                            }

                        }
                        // progresBar.Maximum = listaDopasowKW.FindAll(x => x.KWPoDopasowane != null).GroupBy(x => x.IdDzN).ToList().Count - listaDopasowKW_CzyLadowac.FindAll(x => x.KWPoDopasowane != null).GroupBy(x => x.IdDzN).ToList().Count;
                        Console.WriteLine("progres max " + progresBar.Maximum);
                        for (int i = 0; i <= tmpListaIdDz.Count - 1; i++)
                        {
                            // Console.WriteLine("sss " + listaDopasowKW_CzyLadowac.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane);
                            // if (listaDopasowKW_CzyLadowac.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == null && listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane != null)

                            if (listaDopasowKW_CzyLadowac.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane != listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane)
                            {
                                //if ((listaDopasowKW_CzyLadowac.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == null || listaDopasowKW_CzyLadowac.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == "") && (listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == "" || listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == null))
                                //{
                                //    progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                                //    continue;
                                //}
                                if (BadanieKsiagWieczystych.SprawdzCyfreKontrolnaBool(listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane)|| listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == null || listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == "" )
                                {
                                    if(listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == null || listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane == "")
                                    {
                                        Console.WriteLine("puste badziewie");
                                         writeCommand.Parameters.Add("@IDDZ", tmpListaIdDz[i]);
                                         writeCommand.Parameters.Add("@KW", null);
                                    }
                                    else
                                    {
                                        writeCommand.Parameters.Add("@IDDZ", tmpListaIdDz[i]);
                                        writeCommand.Parameters.Add("@KW", listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane);
                                    }

                                    writeCommand.ExecuteNonQuery();
                                    writeCommand.Parameters.Clear();
                                    Console.WriteLine(i + " " + listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane + " " + listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).NrDZ);
                                    progresBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                                }
                                else
                                {
                                    var result = MessageBox.Show("BŁĘDNY NR KW: " + listaDopasowKW.Find(x => x.IdDzN.Equals(tmpListaIdDz[i])).KWPoDopasowane + " NIE ZOSTANIE PRZYPISANY DO BAZY!\n", "UWAGA!", MessageBoxButton.OKCancel);
                                    if (result == MessageBoxResult.Cancel)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        connection.Close();
                        MessageBox.Show("Zakończono przypisywanie KW do bazy.", "SUKCES!", MessageBoxButton.OK);
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
                Obliczenia.DopasujNrKWDoNowychDzialek(ref listaDopasowKW, listBoxNkr, listBoxDzialkiNowe, listBoxNrKW, "AutoPrzypiszKW");
                Console.WriteLine(listaDopasowKW.Count);

                dgNrKwZSQL.Items.Refresh();
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
            panelLogowania2.Visibility = Visibility.Visible;
            tabControl2.Visibility = Visibility.Hidden;
        }

        private void ButtonZapiszLogIHaslo2(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Login = textBoxLogin.Text;
            Properties.Settings.Default.Haslo = textBoxHaslo.Text;
            Properties.Settings.Default.Save();
            textBoxHaslo.Text = "";
            panelLogowania2.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            tabControl2.Visibility = Visibility.Visible;
        }

        private void Button_Anuluj2(object sender, RoutedEventArgs e)
        {
            panelLogowania2.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            tabControl2.Visibility = Visibility.Visible;
        }



        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowPrzypiszKW.Close();
        }

        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ItemUsunPrzypisaneJednostkiZBazy(object sender, RoutedEventArgs e)
        {
            if (czyPolaczonoZBaza)
            {
                var resultat = MessageBox.Show("Czy chcesz usunąć wcześniej przypisane KW z bazy?\n Procesu nie da się odwrócić!", "UWAGA!", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {
                    aktualizujSciezkeZPropertis();
                    using (var connection = new FbConnection(connectionString))
                    {
                        connection.Open();
                        FbCommand writeCommand = new FbCommand("UPDATE DZIALKI_N SET KW = null", connection);
                        //writeCommand.ExecuteNonQuery();
                        writeCommand.ExecuteNonQueryAsync();
                        connection.Close();
                        MessageBox.Show("KW usunięto pomyślnie.", "SUKCES!", MessageBoxButton.OK);
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
            windowPrzypiszKW.Topmost = true;
        }

        private void CheckBoxZawszeNaWierzchu_Unchecked(object sender, RoutedEventArgs e)
        {
            windowPrzypiszKW.Topmost = false;
        }

        private void DgNrKwZSQL_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            //  Console.WriteLine(((TextBox)e.EditingElement).Text + " @ " + e.EditAction + " @ " +e.Column.DisplayIndex +" displIdx @ local enum " + e.Row.GetLocalValueEnumerator().Current);
            foreach (var item in listaDopasowKW)
            {
                if (item.IdDzN.Equals(listaDopasowKW[e.Row.GetIndex()].IdDzN) && e.Column.DisplayIndex == 3)
                {
                    Console.WriteLine("przed: " + item.KWPoDopasowane + " " + item.IdDzN + " " + item.NKRn);
                    item.KWPoDopasowane = ((TextBox)e.EditingElement).Text;
                    textBlockLogInfo.Text = "";
                    Console.WriteLine("po: " + item.KWPoDopasowane + " " + item.IdDzN + " " + item.NKRn);
                }
            }
            Obliczenia.DopasujNrKWDoNowychDzialek(ref listaDopasowKW, listBoxNkr, listBoxDzialkiNowe, listBoxNrKW);
        }

        private void DgNrKwZSQL_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                listBoxNkr.Items.Refresh();
                listBoxDzialkiNowe.Items.Refresh();
                listBoxNrKW.Items.Refresh();
                dgNrKwZSQL.Items.Refresh();
            }
            catch
            {
                textBlockLogInfo.Text = "Nie udało się odświeiżyć tabeli.";
            }

        }
    }
}

