using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy EdycjaDokumentow.xaml
    /// </summary>
    public partial class WindowEdycjaDokumentow : Window
    {
        public WindowEdycjaDokumentow()
        {
            InitializeComponent();
            textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
            // odczytUstawien();
        }

        string usunOd = "kontury";
        string usunDo = "bilans";
        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowEdycjaDokumentow.Close();
        }
        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public static List<string> odczytZPlikuLinie(string a) //odczyt z pliku z wyjatkami niepowodzenia należy podać ścieżkę, zwraca tablicę odczytaną z pliku
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
            return all.ToList();
        }

        List<string> calyOdczzytanyTextLinie = new List<string>();
        private void Otworz_RTF_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    calyOdczzytanyTextLinie = odczytZPlikuLinie(dlg.FileName);

                    //  calyOdczzytanyTextLinie.ForEach(x => Console.WriteLine(x));
                    if (checkBoxUsunKontury.IsChecked == true)
                    {
                        bool czyUsuwaclinie = false;
                        for (int i = 0; i < calyOdczzytanyTextLinie.Count; i++)
                        {
                            if (calyOdczzytanyTextLinie[i].ToLower().Contains(usunOd)) czyUsuwaclinie = true;
                            if (calyOdczzytanyTextLinie[i].ToLower().Contains(usunDo)) czyUsuwaclinie = false;

                            if (czyUsuwaclinie)
                            {
                                calyOdczzytanyTextLinie.RemoveAt(i - 1);
                                i--;
                            }

                            if (calyOdczzytanyTextLinie[i].ToLower().Contains("obr")) czyUsuwaclinie = false;
                        }
                        calyOdczzytanyTextLinie.ForEach(x => sb.AppendLine(x));


                    }
                    else
                    {
                        StringBuilder sbPuste = new StringBuilder();
                        for (int i = 0; i < calyOdczzytanyTextLinie.Count; i++)
                        {
                            try
                            {
                                sb.AppendLine(UsunWartZSzacunku(calyOdczzytanyTextLinie[i], ref sbPuste));
                            }
                            catch
                            {
                                sbPuste.AppendLine("Błędny format linii:" + calyOdczzytanyTextLinie[i]);
                            }
                        }
                        textBoxPuste.Text = sbPuste.ToString();
                    }


                    var resultat = MessageBox.Show("Wczytano.\nZapisz plik.", "Wczytano", MessageBoxButton.OK);
                }
                catch (Exception esa)
                {
                    Console.WriteLine("Nieprawidłowy format ciągu wejściowego. Wybierz ");
                }

                if (checkBoxUsunKontury.IsChecked == true)
                {
                    zapisDoPliku(sb.ToString());
                }
                else
                {
                    zapisDoPliku(sb.ToString(), ".txt");
                }
            }
        }

        bool SprawdzCzySzacunekTRUECzyKlasyfikacjaFALSE(string liniaZTektowki)
        {
            liniaZTektowki = liniaZTektowki.Trim();

            int ostatniMyslnik = liniaZTektowki.LastIndexOf('-');

            if (liniaZTektowki.LastIndexOf('/') < liniaZTektowki.IndexOf(' '))
            {
                return false;
            }
            else
            if (ostatniMyslnik > liniaZTektowki.LastIndexOf('/'))
            {
                if (ostatniMyslnik < liniaZTektowki.Length - 1)
                {
                    if (char.IsNumber(liniaZTektowki[ostatniMyslnik + 1]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        string UsunWartZSzacunku(string liniaZTektowki, ref StringBuilder sbPuste)
        {

            if (liniaZTektowki.Contains("pusty") || liniaZTektowki.Trim() == "")
            {
                sbPuste.AppendLine(liniaZTektowki);
                return (liniaZTektowki);
            }
            int ostatniMyslnik = liniaZTektowki.LastIndexOf('-');
            int dlTekstu = liniaZTektowki.Length;


            if (SprawdzCzySzacunekTRUECzyKlasyfikacjaFALSE(liniaZTektowki))
            {
                Console.WriteLine(liniaZTektowki + " szacunek TRUE");
                int ileZnakowUsunac = liniaZTektowki.IndexOf(' ', ostatniMyslnik) - ostatniMyslnik;
                liniaZTektowki = liniaZTektowki.Remove(ostatniMyslnik, ileZnakowUsunac);
            }

            if (liniaZTektowki.IndexOf('/', liniaZTektowki.IndexOf(' ')) > liniaZTektowki.IndexOf(' '))
            {
                while (true)
                {
                    int pierwsza_przerwa = liniaZTektowki.IndexOf(' ');
                    int zamiana_Myslnik = liniaZTektowki.IndexOf('-', liniaZTektowki.LastIndexOf('/'));
                    Console.WriteLine(liniaZTektowki + " pp " + pierwsza_przerwa + " om " + zamiana_Myslnik);
                    if (zamiana_Myslnik > pierwsza_przerwa)
                    {
                        liniaZTektowki = liniaZTektowki.Remove(zamiana_Myslnik, 1);
                        liniaZTektowki = liniaZTektowki.Insert(zamiana_Myslnik, "/");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    int pierwsza_przerwa = liniaZTektowki.IndexOf(' ');
                    int ostatni_Myslnik = liniaZTektowki.LastIndexOf('-');

                    if (ostatni_Myslnik > pierwsza_przerwa)
                    {
                        liniaZTektowki = liniaZTektowki.Remove(ostatni_Myslnik, 1);
                        liniaZTektowki = liniaZTektowki.Insert(ostatni_Myslnik, "/");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return liniaZTektowki;
        }


        void zapisDoPliku(string tekstDoZapisu, string format = ".rtf")
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = format;
            svd.Filter = "All files (*.*)|*.*";
            if (svd.ShowDialog() == true)
            {
                using (Stream s = File.Open(svd.FileName, FileMode.Create))
                //  using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                    try
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder();

                            sw.Write(tekstDoZapisu);
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

        private void ButtonPobierzWzorzecWykWydzEkwiw_Click(object sender, RoutedEventArgs e)
        {
            zapisDoPliku(Properties.Resources.WzorzecWykazEkwiwalentów.ToString());
        }

        private void ButtonPobierzWzorzecRejestrPo_Click(object sender, RoutedEventArgs e)
        {
            zapisDoPliku(Properties.Resources.WzorzecRejestrPoScaleniu.ToString());
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
                    Console.WriteLine(esa + "Błędny format importu działek");
                }
            }
        }
        public static string connectionString;
        public static void aktualizujSciezkeZPropertis()
        {
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=3050;Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        public DataTable odczytajZSql(string zapytanieSQL)
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
                    command.CommandText = zapytanieSQL;
                    FbDataAdapter adapter = new FbDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    connection.Close();
                    return dt;
                }
            }
            catch
            {
                return null;
            }
        }



        private void PolaczZBaza(object sender, RoutedEventArgs e)
        {
            try
            {
                itemPolaczZBaza.Background = Brushes.SeaGreen;
                itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);


                DataTable obreby = odczytajZSql(Constants.SQL_Obreby);
                for (int i = 0; i < obreby.Rows.Count; i++)
                {
                    ListaObrebow.DodajObreb(Convert.ToInt32(obreby.Rows[i][0]), Convert.ToInt32(obreby.Rows[i][1]), obreby.Rows[i][2].ToString());
                }

                //jn.id_id, jn.ijr, jn.nkr, jn.ODCHT, jn.zgoda, jn.uwg, jn.id_obr
                DataTable jednostkaRejestrowa = odczytajZSql(Constants.SQLJedn_rej_N);
                for (int i = 0; i < jednostkaRejestrowa.Rows.Count; i++)
                {
                    int idJednRejN = jednostkaRejestrowa.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][0]);
                    int ijr = jednostkaRejestrowa.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][1]);
                    int nkr = jednostkaRejestrowa.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][2]);
                    bool odcht = jednostkaRejestrowa.Rows[i][3].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][3]);
                    bool zgoda = jednostkaRejestrowa.Rows[i][4].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][4]);
                    string uwaga = jednostkaRejestrowa.Rows[i][5].ToString().Equals(DBNull.Value) ? "" : jednostkaRejestrowa.Rows[i][5].ToString();
                    int idobr = jednostkaRejestrowa.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToInt32(jednostkaRejestrowa.Rows[i][6]);
                    JednostkiRejestroweNowe.DodajJrNowa(idJednRejN, ijr, nkr, odcht, zgoda, uwaga, idobr);    // jednostkaRejestrowa.Rows[i][0]);
                }


             //   dgMy.ItemsSource = JednostkiRejestroweNowe.Jedn_REJ_N;

                DataTable wlasciciele = odczytajZSql(Constants.SQLWlascicieleAdresyUdziayIdNKRNOWY);
                for (int i = 0; i < wlasciciele.Rows.Count; i++)
                {
                    int idJednN = wlasciciele.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(wlasciciele.Rows[i][0]);
                    string udzial = wlasciciele.Rows[i][1].ToString().Equals(DBNull.Value) ? "" : wlasciciele.Rows[i][1].ToString();
                    double udzial_NR = wlasciciele.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToDouble(wlasciciele.Rows[i][2]);       
                    string nazwaWlasciciela = wlasciciele.Rows[i][3].ToString().Equals(DBNull.Value) ? "" : wlasciciele.Rows[i][3].ToString();
                    string adres = wlasciciele.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : wlasciciele.Rows[i][4].ToString();
                    int idMalzenstwa = wlasciciele.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(wlasciciele.Rows[i][5]);

                    
                    Wlasciciel wlasciciel = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == idJednN).DodajWlasciciela(wlasciciel);
                }

                Console.WriteLine("heja");
                JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.Ijr == 4103).Wlasciciele.ForEach(x => x.WypiszWKoncoli());
                Console.WriteLine("endo:");

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
            textBoxLogin.Text = Properties.Settings.Default.Login;
            textBoxHaslo.Password = Properties.Settings.Default.Haslo;
            panelLogowania.Visibility = Visibility.Visible;
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
        }

        private void Button_Anuluj(object sender, RoutedEventArgs e)
        {
            panelLogowania.Visibility = Visibility.Hidden;
        }





    }
}
