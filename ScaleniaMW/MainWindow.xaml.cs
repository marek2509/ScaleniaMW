using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Punkt> listaPunktów = new List<Punkt>();
        List<DzialkaNkrZSQL> listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
        //  public static FbConnection connection;
        //FbCommand command;
        //FbTransaction transaction;
        //FbDataAdapter adapter;
        public static string connectionString;

        public static void aktualizujSciezkeZPropertis()
        {
            connectionString = @"User="+ Properties.Settings.Default.Login +";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=3050;Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        //public void polacz()
        //{
        //    using (var conn = new FbConnection(connectionString)) {
        //        connection.Open();
        //    }
        //}

        public void odczytajCos()
        {
            listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
            aktualizujSciezkeZPropertis();
            Console.WriteLine(connectionString);
            using (var connection = new FbConnection(connectionString))
            {
                connection.Open();
                DataTable dt = null;
                FbCommand command = new FbCommand();

                FbTransaction transaction = connection.BeginTransaction();

                command.Connection = connection;
                command.Transaction = transaction;
                command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case WHEN JEDN_REJ.nkr is null then obreby.id * 1000 + JEDN_REJ.grp else JEDN_REJ.nkr end as NKR_Z_GRUPAMI from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ on dzialka.rjdr = JEDN_REJ.id_id order by NKR_Z_GRUPAMI";

                FbDataAdapter adapter = new FbDataAdapter(command);
                dt = new DataTable();

                adapter.Fill(dt);
                //dataGrid.ItemsSource = dt.DefaultView;
                foreach (var item in dt.Columns)
                {

                    Console.Write(item + " << ");
                }
                Console.WriteLine();
                Console.WriteLine("row count:" + dt.Rows.Count);
                Console.WriteLine("column count:" + dt.Columns.Count);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    
                      listaDzNkrzSQL.Add(new DzialkaNkrZSQL(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString()));
                }
                Console.WriteLine("LISTA DZIALKA NKR");
                //foreach (var item in listaDzNkrzSQL)
                //{
                //    Console.WriteLine(item.Obr_Dzialka + " " + item.NKR);
                //}
                  
                 


                    //Console.WriteLine(i+ " " + dt.Rows[i][0]); // "" + dt.Rows[i][1] + " " );

               
                connection.Close();
            }

        }


        public MainWindow()
        {

            InitializeComponent();

            try
            {

                textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
            }
            catch (Exception e)
            {
                Console.WriteLine(e + " problem z oknem");
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
            listaPunktów = new List<Punkt>();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
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
                            listaPunktów.Add(new Punkt() { NazwaDz = listTmp[0] });
                        }
                        else if (przekierownik == 7)
                        {


                            listaPunktów[listaPunktów.Count - 1].DzX1 = double.Parse(listTmp[0], CultureInfo.InvariantCulture);
                            listaPunktów[listaPunktów.Count - 1].DzY1 = double.Parse(listTmp[1], CultureInfo.InvariantCulture);
                            listaPunktów[listaPunktów.Count - 1].DzX2 = double.Parse(listTmp[2], CultureInfo.InvariantCulture);
                            listaPunktów[listaPunktów.Count - 1].DzY2 = double.Parse(listTmp[3], CultureInfo.InvariantCulture);
                        }
                        else if (przekierownik == 8)
                        {
                            listaPunktów[listaPunktów.Count - 1].ilePktow = int.Parse(listTmp[0], CultureInfo.InvariantCulture);
                        }
                        else if (przekierownik > 8)
                        {
                            listaPunktów[listaPunktów.Count - 1].listaWspPktu.Add(new Punkt.WspPktu() { NR = listTmp[0], X = double.Parse(listTmp[1], CultureInfo.InvariantCulture), Y = double.Parse(listTmp[2], CultureInfo.InvariantCulture) });
                        }
                        foreach (var item in listaPunktów)
                        {
                           // Console.WriteLine(item.);
                        }
                    }



                    //foreach (var item in listaPunktów)
                    //{
                    //    Console.WriteLine(item.NazwaDz + " " + item.DzX1 + " " + item.DzY1 + " " + item.DzX2 + " " + item.DzY2 + " ile: " + item.ilePktow);
                    //    foreach (var items in listaPunktów[listaPunktów.Count - 1].listaWspPktu)
                    //    {
                    //        Console.WriteLine("NRP " + items.NR + " PX " + items.X + " PY " + items.Y);
                    //    }
                    //}
                    textBlockLogInfo.Text = "Wczytano EDZ.";
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

        private void ZapiszDoPliku(object sender, RoutedEventArgs e)
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
                            //StringBuilder sb = new StringBuilder();
                            //foreach (var item in listaPunktów)
                            //{
                            //    sb.AppendLine(item.NazwaDz + "\t" + item.podajeKatUstawienia());
                            //}
                            string loginfo = "";
                            sw.Write(Obliczenia.DopasujNkrDoDziałkiGenerujtxtDoEWM(listaPunktów,listaDzNkrzSQL, ref loginfo));
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
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    ustawProperties(dlg.FileName);
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

        private void PolaczZBaza(object sender, RoutedEventArgs e)
        {
            try
            {
                odczytajCos();
                itemPolaczZBaza.Background = Brushes.LightSeaGreen;
                textBlockLogInfo.Text = "Połączono z bazą FDB";
            }
            catch(Exception ex)
            {
                itemPolaczZBaza.Background = Brushes.Red;
                
                textBlockLogInfo.Text = "Problem z połączeniem z bazą FDB " + ex;

            }
       
        }

        private void UstawLoginIHaslo(object sender, RoutedEventArgs e)
        {
            textBoxLogin.Text = Properties.Settings.Default.Login;
            panelLogowania.Visibility = Visibility.Visible;
        }

        private void ButtonZapiszLogIHaslo(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Login = textBoxLogin.Text;
            Properties.Settings.Default.Haslo = textBoxHaslo.Text;
            Properties.Settings.Default.Save();
            textBoxHaslo.Text = "";
            panelLogowania.Visibility = Visibility.Hidden;
        }

        private void Button_Anuluj(object sender, RoutedEventArgs e)
        {
            panelLogowania.Visibility = Visibility.Hidden;
        }
        
    }
}
