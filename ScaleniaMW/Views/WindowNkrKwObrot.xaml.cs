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
    /// Logika interakcji dla klasy WindowNkrKwObrot.xaml
    /// </summary>
    public partial class WindowNkrKwObrot : Window
    {
        public WindowNkrKwObrot()
        {
            InitializeComponent();
            try
            {
                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
                checkBoxIgnorujKropkeIPrzecinej.IsChecked = Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek;
                Console.WriteLine("ASSMBLY VERSJA: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                windowNkrKwObrot.Title += " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
                //command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, JEDN_REJ.nkr NKR_Z_GRUPAMI, kw, JEDN_REJ.idgrp from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ on dzialka.rjdr = JEDN_REJ.id_id order by NKR_Z_GRUPAMI";
               // command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case when (select first 1 nkr from jedn_rej where idgrp = j1.idgrp and idgrp>0  and nkr > 0 order by nkr asc) > 0 then (select first 1 nkr from jedn_rej where idgrp = j1.idgrp and idgrp>0  and nkr > 0 order by nkr asc) else j1.nkr end NKR_Zgupowany, kw, j1.idgrp from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ j1 on dzialka.rjdr = j1.id_id order by NKR_Zgupowany";
                command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case when idgrp > 0 then (select nkr from jedn_rej where id_id = j1.idgrp) else j1.nkr end NKR_Zgupowany, kw, j1.idgrp from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ j1 on dzialka.rjdr = j1.id_id order by NKR_Zgupowany";

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

                    listaDzNkrzSQL.Add(new DzialkaNkrZSQL(
                        dt.Rows[i][0].ToString(),
                        Convert.ToInt32(dt.Rows[i][1].Equals(System.DBNull.Value) ? null : dt.Rows[i][1]), 
                        dt.Rows[i][2].ToString(),
                         Convert.ToInt32(dt.Rows[i][3].Equals(System.DBNull.Value) ? 0 : dt.Rows[i][3])));
                }


                //while (listaDzNkrzSQL.Exists(x => x.NKR == 0))
                //{
                //    DzialkaNkrZSQL tmp = new DzialkaNkrZSQL();
                //    tmp = listaDzNkrzSQL.Find(x => x.NKR == 0);
                //    DzialkaNkrZSQL tmp2 = new DzialkaNkrZSQL();

                //    tmp2 = listaDzNkrzSQL.Find(x => x._grp == tmp._grp && x.NKR != 0);
                //    Console.WriteLine();
                //    Console.WriteLine("tmp: " + tmp._grp + " " +tmp.NKR + tmp.ObrDzialka );
                //    Console.WriteLine("tmp2: " + tmp2._grp + " " +tmp2.NKR + tmp2.ObrDzialka );
                //    tmp.NKR = tmp2.NKR;
                //}
                


                try
                {

                    dgNkrFDB.ItemsSource = listaDzNkrzSQL;
                    dgNkrFDB.Visibility = Visibility.Visible;
                    dgNkrFDB.Items.Refresh();

                    Console.WriteLine("ustawiam SOURCE");
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp);
                }


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
            stackBledyKW.Visibility = Visibility.Hidden;
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

                  //x  tabControl.SelectedIndex = 0;
                }
                catch (Exception esa)
                {

                    var resultat = MessageBox.Show("Nieprawidłowy format ciągu wejściowego.\nPrzy eksporcie EDZ z EWMAPY zaznacz wszystko prócz opcji 'operat'.\nPrzerwać?", "ERROR", MessageBoxButton.YesNo);

                    if (resultat == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }

                    Console.WriteLine(esa + "Błędny format importu działek");
                }
            }
        }

        private void ZapiszDoPliku(object sender, RoutedEventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = ".txt";
            svd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (svd.ShowDialog() == true)
            {
                using (Stream s = File.Open(svd.FileName, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                    try
                    {
                        try
                        {
                            string loginfo = "";
                            int kodRodzajuNKRczyKW = 0;
                            if (sender.GetHashCode().Equals(obrNKR.GetHashCode()))
                            {
                                kodRodzajuNKRczyKW = 0;
                            }
                            else if (sender.GetHashCode().Equals(obrKW.GetHashCode()))
                            {
                                kodRodzajuNKRczyKW = 1;
                            }

                            sw.Write(Obliczenia.DopasujNkrDoDziałkiGenerujtxtDoEWM(listaZEDZ, listaDzNkrzSQL, ref loginfo, Properties.Settings.Default.checkBoxignorujKropkeIPrzecinek, kodRodzajuNKRczyKW, Properties.Settings.Default.checkBoxBrakKW, Properties.Settings.Default.checkBoxDopiszBlad,0));
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
            try
            {
                logBledowKW.Visibility = Visibility.Hidden;
                odczytajZSql();
                itemPolaczZBaza.Background = Brushes.SeaGreen;
                StringBuilder stringBuilder = new StringBuilder();
                textBlockLogInfo.Text = "Połączono z bazą FDB. Ilość wczytanych linii: " + dt.Rows.Count + ".";
                itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);
                foreach (var item in listaDzNkrzSQL)
                {
                    if (!BadanieKsiagWieczystych.SprawdzCyfreKontrolna(item.KW, item.ObrDzialka).Equals(""))
                    {
                        stringBuilder.AppendLine(BadanieKsiagWieczystych.SprawdzCyfreKontrolna(item.KW, item.ObrDzialka) + " przypisaną do działki nr: " + item.ObrDzialka);
                        logBledowKW.Visibility = Visibility.Visible;
                        logBledowKW.Background = Brushes.Red;
                    }
                }
                stringBuilder.AppendLine("----------------------------------KONIEC----------------------------------");

                textBlockBledy.Text = stringBuilder.ToString();
               //x tabControl.SelectedIndex = 1;
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

        void przejdzDoUstawLoginIHaslo()
        {
            WindowLogowanie windowLogowanie = new WindowLogowanie();
            windowLogowanie.Show();
        }

        private void UstawLoginIHaslo(object sender, RoutedEventArgs e)
        {
            przejdzDoUstawLoginIHaslo();
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
            Properties.Settings.Default.Save();
            panelOpcje.Visibility = Visibility.Hidden;
        }

        private void MenuItem_Opcje(object sender, RoutedEventArgs e)
        {
            checkDopiszBlad.IsChecked = Properties.Settings.Default.checkBoxDopiszBlad;
            checkWypiszBrakKW.IsChecked = Properties.Settings.Default.checkBoxBrakKW;
            panelOpcje.Visibility = Visibility.Visible;
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
            windowNkrKwObrot.Close();
        }

        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CheckBoxZawszeNaWierzchu_Checked(object sender, RoutedEventArgs e)
        {
             windowNkrKwObrot.Topmost = true;
        }

        private void CheckBoxZawszeNaWierzchu_Unchecked(object sender, RoutedEventArgs e)
        {
            windowNkrKwObrot.Topmost = false;
        }

        private void LogBledowKW_Click(object sender, RoutedEventArgs e)
        {
            stackBledyKW.Visibility = Visibility.Visible;
            dgDzialkiEdz.Visibility = Visibility.Hidden;
        }

        private void ZamknijTextBlockBledy_Click(object sender, RoutedEventArgs e)
        {
            dgDzialkiEdz.Visibility = Visibility.Visible;
            stackBledyKW.Visibility = Visibility.Hidden;
            logBledowKW.Background = Brushes.Transparent;
        }

        private void ItemPrzypiszLpWUwg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BazaFB.Execute_SQL(Constants.SQLPaginationUnit);
                MessageBox.Show("Sukces!");
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }

        private void ItemPrzypiszLpWUwgGrupy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BazaFB.Execute_SQL(Constants.SQLPaginationUnitWithGroupingToObreb);
                MessageBox.Show("Sukces!");
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }

        private void ItemPrzypiszLpWUwgGrupyDoGminy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BazaFB.Execute_SQL(Constants.SQLPaginationUnitWithGroupingToGmina);
                MessageBox.Show("Sukces!");
            }
            catch (Exception c)
            {
                MessageBox.Show(c.Message);
            }
        }
    }
}
