﻿using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    /// Logika interakcji dla klasy WindowKwNaMapePO.xaml
    /// </summary>
    public partial class WindowKwNaMapePO : Window
    {
        public WindowKwNaMapePO()
        {
            InitializeComponent();
            try
            {
                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
                comboBoxNkrJustyfikacja.SelectedIndex = Properties.Settings.Default.JustyfikacjNKR;
                comboBoxWartoscJustyfikacja.SelectedIndex = Properties.Settings.Default.JustyfikacjaWartosci;
                listBoxOdsuniecieTekstu.SelectedIndex = Properties.Settings.Default.OdsumieciePionoweTekstu;
                checkBoxIgnorujKropkeIPrzecinej.IsChecked = Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek;
                checkBoxPodkreslenieNKR.IsChecked = Properties.Settings.Default.CheckBoxPodkreslenieNKR;
                checkBoxPodkreslenieWARTOSCI.IsChecked = Properties.Settings.Default.CheckBoxPodkreslenieWARTOSCI;

                Console.WriteLine("ASSMBLY VERSJA: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                windowKwNaMapePO.Title += " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        public bool czyJestTakiWierszW2(List<DzNkrKWzSQLProponow> dopasowanieKWs, DzNkrKWzSQLProponow dkw)
        {
            foreach (var item in dopasowanieKWs)
            {
                if (dkw.ObrDzialka == item.ObrDzialka && dkw.NKR == item.NKR && dkw.ObrDzialka == item.ObrDzialka && dkw.ProponowKW == item.ProponowKW)
                {
                    return true;
                }
            }
            return false;
        }

        List<DzNkrKWzSQLProponow> listDzNkrKWzSQLProponows = new List<DzNkrKWzSQLProponow>();
        public void odczytajZSqlProponowaneKW()
        {
            listDzNkrKWzSQLProponows = new List<DzNkrKWzSQLProponow>();
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
                command.CommandText = "select ob.id ||'-'|| dn.idd nr_Dz, dn.id_id, jn.ijr, case when dn.kw is null then ds.kw when dn.kw = '' then ds.kw  end as OK " +
                            "from dzialki_N dn join JEDN_REJ_N jn on jn.ID_ID = dn.rjdr join JEDN_SN sn on sn.ID_jednn = dn.rjdr join dzialka ds on ds.rjdr = sn.id_jedns " +
                            "left outer join OBREBY ob on ob.id_id = dn.idobr";
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
                    if (!czyJestTakiWierszW2(listDzNkrKWzSQLProponows, new DzNkrKWzSQLProponow { ObrDzialka = dt.Rows[i][0].ToString(), IdDz = (int)dt.Rows[i][1], NKR = (int)dt.Rows[i][2], ProponowKW = dt.Rows[i][3].ToString() }))
                    {
                        listDzNkrKWzSQLProponows.Add(new DzNkrKWzSQLProponow { ObrDzialka = dt.Rows[i][0].ToString(), IdDz = (int)dt.Rows[i][1], NKR = (int)dt.Rows[i][2], ProponowKW = dt.Rows[i][3].ToString() });

                    }

                }
                try
                {
                    //dataGrid.ItemsSource = dt.AsDataView();
                    //dataGrid.Visibility = Visibility.Visible;
                    //dataGrid.Items.Refresh();
                    var myBinding = new Binding("ProponowKW");
                    columnKW.Binding = myBinding;
                    columnKW.Header = "PROPOZYCJA NR KW";
                    dgNkrFDB.ItemsSource = listDzNkrKWzSQLProponows;
                    dgNkrFDB.Visibility = Visibility.Visible;
                    dgNkrFDB.Items.Refresh();

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


        public void odczytajZSql()
        {
            listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
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
                //command.CommandText = "select obreby.id || '-' || dzialki_N.idd as NR_DZ, JEDN_REJ_N.ijr NKR, kw, dzialki_n.ww WARTOSC from DZIALKi_N left outer join OBREBY on dzialki_N.idobr = OBREBY.id_id left outer join JEDN_REJ_N on dzialki_N.rjdr = JEDN_REJ_N.id_id order by NKR";

                command.CommandText = "select obreby.id || '-' || dn.idd as NR_DZ, j1.ijr NKR, dn.kw, dn.ww WARTOSC, case when round((select round(sum(d.ww),2) from dzialki_n d where rjdr = j1.id_id) - (select sum(round(wwgsp,2)) from jedn_sn jsn join jedn_rej_n jn on jn.id_id = jsn.id_jednn where jn.id_id =j1.id_id) ,2) > 0 then '+' else '' end || round((select round(sum(d.ww),2) from dzialki_n d where rjdr = j1.id_id ) - (select sum(round(wwgsp,2)) from jedn_sn jsn join jedn_rej_n jn on jn.id_id = jsn.id_jednn where jn.id_id =j1.id_id) ,0) || '/' || round( ((select sum(round(wwgsp,2)) from jedn_sn jsn join jedn_rej_n jn on jn.id_id = jsn.id_jednn where jn.id_id =j1.id_id)*0.03),0) odchFkt_na_dopuszczalna from dzialki_n dn left outer join OBREBY on dn.idobr = OBREBY.id_id left outer join jedn_rej_n j1 on dn.rjdr = j1.id_id order by NKR";

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
                    string tmpWartosc = dt.Rows[i][3].ToString();
                    tmpWartosc = (tmpWartosc.Equals(null) || tmpWartosc == "") ? "0" : tmpWartosc;
                    decimal wartosc = Convert.ToDecimal(tmpWartosc);

                    listaDzNkrzSQL.Add(new DzialkaNkrZSQL(obrDz: dt.Rows[i][0].ToString(), nkr: Convert.ToInt32(dt.Rows[i][1]), wartosc: wartosc,kw: dt.Rows[i][2].ToString(), fkt_dop: dt.Rows[i][4].ToString()));
                }
                try
                {
                    //dataGrid.ItemsSource = dt.AsDataView();
                    //dataGrid.Visibility = Visibility.Visible;
                    //dataGrid.Items.Refresh();
                    //dgNkrFDB.ItemsSource = dt.AsDataView();

                    var myBinding = new Binding("KW");
                    columnKW.Binding = myBinding;
                    columnKW.Header = "PRZYPISANY NR KW";
                    dgNkrFDB.ItemsSource = listaDzNkrzSQL;
                    dgNkrFDB.Visibility = Visibility.Visible;
                    dgNkrFDB.Items.Refresh();

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

        public static string[] odczytZPlikuLinie(string a) //odczyt z pliku z wyjatkami niepowodzenia należy podać ścieżkę, zwraca tablicę odczytaną z pliku
        {
            string[] all = null;
            //string[] lines = null;
            try
            {
                all = System.IO.File.ReadAllLines(a, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine("Do dupy: {0}", e.Message);
                MessageBox.Show("Błąd odcztu pliku txt lub csv.\nUpewnij się, że plik, \nktóry chcesz otworzyć jest zamknięty!", "ERROR", MessageBoxButton.OK);
            }
            return all;
        }

        public static string[] pobranieWartoscZLinii(string LiniaTekstu)
        {
            char[] charSeparators = new char[] { '\t', ' ' };
            string[] wartosciZlini = LiniaTekstu.Trim().Split(charSeparators);
            return wartosciZlini;
        }

        private void otworzEDZ(object sender, RoutedEventArgs e)
        {
            listaZEDZ = new List<DzialkaEDZ>();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (!(Properties.Settings.Default.PathEDZ.Equals("") || Properties.Settings.Default.PathEDZ.Equals(null)))
            {
                dlg.InitialDirectory = Properties.Settings.Default.PathEDZ.ToString();
            }
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string[] calyOdczzytanyTextLinie = odczytZPlikuLinie(dlg.FileName);

                    List<string> listaBezPustychLinii = new List<string>();
                    for (int i = 0; i < calyOdczzytanyTextLinie.Length; i++)
                    {
                        if (calyOdczzytanyTextLinie[i].Trim() == "" || calyOdczzytanyTextLinie[i].Trim().Equals(null))
                        {
                            continue;
                        }
                        else
                        {
                            listaBezPustychLinii.Add(calyOdczzytanyTextLinie[i].Trim());
                            //listaBezPustychLinii.Add(calyOdczzytanyTextLinie[i].Trim().Replace(".", ","));

                        }
                    }

                    int przekierownik = 0;
                    for (int i = 0; i < listaBezPustychLinii.Count; i++)
                    {

                        textBlockLogInfo.Text = listaBezPustychLinii[i];
                        string[] wartosciZlini = pobranieWartoscZLinii(listaBezPustychLinii[i]);
                        List<string> listTmp = new List<string>();
                        foreach (var item in wartosciZlini)
                        {
                            if (item.Trim().Equals("") || item.Trim().Equals(null))
                            {
                                continue;
                            }
                            else
                            {
                                listTmp.Add(item.Trim());
                            }
                        }

                        if (przekierownik > 8 && listTmp.Count.Equals(1))
                        {
                            przekierownik = 0;
                        }

                        przekierownik += listTmp.Count;
                        if (przekierownik == 1)
                        {
                            listaZEDZ.Add(new DzialkaEDZ() { Nr_Dz = listTmp[0] });
                        }
                        else if (przekierownik == 7)
                        {
                            listaZEDZ[listaZEDZ.Count - 1].DzX1 = double.Parse(listTmp[0], CultureInfo.InvariantCulture);
                            listaZEDZ[listaZEDZ.Count - 1].DzY1 = double.Parse(listTmp[1], CultureInfo.InvariantCulture);
                            listaZEDZ[listaZEDZ.Count - 1].DzX2 = double.Parse(listTmp[2], CultureInfo.InvariantCulture);
                            listaZEDZ[listaZEDZ.Count - 1].DzY2 = double.Parse(listTmp[3], CultureInfo.InvariantCulture);
                        }
                        else if (przekierownik == 8)
                        {
                            listaZEDZ[listaZEDZ.Count - 1].ilePktow = int.Parse(listTmp[0], CultureInfo.InvariantCulture);
                        }
                        else if (przekierownik > 8)
                        {
                            listaZEDZ[listaZEDZ.Count - 1].listaWspPktu.Add(new DzialkaEDZ.WspPktu() { NR = listTmp[0], X = double.Parse(listTmp[1], CultureInfo.InvariantCulture), Y = double.Parse(listTmp[2], CultureInfo.InvariantCulture) });
                        }

                        //foreach (var item in listaZEDZ)
                        //{
                        //   // Console.WriteLine(item.);
                        //}
                    }


                    foreach (var item in listaZEDZ)
                    {
                        Console.WriteLine("{0} {1} {2}", item.Nr_Dz.ToString(), item.DzX1.ToString(), item.DzY1.ToString());
                    }

                    //foreach (var item in listaPunktów)
                    //{
                    //    Console.WriteLine(item.NazwaDz + " " + item.DzX1 + " " + item.DzY1 + " " + item.DzX2 + " " + item.DzY2 + " ile: " + item.ilePktow);
                    //    foreach (var items in listaPunktów[listaPunktów.Count - 1].listaWspPktu)
                    //    {
                    //        Console.WriteLine("NRP " + items.NR + " PX " + items.X + " PY " + items.Y);
                    //    }
                    //}

                    textBlockLogInfo.Text = "Wczytane działki: " + listaZEDZ.Count + ", z pliku " + dlg.FileName.Substring(dlg.FileName.LastIndexOf('\\') + 1);
                    //dataGrid.ItemsSource = listaZEDZ;
                    //dataGrid.Visibility = Visibility.Visible;
                    //dataGrid.Items.Refresh();

                    dgDzialkiEdz.ItemsSource = listaZEDZ;
                    dgDzialkiEdz.Visibility = Visibility.Visible;
                    dgDzialkiEdz.Items.Refresh();

                    Properties.Settings.Default.PathEDZ = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf("\\"));
                    Properties.Settings.Default.Save();

                }
                catch (Exception esa)
                {
                    var resultat = MessageBox.Show("Nieprawidłowy format ciągu wejściowego.\nPrzy eksporcie EDZ z EWMAPY zaznacz wszystko prócz opcji 'operat'.\nPrzerwać?", "ERROR", MessageBoxButton.YesNo);

                    if (resultat == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }

                    Console.WriteLine("Nieprawidłowy format ciągu wejściowego. Wybierz " + esa);

                }
            }
        }

        private void ZapiszDoPliku(object sender, RoutedEventArgs e)
        {
            if (CzyKwProponowane)
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
                                string loginfo = "";
                                int kodRodzajuNKRczyKW = 1;


                                sw.Write(Obliczenia.DopasujNkrDoDziałkiGenerujtxtDoEWM(listaZEDZ, listDzNkrKWzSQLProponows, ref loginfo, Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek, kodRodzajuNKRczyKW, Properties.Settings.Default.checkBoxBrakKW, Properties.Settings.Default.checkBoxDopiszBlad));
                                textBlockLogInfo.Text = loginfo;
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
            else if (CzyKwPrzypisane)
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
                                string loginfo = "";
                                int kodRodzajuNKRczyKW = 0;
                                if (sender.GetHashCode().Equals(ZapiszDoPlikuNKR.GetHashCode()))
                                {
                                    kodRodzajuNKRczyKW = 0;
                                }
                                else if (sender.GetHashCode().Equals(obrKW.GetHashCode()))
                                {
                                    kodRodzajuNKRczyKW = 1;
                                }


                                sw.Write(Obliczenia.DopasujNkrDoDziałkiGenerujtxtDoEWM(listaZEDZ, listaDzNkrzSQL, ref loginfo, Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek, kodRodzajuNKRczyKW, Properties.Settings.Default.checkBoxBrakKW, Properties.Settings.Default.checkBoxDopiszBlad, listBoxOdsuniecieTekstu.SelectedIndex / 2, checkBoxPodkreslenieNKR.IsChecked == true ?  comboBoxNkrJustyfikacja.SelectedIndex + 16 : comboBoxNkrJustyfikacja.SelectedIndex));
                                textBlockLogInfo.Text = loginfo;
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
        }
        bool CzyKwPrzypisane = false;
        bool CzyKwProponowane = false;
        
        private void ZapiszDoPlikuWartosci_Click(object sender, RoutedEventArgs e)
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
                            string loginfo = "";

                            sw.Write(Obliczenia.DopasujWartosciDlaNowychDzialek(listaZEDZ, listaDzNkrzSQL, ref loginfo, Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek, Properties.Settings.Default.checkBoxBrakKW, listBoxOdsuniecieTekstu.SelectedIndex / 2, checkBoxPodkreslenieWARTOSCI.IsChecked == true ? comboBoxWartoscJustyfikacja.SelectedIndex + 16 : comboBoxWartoscJustyfikacja.SelectedIndex));
                            textBlockLogInfo.Text = loginfo;
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

        private void ZapiszDoPliku_FKT_Dop_Click(object sender, RoutedEventArgs e)
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
                            string loginfo = "";

                            sw.Write(Obliczenia.DopasujWartosciDlaNowychDzialek(listaZEDZ, listaDzNkrzSQL, ref loginfo, Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek, Properties.Settings.Default.checkBoxBrakKW, listBoxOdsuniecieTekstu.SelectedIndex / 2, checkBoxPodkreslenieWARTOSCI.IsChecked == true ? comboBoxWartoscJustyfikacja.SelectedIndex + 16 : comboBoxWartoscJustyfikacja.SelectedIndex,true));
                            textBlockLogInfo.Text = loginfo;
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
                    itemPolaczZBaza.Background = Brushes.Transparent;
                    itemPolaczZBazaProponowaneKW.Background = Brushes.Transparent;
                    //czyPolaczonoZBaza = false;
                    //itemImportJednostkiSN.Background = Brushes.Transparent;
                    //itemImportJednostkiSN.Header = "Baza.fdb";

                    //listaDopasowJednos_CzyLadowac = new List<DopasowanieJednostek>();
                    //listaDopasowJednos = new List<DopasowanieJednostek>();

                    //dgNiedopJednostki.ItemsSource = null;
                    //listBoxDzialkiNowe.ItemsSource = null;
                    //listBoxNkr.ItemsSource = null;
                    //listBoxNrRej.ItemsSource = null;

                    //listBoxDzialkiNowe.Items.Refresh();
                    //listBoxNkr.Items.Refresh();
                    //listBoxNrRej.Items.Refresh();
                    //dgNiedopJednostki.Items.Refresh();
                }
                catch (Exception esa)
                {
                    var resultat = MessageBox.Show(esa.ToString() + " Przerwać?", "ERROR", MessageBoxButton.YesNo);

                    if (resultat == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }

                }
            }
        }


        private void PolaczZBaza(object sender, RoutedEventArgs e)
        {
            itemPolaczZBazaProponowaneKW.Background = Brushes.Transparent;
            CzyKwProponowane = false;
            CzyKwPrzypisane = false;
            try
            {
                logBledowKW.Visibility = Visibility.Hidden;
                odczytajZSql();
                itemPolaczZBaza.Background = Brushes.LightSeaGreen;
                itemPolaczenieZbaza.Background = Brushes.LightSeaGreen;
                itemPolaczenieZbaza.Header = "Połączono";
                StringBuilder stringBuilder = new StringBuilder();
                textBlockLogInfo.Text = "Połączono z bazą FDB. Ilość wczytanych linii: " + dt.Rows.Count + ".";
                // itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
                foreach (var item in listaDzNkrzSQL)
                {
                    if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolna(item.KW, item.ObrDzialka).Equals(""))
                    {
                        stringBuilder.AppendLine(BadanieKsiagWieczystych.SprawdzCyfreKontrolna(item.KW, item.ObrDzialka));
                        logBledowKW.Visibility = Visibility.Visible;
                    }
                }
                stringBuilder.AppendLine("----------------------------------KONIEC----------------------------------");
                textBlockBledy.Text = stringBuilder.ToString();


                CzyKwPrzypisane = true;
            }
            catch (Exception ex)
            {
                itemPolaczZBaza.Background = Brushes.Red;
                itemPolaczenieZbaza.Header = "Połącz z bazą";
                textBlockLogInfo.Text = "Problem z połączeniem z bazą FDB " + ex.Message;
                itemPolaczenieZbaza.Background = Brushes.Red;
                labelEdz.Visibility = Visibility.Hidden;
                przejdzDoOknaLogowania(ex.Message);
            }

        }

        private void PolaczZBazaProponowanych(object sender, RoutedEventArgs e)
        {
            itemPolaczZBaza.Background = Brushes.Transparent;
            CzyKwProponowane = false;
            CzyKwPrzypisane = false;
            try
            {
                logBledowKW.Visibility = Visibility.Hidden;
                odczytajZSqlProponowaneKW();
                itemPolaczZBazaProponowaneKW.Background = Brushes.LightSeaGreen;
                StringBuilder stringBuilder = new StringBuilder();
                textBlockLogInfo.Text = "Połączono z bazą FDB.";
                //itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
                foreach (var item in listDzNkrKWzSQLProponows)
                {
                    if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolna(item.ProponowKW, item.ObrDzialka).Equals(""))
                    {
                        stringBuilder.AppendLine(BadanieKsiagWieczystych.SprawdzCyfreKontrolna(item.ProponowKW, item.ProponowKW));
                        logBledowKW.Visibility = Visibility.Visible;
                    }
                }
                stringBuilder.AppendLine("----------------------------------KONIEC----------------------------------");
                textBlockBledy.Text = stringBuilder.ToString();

                CzyKwProponowane = true;
                itemPolaczenieZbaza.Background = Brushes.LightSeaGreen;
                itemPolaczenieZbaza.Header = "Połączono";
            }
            catch (Exception ex)
            {

                itemPolaczenieZbaza.Background = Brushes.Red;
                itemPolaczZBazaProponowaneKW.Background = Brushes.Red;
                itemPolaczenieZbaza.Header = "Połącz z bazą";
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

        void przejdzDoUstawLoginIHaslo()
        {
            textBoxLogin.Text = Properties.Settings.Default.Login;
            textBoxHaslo.Password = Properties.Settings.Default.Haslo;
            panelLogowania.Visibility = Visibility.Visible;
            dgDzialkiEdz.Visibility = Visibility.Hidden;
            tekstyTytuly.Visibility = Visibility.Hidden;
            dgNkrFDB.Visibility = Visibility.Hidden;

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
            dgDzialkiEdz.Visibility = Visibility.Visible;
            tekstyTytuly.Visibility = Visibility.Visible;
            dgNkrFDB.Visibility = Visibility.Visible;
            labelEdz.Visibility = Visibility;
        }

        private void Button_Anuluj(object sender, RoutedEventArgs e)
        {
            panelLogowania.Visibility = Visibility.Hidden;
            //dataGrid.Visibility = Visibility.Visible;
            dgDzialkiEdz.Visibility = Visibility.Visible;
            tekstyTytuly.Visibility = Visibility.Visible;
            dgNkrFDB.Visibility = Visibility.Visible;
        }

        private void CheckBoxIgnorujKropkeIPrzecinej_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek = (bool)checkBoxIgnorujKropkeIPrzecinej.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void CheckBoxIgnorujKropkeIPrzecinej_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek = (bool)checkBoxIgnorujKropkeIPrzecinej.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void ZapiszUstawienia_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxDopiszBlad = (bool)checkDopiszBlad.IsChecked;
            Properties.Settings.Default.checkBoxBrakKW = (bool)checkWypiszBrakKW.IsChecked;
            Properties.Settings.Default.JustyfikacjNKR = comboBoxNkrJustyfikacja.SelectedIndex;
            Properties.Settings.Default.JustyfikacjaWartosci = comboBoxWartoscJustyfikacja.SelectedIndex;
            Properties.Settings.Default.OdsumieciePionoweTekstu = listBoxOdsuniecieTekstu.SelectedIndex;
            Properties.Settings.Default.CheckBoxPodkreslenieNKR = (bool)checkBoxPodkreslenieNKR.IsChecked;
            Properties.Settings.Default.CheckBoxPodkreslenieWARTOSCI = (bool)checkBoxPodkreslenieWARTOSCI.IsChecked; 

            Properties.Settings.Default.Save();

            panelOpcje.Visibility = Visibility.Hidden;
        }

        private void MenuItem_Opcje(object sender, RoutedEventArgs e)
        {
            checkDopiszBlad.IsChecked = Properties.Settings.Default.checkBoxDopiszBlad;
            checkWypiszBrakKW.IsChecked = Properties.Settings.Default.checkBoxBrakKW;
            panelOpcje.Visibility = Visibility.Visible; // zrobić tak żeby te checkboxy dzialaly
        }

        private void CheckWypiszBrakKW_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxBrakKW = (bool)checkWypiszBrakKW.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void CheckWypiszBrakKW_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxBrakKW = (bool)checkWypiszBrakKW.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void CheckDopiszBlad_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxDopiszBlad = (bool)checkDopiszBlad.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void CheckDopiszBlad_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.checkBoxDopiszBlad = (bool)checkDopiszBlad.IsChecked;
            Properties.Settings.Default.Save();
        }


        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowKwNaMapePO.Close();

        }

        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CheckBoxZawszeNaWierzchu_Checked(object sender, RoutedEventArgs e)
        {
            windowKwNaMapePO.Topmost = true;
        }

        private void CheckBoxZawszeNaWierzchu_Unchecked(object sender, RoutedEventArgs e)
        {
            windowKwNaMapePO.Topmost = false;
        }


        private delegate void TextLogInfoDelegate();

        public void SetLogInfoCopy()
        {
            Thread.Sleep(1000);
            textBlockLogInfo.Text = tmpString;
        }

        string tmpString;

        private void KopiujDoSchowka_MouseDown(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(textBlockLogInfo.Text);
             tmpString = textBlockLogInfo.Text;
            textBlockLogInfo.Text = "Skopiowano do schowka";
          //  Thread.Sleep(1000);
           
            textBlockLogInfo.Dispatcher.BeginInvoke(new TextLogInfoDelegate(SetLogInfoCopy), DispatcherPriority.Background);

        }

 
    }
}
