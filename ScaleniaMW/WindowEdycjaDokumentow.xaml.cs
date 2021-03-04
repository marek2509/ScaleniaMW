using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy EdycjaDokumentow.xaml
    /// </summary>
    public partial class WindowEdycjaDokumentow : Window
    {
        /* public void odczytUstawien()
         {
             try
             {
                 checkBoxPodzialSekcji.IsChecked = Properties.Settings.Default.CheckBoxPodzialSekcji;
             }
             catch
             {
                 Console.WriteLine("Nie odczytano ustawień");
             }
         }

         public void zapisUstawien()
         {
             try
             {
                 Properties.Settings.Default.CheckBoxPodzialSekcji = (bool)checkBoxPodzialSekcji.IsChecked;
             }
             catch
             {
                 Console.WriteLine("Nie zapisno ustawień");
             }
         }*/



        public WindowEdycjaDokumentow()
        {
            InitializeComponent();
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

            if(liniaZTektowki.LastIndexOf('/') < liniaZTektowki.IndexOf(' '))
            {
                return false;
            }
            else
            if (ostatniMyslnik > liniaZTektowki.LastIndexOf('/'))
            {
                if(ostatniMyslnik < liniaZTektowki.Length - 1)
                {
                    if (char.IsNumber(liniaZTektowki[ostatniMyslnik + 1]))
                    {
                        return true;
                    }
                    else{
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


            /*  while (true)
              {
                  int indexOstatniMyslnik = liniaZTektowki.LastIndexOf('-');
                  int indexOstatniUkosnik = liniaZTektowki.LastIndexOf('/');

                  if (indexOstatniMyslnik > indexOstatniUkosnik)
                  {
                      liniaZTektowki = liniaZTektowki.Remove(indexOstatniMyslnik, 1);
                      liniaZTektowki = liniaZTektowki.Insert(indexOstatniMyslnik, "/");
                  }
                  else
                  {
                      break;
                  }
              }*/
            while (true)
            {
                int pierwsza_przerwa = liniaZTektowki.LastIndexOf(' ');
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

                return liniaZTektowki;
            
        }
        // 6-106                    6-38/Lzr                           506
        // 4-1/1                    4-898/Lzr-ŁVI-11478                  90.47
      /*  string UsunWartZKlasyfkacji(string liniaZTektowki, ref StringBuilder sbPuste)
        {

            if (liniaZTektowki.Contains("pusty") || liniaZTektowki.Trim() == "")
            {
                sbPuste.AppendLine(liniaZTektowki);
                return (liniaZTektowki);
            }

            int ukosnikKonturu = liniaZTektowki.IndexOf('/', liniaZTektowki.IndexOf(' '));
            int ostatniMyslnik = liniaZTektowki.LastIndexOf('-');
            if (ostatniMyslnik > ukosnikKonturu)
            {



            }

            liniaZTektowki.Replace('-', '/');
            int ileZnakowUsunac = liniaZTektowki.IndexOf(' ', ostatniMyslnik) - ostatniMyslnik;
            //   Console.WriteLine( calyOdczzytanyTextLinie[i] +  " last index -: " + ostatniMyslnik + " Dł: " + dlTekstu + " wynik:" + calyOdczzytanyTextLinie[i].Remove(ostatniMyslnik, ileZnakowUsunac));

            //   liniaZTektowki = liniaZTektowki.Replace(ostatniMyslnik, ileZnakowUsunac);

            return liniaZTektowki;
        }*/


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
    }
}
