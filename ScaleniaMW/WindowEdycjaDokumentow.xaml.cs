using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

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
                    if (checkBoxUsunKontury.IsChecked == true) // usuwanie konturów z dokumentu
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
                                sb.AppendLine(UsunWartZSzacunkuIZamienNaUkosniki(calyOdczzytanyTextLinie[i], ref sbPuste, (bool)checkBoxZrobNowaKlasyfZSzac.IsChecked));
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

        string UsunWartZSzacunkuIZamienNaUkosniki(string liniaZTektowki, ref StringBuilder sbPuste, bool usunWartosc = true)
        {
            if (liniaZTektowki.Contains("pusty") || liniaZTektowki.Trim() == "")
            {
                sbPuste.AppendLine(liniaZTektowki);
                richTextBox.Text = sbPuste.ToString();
                return (liniaZTektowki);
            }

            Regex regex = new Regex(@"^[-+]?([0-9]+(\.[0-9]*)?|\.[0-9]+)$");

            var tmpSplitString = liniaZTektowki.Split(' '); // temporary elem
            List<string> splitedString = new List<string>();   // main list with true value without whitespace


            foreach (var item in tmpSplitString) // assign value and reject wrong value (whitespace)
            {
                if(item != "")
                {
                    splitedString.Add(item);
                }
            }
             
            if(splitedString.Count != 3)
            {
                sbPuste.AppendLine("ERROR: " +liniaZTektowki);
                return liniaZTektowki;
            }
            else
            {
                // sprawdzenie czy kontur posiada wartość
                string tmpLandUseContour = splitedString[1];
                var wartoscKontury = tmpLandUseContour.Remove(0, tmpLandUseContour.LastIndexOf('-') + 1);

                if (usunWartosc) // warunek dla opcji czy usuwać wartość!!!
                {                 
                    // jeśli nie posiada wartości to nie usuwaj
                    if (!regex.IsMatch(wartoscKontury))
                    {
                        sbPuste.AppendLine("Brak wartości dla konturu: " + liniaZTektowki);
                    }
                    else
                    {
                        //usunięcie wartości z konturu
                        splitedString[1] = tmpLandUseContour.Remove(tmpLandUseContour.LastIndexOf('-'));
                    }
              
                }





                // zamiana myślników na ukośniki
                Console.WriteLine(splitedString[1]);
                var tmpArray = splitedString[1].Split('/');
                StringBuilder sb = new StringBuilder();
                if (tmpArray.Length == 2)
                {
                    sb.Append(tmpArray[0]);
                    sb.Append("/");
                    sb.Append(tmpArray[1].Replace("-", "/"));
                    splitedString[1] = sb.ToString();
                }
                else if(tmpArray.Length == 1)
                {
                    splitedString[1] = tmpArray[0].Replace("-", "/");
                }
                else
                {
                    sbPuste.AppendLine("ERROR: " + liniaZTektowki);
                    return liniaZTektowki;
                }


                // merge  text line
                StringBuilder processedValue = new StringBuilder();
                processedValue.Append(splitedString[0]);
                 while(processedValue.Length < 25)
                {
                    processedValue.Append(" ");
                }

                processedValue.Append(splitedString[1]);
                while(processedValue.Length < 67 - splitedString[2].Length)
                {
                    processedValue.Append(" ");
                }
                processedValue.Append(splitedString[2]);


                //jeśli nie usuwać wartości to zrób tak żeby przed wartością był myślnik

                if (regex.IsMatch(wartoscKontury))
                {
                    sbPuste.AppendLine("Brak wartości dla konturu: " + liniaZTektowki);
                
                    if (!usunWartosc)
                    {
                        var tmpString = processedValue.ToString();
                        var startString = tmpString.Remove(tmpString.LastIndexOf('/'));
                        var endString = tmpString.Remove(0, tmpString.LastIndexOf('/')+1);
                        processedValue.Clear();
                        processedValue.Append(startString);
                        processedValue.Append("-");
                        processedValue.Append(endString);
                    }
                }

                return processedValue.ToString();
            }
        }

      
        public void zapisDoPliku(string tekstDoZapisu, string format = ".rtf")
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = format;
            svd.Filter = "All files (*.*)|*.*";
            if (svd.ShowDialog() == true)
            {
                try
                {

                    using (Stream s = File.Open(svd.FileName, FileMode.Create))
                    {


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
                catch (Exception ed)
                {
                    MessageBox.Show(ed.Message.ToString(), "ERROR", MessageBoxButton.OK);
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
                    Console.WriteLine(esa + "Błędny format importu działek");
                }
            }
        }
        public static string connectionString;
        public static void aktualizujSciezkeZPropertis()
        {
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
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
            richTextBox.Text = "";
            try
            {
                ListaObrebow.ClearData();
                JednostkiRejestroweNowe.ClearData();

                itemPolaczZBaza.Background = Brushes.SeaGreen;
                itemPolaczZBaza.Header = "Połączono z " + Properties.Settings.Default.PathFDB.Substring(Properties.Settings.Default.PathFDB.LastIndexOf('\\') + 1);

                DataTable obreby = odczytajZSql(Constants.SQL_Obreby);
                for (int i = 0; i < obreby.Rows.Count; i++)
                {
                    ListaObrebow.DodajObreb(Convert.ToInt32(obreby.Rows[i][0]), Convert.ToInt32(obreby.Rows[i][1]), obreby.Rows[i][2].ToString());
                }

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
                    JednostkiRejestroweNowe.DodajJrNowa(idJednRejN, ijr, nkr, odcht, zgoda, uwaga, idobr);
                }

                DataTable WlascicielePO = odczytajZSql(Constants.SQLWlascicieleAdresyUdziayIdNKRNOWY);
                for (int i = 0; i < WlascicielePO.Rows.Count; i++)
                {
                    int idJednN = WlascicielePO.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePO.Rows[i][0]);
                    string udzial = WlascicielePO.Rows[i][1].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][1].ToString();
                    double udzial_NR = WlascicielePO.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToDouble(WlascicielePO.Rows[i][2]);
                    string nazwaWlasciciela = WlascicielePO.Rows[i][3].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][3].ToString();
                    string adres = WlascicielePO.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][4].ToString();
                    int idMalzenstwa = WlascicielePO.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePO.Rows[i][5]);

                    Wlasciciel wlasciciel = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == idJednN).DodajWlasciciela(wlasciciel);
                }

                DataTable Jedn_SN = odczytajZSql(Constants.SQLJedn_SN);
                for (int i = 0; i < Jedn_SN.Rows.Count; i++)
                {
                    int idJednN = Jedn_SN.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][0]);
                    int idJednS = Jedn_SN.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][1]);
                    int IJR = Jedn_SN.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][2]);
                    int Idobr = Jedn_SN.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Jedn_SN.Rows[i][3]);
                    string Ud_z_Jedn = Jedn_SN.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : Jedn_SN.Rows[i][4].ToString();
                    decimal WrtPrzed = Jedn_SN.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Jedn_SN.Rows[i][5]);
                    double PowPrzed = Jedn_SN.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Jedn_SN.Rows[i][6]);

                    ZJednRejStarej zJednRej = new ZJednRejStarej(idJednS, IJR, Ud_z_Jedn, WrtPrzed, PowPrzed, Idobr);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == idJednN).DodajZJrStarej(zJednRej);
                }

                DataTable Dzialki_nowe = odczytajZSql(Constants.SQL_Dzialki_N);
                for (int i = 0; i < Dzialki_nowe.Rows.Count; i++)
                {

                    int Id_dz = Dzialki_nowe.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][0]);
                    int Id_obr = Dzialki_nowe.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][1]);
                    string NrDz = Dzialki_nowe.Rows[i][2].ToString().Equals(DBNull.Value) ? "" : Dzialki_nowe.Rows[i][2].ToString();
                    double PowDz = Dzialki_nowe.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Dzialki_nowe.Rows[i][3]);
                    int Rjdr = Dzialki_nowe.Rows[i][4].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][4]);
                    int RjdrPrzed = Dzialki_nowe.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_nowe.Rows[i][5]);
                    string KW = Dzialki_nowe.Rows[i][6].ToString().Equals(DBNull.Value) ? "" : Dzialki_nowe.Rows[i][6].ToString();
                    decimal Wartosc = Dzialki_nowe.Rows[i][7].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Dzialki_nowe.Rows[i][7]);

                    Dzialka_N dzialka = new Dzialka_N(Id_dz, Id_obr, NrDz, PowDz, Rjdr, RjdrPrzed, KW, Wartosc);
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == Rjdr).DodajDzialke(dzialka);

                }

                DataTable Dzialki_przed = odczytajZSql(Constants.SQL_Dzialka);
                List<Dzialka> DzialkaStaraTMP = new List<Dzialka>();
                for (int i = 0; i < Dzialki_przed.Rows.Count; i++)
                {

                    int Id_dz = Dzialki_przed.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][0]);
                    int Id_obr = Dzialki_przed.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][1]);
                    string NrDz = Dzialki_przed.Rows[i][2].ToString().Equals(DBNull.Value) ? "" : Dzialki_przed.Rows[i][2].ToString();
                    double PowDz = Dzialki_przed.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Dzialki_przed.Rows[i][3]);
                    int Rjdr = Dzialki_przed.Rows[i][4].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][4]);
                    string KW = Dzialki_przed.Rows[i][5].ToString().Equals(DBNull.Value) ? "" : Dzialki_przed.Rows[i][5].ToString();
                    decimal Wartosc = Dzialki_przed.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Dzialki_przed.Rows[i][6]);

                    DzialkaStaraTMP.Add(new Dzialka(Id_dz, Id_obr, NrDz, PowDz, Rjdr, KW, Wartosc));
                }

                foreach (var JN in JednostkiRejestroweNowe.Jedn_REJ_N)
                {
                    foreach (var JRst in JN.zJednRejStarej)
                    {
                        foreach (var dzialkaPrzed in DzialkaStaraTMP.FindAll(x => x.Rjdr == JRst.Id_Jedns))
                        {
                            JRst.DodajDzialkePrzed(dzialkaPrzed);
                        }
                    }
                }

                DataTable WlascicielePrzed = odczytajZSql(Constants.SQL_WlascicielAdresyUdzialyIdNkrStary);
                List<WlascicielStanPrzed> WlascicielePrzedTMP = new List<WlascicielStanPrzed>();
                for (int i = 0; i < WlascicielePrzed.Rows.Count; i++)
                {
                    int idJednS = WlascicielePrzed.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePrzed.Rows[i][0]);
                    string udzial = WlascicielePrzed.Rows[i][1].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][1].ToString();
                    double udzial_NR = WlascicielePrzed.Rows[i][2].Equals(DBNull.Value) ? 0 : Convert.ToDouble(WlascicielePrzed.Rows[i][2]);
                    string nazwaWlasciciela = WlascicielePrzed.Rows[i][3].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][3].ToString();
                    string adres = WlascicielePrzed.Rows[i][4].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][4].ToString();
                    int idMalzenstwa = WlascicielePrzed.Rows[i][5].Equals(DBNull.Value) ? 0 : Convert.ToInt32(WlascicielePrzed.Rows[i][5]);

                    // Wlasciciel wlascicielPrzed = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa);
                    WlascicielePrzedTMP.Add(new WlascicielStanPrzed(idJednS, udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa));
                }

                foreach (var JN in JednostkiRejestroweNowe.Jedn_REJ_N)
                {
                    foreach (var JRst in JN.zJednRejStarej)
                    {
                        foreach (var wlascicielPrzed in WlascicielePrzedTMP.FindAll(x => x.IdJednPrzed == JRst.Id_Jedns))
                        {
                            JRst.DodajWlascicielaWStaniePrzed(wlascicielPrzed);
                        }
                    }
                }
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

        /*
        public string GenerujWWE()
        {

            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PoczatekWykazyWydzEkwiwalentow);
            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PodzialSekcjiNaStronieNieparzystej);


            foreach (var JednoskaRejNowa in JednostkiRejestroweNowe.Jedn_REJ_N)
            {
                dokHTML.Append( HtmlDokumentWykazWydzEkwiwalentow.GenerujWykazWE(JednoskaRejNowa));
            }


            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_ZakonczenieWykazuWydzEkwiw);

            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.Text +="W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR OBREBU\n");
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.Nkr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR JEDNOSTKI REJESTROWEJ\n");
            //JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.AppendText("W jednostce: " + x.IjrPo.ToString() + " brakuje numeru obrębu"));

            
            return dokHTML.ToString();
        }*/

        public string GenerujWWE(List<JR_Nowa> jR_Nowa)
        {
            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PoczatekWykazyWydzEkwiwalentow);
            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_PodzialSekcjiNaStronieNieparzystej);

            foreach (var JednoskaRejNowa in jR_Nowa)
            {
                dokHTML.Append(HtmlDokumentWykazWydzEkwiwalentow.GenerujWykazWE(JednoskaRejNowa));
            }
            dokHTML.AppendLine(HtmlDokumentWykazWydzEkwiwalentow.HTML_ZakonczenieWykazuWydzEkwiw);
            //JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.AppendText("W jednostce: " + x.IjrPo.ToString() + " brakuje numeru obrębu"));
            return dokHTML.ToString();
        }


        private void Generuj_Wykaz_wydz_ekw_Click(object sender, RoutedEventArgs e)
        {

            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR OBREBU\n");
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.Nkr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR JEDNOSTKI REJESTROWEJ\n");
            //List<List<JR_Nowa>> jR_s = new List<List<JR_Nowa>>();


            //int naIlePlikowDzielimy = (int)Math.Ceiling(JednostkiRejestroweNowe.Jedn_REJ_N.Count / 100d);

            //if (naIlePlikowDzielimy >= 2)
            //{
            //    MessageBox.Show("Ze względu na wielkość wykaz podzielono na " + naIlePlikowDzielimy + " pliki. Zapisz każdy z nich.", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //else if (naIlePlikowDzielimy > 4)
            //{
            //    MessageBox.Show("Ze względu na wielkość wykaz podzielono na " + naIlePlikowDzielimy + " plików. Zapisz każdy z nich.", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Information);
            //}



            //for (int i = 0; i < naIlePlikowDzielimy; i++)
            //{

            //    if (i == naIlePlikowDzielimy - 1)
            //    {
            //        jR_s.Add(JednostkiRejestroweNowe.Jedn_REJ_N.GetRange(i * 100, (JednostkiRejestroweNowe.Jedn_REJ_N.Count - (i * 100))));
            //    }
            //    else
            //    {
            //        jR_s.Add(JednostkiRejestroweNowe.Jedn_REJ_N.GetRange(i * 100, 100));
            //    }
            //    Console.WriteLine(i * 100);
            //}

            //foreach (var item in jR_s)
            //{
            //    zapisDoPliku(GenerujWWE(item), ".doc");
            //}

            zapisDoPliku(GenerujWWE(JednostkiRejestroweNowe.Jedn_REJ_N), ".doc");
        }

        private void RichTextBox_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void MenuItem_ClicGenerujWWEdlaWybranego(object sender, RoutedEventArgs e)
        {

            WindowPobierzNKR windowPobierzNKR = new WindowPobierzNKR();
            windowPobierzNKR.Show();
        }


        public string GenerujUproszczonyWWE(List<JR_Nowa> jR_Nowa)
        {
            StringBuilder dokHTML = new StringBuilder();
            dokHTML.AppendLine(HtmlDokumentUproszczonyWykazWydzEkwiwalentow.HTML_PoczatekWykazyWydzEkwiwalentow);
            dokHTML.AppendLine(HtmlDokumentUproszczonyWykazWydzEkwiwalentow.HTML_PodzialSekcjiNaStronieNieparzystej);

            foreach (var JednoskaRejNowa in jR_Nowa)
            {
                dokHTML.Append(HtmlDokumentUproszczonyWykazWydzEkwiwalentow.GenerujWykazWE(JednoskaRejNowa));
            }
            dokHTML.AppendLine(HtmlDokumentUproszczonyWykazWydzEkwiwalentow.HTML_ZakonczenieWykazuWydzEkwiw);
            //JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.AppendText("W jednostce: " + x.IjrPo.ToString() + " brakuje numeru obrębu"));
            return dokHTML.ToString();
        }


        private void ButtonUproszczonyWszytkieDoWWE_Click(object sender, RoutedEventArgs e)
        {
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR OBREBU\n");
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.Nkr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR JEDNOSTKI REJESTROWEJ\n");
            zapisDoPliku(GenerujUproszczonyWWE(JednostkiRejestroweNowe.Jedn_REJ_N), ".doc");
        }
    }
}
