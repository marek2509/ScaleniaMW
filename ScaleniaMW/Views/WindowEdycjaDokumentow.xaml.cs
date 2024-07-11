using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace ScaleniaMW.Views
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

            Console.WriteLine(">>" + Properties.Settings.Default.czyWziacNrJednRejZNkrPo);

            if (Properties.Settings.Default.czyWziacNrJednRejZNkrPo == true)
            {
                checkBoxWezZNKR.IsChecked = true;
                checkBoxWezZUwagi.IsChecked = false;
            }
            else
            {
                checkBoxWezZNKR.IsChecked = false;
                checkBoxWezZUwagi.IsChecked = true;
            }

            Helpers.ExtensionsClass.IsSemicolon =  Properties.Settings.Default.przecinekWWWe;
            CheckBoxIsSemicolon.IsChecked = Properties.Settings.Default.przecinekWWWe;
            // odczytUstawien();        
            Console.WriteLine("IsSemicolon: " + Helpers.ExtensionsClass.IsSemicolon);
        }

        string usunOd = "kontury";
        string usunDo = "bilans";
        private async void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
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
                        richTextBox.Text = sbPuste.ToString();
                    }
                    var resultat = MessageBox.Show("Wczytano.\nZapisz plik.", "Wczytano", MessageBoxButton.OK);
                }
                catch (Exception esa)
                {
                    Console.WriteLine("Nieprawidłowy format ciągu wejściowego. Wybierz " + esa);
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

        string UsunWartZSzacunkuIZamienNaUkosniki(string liniaZTektowki, ref StringBuilder sbPuste, bool usunWartosc = false)
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
                if (item != "")
                {
                    splitedString.Add(item);
                }
            }

            if (splitedString.Count != 3)
            {
                sbPuste.AppendLine("ERROR: " + liniaZTektowki);
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
                else if (tmpArray.Length == 1)
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
                while (processedValue.Length < 25)
                {
                    processedValue.Append(" ");
                }

                processedValue.Append(splitedString[1]);
                while (processedValue.Length < 67 - splitedString[2].Length)
                {
                    processedValue.Append(" ");
                }
                processedValue.Append(splitedString[2]);


                //jeśli nie usuwać wartości to zrób tak żeby przed wartością był myślnik

                if (regex.IsMatch(wartoscKontury))
                {

                    if (!usunWartosc)
                    {
                        var tmpString = processedValue.ToString();
                        var startString = tmpString.Remove(tmpString.LastIndexOf('/'));
                        var endString = tmpString.Remove(0, tmpString.LastIndexOf('/') + 1);
                        processedValue.Clear();
                        processedValue.Append(startString);
                        processedValue.Append("-");
                        processedValue.Append(endString);
                    }
                }
                return processedValue.ToString();
            }
        }


        public void zapisDoPliku(string tekstDoZapisu, string format = ".rtf", string nazwaPliku = "")
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.FileName = nazwaPliku;
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
                                    throw;
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
                Potracenie potracenie = new Potracenie();
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
                    bool dopldr = jednostkaRejestrowa.Rows[i][7].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][7]);
                    bool zerujDoplaty = jednostkaRejestrowa.Rows[i][8].Equals(DBNull.Value) ? false : Convert.ToBoolean(jednostkaRejestrowa.Rows[i][8]);

                    JednostkiRejestroweNowe.DodajJrNowa(idJednRejN, ijr, nkr, odcht, zgoda, uwaga, idobr, dopldr, zerujDoplaty);
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
                    string symbolWl = WlascicielePO.Rows[i][6].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][6].ToString();
                    string rodzice = WlascicielePO.Rows[i][9].ToString().Equals(DBNull.Value) ? "" : WlascicielePO.Rows[i][9].ToString();

                    Wlasciciel wlasciciel = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa, symbolWl, rodzice);
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

                    bool czyJestPotracenie = Jedn_SN.Rows[i][7].Equals(DBNull.Value) ? false : Convert.ToBoolean(Jedn_SN.Rows[i][7]);

                    ZJednRejStarej zJednRej = new ZJednRejStarej(idJednS, IJR, Ud_z_Jedn, WrtPrzed, PowPrzed, Idobr, potracenie.WartoscPotracenia, czyJestPotracenie);
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
                    JednostkiRejestroweNowe.Jedn_REJ_N.Find(x => x.IdJednRejN == Rjdr)?.DodajDzialke(dzialka);
                }

                DataTable Dzialki_przed = odczytajZSql(Constants.SQL_Dzialka);
                List<DzialkaWykEkwiw> DzialkaStaraTMP = new List<DzialkaWykEkwiw>();
                for (int i = 0; i < Dzialki_przed.Rows.Count; i++)
                {

                    int Id_dz = Dzialki_przed.Rows[i][0].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][0]);
                    int Id_obr = Dzialki_przed.Rows[i][1].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][1]);
                    string NrDz = Dzialki_przed.Rows[i][2].ToString().Equals(DBNull.Value) ? "" : Dzialki_przed.Rows[i][2].ToString();
                    double PowDz = Dzialki_przed.Rows[i][3].Equals(DBNull.Value) ? 0 : Convert.ToDouble(Dzialki_przed.Rows[i][3]);
                    int Rjdr = Dzialki_przed.Rows[i][4].Equals(DBNull.Value) ? 0 : Convert.ToInt32(Dzialki_przed.Rows[i][4]);
                    string KW = Dzialki_przed.Rows[i][5].ToString().Equals(DBNull.Value) ? "" : Dzialki_przed.Rows[i][5].ToString();
                    decimal Wartosc = Dzialki_przed.Rows[i][6].Equals(DBNull.Value) ? 0 : Convert.ToDecimal(Dzialki_przed.Rows[i][6]);

                    DzialkaStaraTMP.Add(new DzialkaWykEkwiw(Id_dz, Id_obr, NrDz, PowDz, Rjdr, KW, Wartosc));
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
                    string symbolWlad = WlascicielePrzed.Rows[i][6].ToString().Equals(DBNull.Value) ? "" : WlascicielePrzed.Rows[i][6].ToString();
                    // Wlasciciel wlascicielPrzed = new Wlasciciel(udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa);
                    WlascicielePrzedTMP.Add(new WlascicielStanPrzed(idJednS, udzial, udzial_NR, nazwaWlasciciela.ToUpper(), adres.ToUpper(), idMalzenstwa, symbolWlad));
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

                // sortowanie właścicieli
                JednostkiRejestroweNowe.Jedn_REJ_N.ForEach(x => x.Wlasciciele.Sort(JR_Nowa.CompareStringRodzWlada));

                JednostkiRejestroweNowe.Jedn_REJ_N.ForEach(x => x.zJednRejStarej.ForEach(y => y.Wlasciciele.Sort(JR_Nowa.CompareStringRodzWlada)));

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








        private void ButtonUproszczonyWszytkieDoWWE_Click(object sender, RoutedEventArgs e)
        {
            //JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR OBREBU\n");
            //JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.Nkr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR JEDNOSTKI REJESTROWEJ\n");
            IHTMLDokument dokumentWWE = new HtmlDokumentUproszczonyWykazWydzEkwiwalentow();
            zapisDoPliku(dokumentWWE.GenerujWWE(JednostkiRejestroweNowe.Jedn_REJ_N), ".doc");
        }

        private void MenuItem_ClicGenerujUproszczonyWWEdlaWybranego(object sender, RoutedEventArgs e)
        {
            IHTMLDokument dokumentWWE = new HtmlDokumentUproszczonyWykazWydzEkwiwalentow();
            WindowPobierzNKR windowPobierzNKR = new WindowPobierzNKR(dokumentWWE);
            windowPobierzNKR.Show();
        }

        private void CheckBoxWezZNKR_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.czyWziacNrJednRejZNkrPo = true;
            Properties.Settings.Default.Save();
            Console.WriteLine(Properties.Settings.Default.czyWziacNrJednRejZNkrPo);
        }

        private void CheckBoxWezZUwagi_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.czyWziacNrJednRejZNkrPo = false;
            Properties.Settings.Default.Save();
            Console.WriteLine(Properties.Settings.Default.czyWziacNrJednRejZNkrPo);
        }

        class ModelNKR
        {
            public ModelNKR(int nkr)
            {
                NKR = nkr;
            }
            public int NKR { get; set; }
        }

        List<ModelNKR> listNKR;

        private void ItemGenerujPlikPoprawDane_DodajDzialki_0_Click(object sender, RoutedEventArgs e)
        {

            DataTable jednostkiBezDzialekPoScaleniu = odczytajZSql(Constants.SQLIJRGdzieBrakStanuPoScaleniu);
            listNKR = new List<ModelNKR>();
            // powinna być jedna kolumna z nr NKR(jedn_rej_n.ijr) dla których brak stanu po scaleniu
            StringBuilder plikPoprawDane = new StringBuilder();

            for (int i = 0; i < jednostkiBezDzialekPoScaleniu.Rows.Count; i++)
            {
                for (int j = 0; j < jednostkiBezDzialekPoScaleniu.Columns.Count; j++)
                {
                    int nkr = Convert.ToInt32(jednostkiBezDzialekPoScaleniu.Rows[i][j]);
                    listNKR.Add(new ModelNKR(nkr));
                }
            }
            dataGridNkrBezDzialekPo.ItemsSource = listNKR;
            pokazPanelLadowaniaDzialek_0();
        }

        void pokazPanelLadowaniaDzialek_0(bool pokaz = true)
        {
            if (pokaz)
            {
                dataGridNkrBezDzialekPo.Visibility = Visibility.Visible;
                gridBtnsZatwierdzAnuluj.Visibility = Visibility.Visible;
            }
            else
            {
                dataGridNkrBezDzialekPo.Visibility = Visibility.Hidden;
                gridBtnsZatwierdzAnuluj.Visibility = Visibility.Hidden;
            }
        }

        private void BtnZaladujDzialkiZero_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new FbConnection(BazaFB.connectionString()))
            {
                connection.Open();
                FbCommand wstawDzialkeZero = new FbCommand(Constants.SQLInsertDzialka0, connection);

                foreach (var nkr in listNKR)
                {
                    wstawDzialkeZero.Parameters.Add("@NKR", nkr.NKR);
                    wstawDzialkeZero.ExecuteNonQuery();
                    wstawDzialkeZero.Parameters.Clear();
                }
                connection.Close();
            }

            messageThumbUp();
            pokazPanelLadowaniaDzialek_0(false);
        }

        public void messageThumbUp()
        {
            MessageBox.Show("OK! 👍");
        }

        private void BtnAnulujDzialkiZero_Click(object sender, RoutedEventArgs e)
        {
            pokazPanelLadowaniaDzialek_0(false);
        }

        private void BtnGenerujPLikPoprawDane_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder poprawDane = new StringBuilder();
            poprawDane.AppendLine("****");
            poprawDane.AppendLine("insert into DZIALKI_N(id_id, idobr, idd, rjdr, id_rd, sidd, pew, v) values((select gen_id(ID_DZIALKI_N, 1)from rdb$database), 1, 0, (select id_id from jedn_rej_n where ijr =:NKR), 1, (select gen_id(ID_DZIALKI_N, 0)from rdb$database), 0, 0)");
            poprawDane.AppendLine("****");
            poprawDane.AppendLine("NKR");
            listNKR.ForEach(x => poprawDane.AppendLine(x.NKR.ToString()));

            zapisDoPliku(poprawDane.ToString(), ".txt");
            messageThumbUp();
            pokazPanelLadowaniaDzialek_0(false);
        }

        private void ItemUsunDzialki_0_Click(object sender, RoutedEventArgs e)
        {
            BazaFB.Execute_SQL(Constants.SQLDeleteAllDzialki0);
        }


        private void Generuj_Wykaz_wydz_ekw_Click(object sender, RoutedEventArgs e)
        {
            IHTMLDokument dokumentWWE = new HtmlDokumentWykazWydzEkwiwalentow();
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR OBREBU\n");
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.Nkr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR JEDNOSTKI REJESTROWEJ\n");
            zapisDoPliku(dokumentWWE.GenerujWWE(JednostkiRejestroweNowe.Jedn_REJ_N), ".doc");

            richTextBox.Text += JednostkiRejestroweNowe.KontrolaPrzypisaniaDoRjdr();
        }

        private void MenuItem_ClicGenerujWWEdlaWybranego(object sender, RoutedEventArgs e)
        {
            IHTMLDokument dokumentWWE = new HtmlDokumentWykazWydzEkwiwalentow();
            WindowPobierzNKR windowPobierzNKR = new WindowPobierzNKR(dokumentWWE);
            windowPobierzNKR.Show();
        }

        private void ButtonPotraceniaWszytkieDoWWE_Click(object sender, RoutedEventArgs e)
        {
            IHTMLDokument dokumentWWE = new HTMLDokWykazEkwPotracenia();
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x._id_obr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR OBREBU\n");
            JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.Nkr == 0).ForEach(x => richTextBox.Text += "W jednostce:\t" + x.IjrPo.ToString() + " BRAK NR JEDNOSTKI REJESTROWEJ\n");
            zapisDoPliku(dokumentWWE.GenerujWWE(JednostkiRejestroweNowe.Jedn_REJ_N), ".doc");

            richTextBox.Text += JednostkiRejestroweNowe.KontrolaPrzypisaniaDoRjdr();
        }

        private void ButtonPotraceniaWWEDlaWybranego_Click(object sender, RoutedEventArgs e)
        {
            IHTMLDokument dokumentWWE = new HTMLDokWykazEkwPotracenia();
            WindowPobierzNKR windowPobierzNKR = new WindowPobierzNKR(dokumentWWE);
            windowPobierzNKR.Show();
        }

        private async void MenuItem_ClickGenerujTekstowyWykazJedn(object sender, RoutedEventArgs e)
        {
            int idMalz = 0;

            string piszMalzenstwo(int idMalzenstwa)
            {
                if (idMalzenstwa > 0)
                {
                    if (idMalz == idMalzenstwa) // drugi małżonek nie wymaga opisu małżeństwo
                    {
                        return "";
                    }
                    idMalz = idMalzenstwa;
                    return "(małżeństwo)^";
                }
                return "";
            }

            var dlaChemika = await Task.Run(() => JednostkiRejestroweNowe.Jedn_REJ_N.Select(jedn => new
            {

                podmiot = string.Join("^", jedn.Wlasciciele.Select((x) =>

                            piszMalzenstwo(x.IdMalzenstwa) + x.Udzial + " " + x.Symbol_Wladania + "-" + x.MazCzyZona + " " + x.NazwaWlasciciela + (x.Rodzice == null || x.Rodzice == "" ? "" : "^" + x.Rodzice) + (x.Adres == null || x.Adres == "" ? "" : "^" + x.Adres)
                   )),
                ijr = jedn.IjrPo,
                dzialka = string.Join("^", jedn.Dzialki_Nowe.Select(d => d.NrObr + "-" + d.NrDz + " " + d.PowDz.ToString("F4", CultureInfo.InvariantCulture) + "ha")),
            }));

            var doWydruku = dlaChemika.ToList().Select(x => string.Join("\t", x.ijr, x.podmiot, x.dzialka));
            await Task.Run(() => EWOPIS.Infrstruktura.Plik.Save(string.Join("\n", doWydruku)));
        }

        private void CheckBoxIsSemicolon_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.przecinekWWWe = true;
            Properties.Settings.Default.Save();
            Console.WriteLine(Properties.Settings.Default.przecinekWWWe);
            Helpers.ExtensionsClass.IsSemicolon = true;
        }

        private void CheckBoxIsSemicolon_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.przecinekWWWe = false;
            Properties.Settings.Default.Save();
            Console.WriteLine(Properties.Settings.Default.przecinekWWWe);
            Helpers.ExtensionsClass.IsSemicolon = false;
        }

        private void MenuItemWzorzec_Click(object sender, RoutedEventArgs e)
        {
            WindowWzorzec windowWzorzec = new WindowWzorzec();
            windowWzorzec.Show();
        }

        private void ItemGenerujPlikPoprawDane_DodajDzialki_0_tez_dla_usunietych_Click(object sender, RoutedEventArgs e)
        {
            DataTable jednostkiBezDzialekPoScaleniu = odczytajZSql(Constants.SQLIJRGdzieBrakStanuPoScaleniuRowniezWUsunietych);
            listNKR = new List<ModelNKR>();
            // powinna być jedna kolumna z nr NKR(jedn_rej_n.ijr) dla których brak stanu po scaleniu
            StringBuilder plikPoprawDane = new StringBuilder();

            for (int i = 0; i < jednostkiBezDzialekPoScaleniu.Rows.Count; i++)
            {
                for (int j = 0; j < jednostkiBezDzialekPoScaleniu.Columns.Count; j++)
                {
                    int nkr = Convert.ToInt32(jednostkiBezDzialekPoScaleniu.Rows[i][j]);
                    listNKR.Add(new ModelNKR(nkr));
                }
            }
            dataGridNkrBezDzialekPo.ItemsSource = listNKR;
            pokazPanelLadowaniaDzialek_0();
        }
    }
}
