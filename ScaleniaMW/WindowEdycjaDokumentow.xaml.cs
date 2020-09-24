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
        public WindowEdycjaDokumentow()
        {

            InitializeComponent();

           
           
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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    //////////////////////////////////////////////////////////////////////////////

                    //////////////////////////////////////////////////////////////////////////////
                    calyOdczzytanyTextLinie = odczytZPlikuLinie(dlg.FileName);

                    //  calyOdczzytanyTextLinie.ForEach(x => Console.WriteLine(x));
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
                    var resultat = MessageBox.Show("Wczytano.\nZapisz plik.", "Wczytano", MessageBoxButton.OK);
                }
                catch (Exception esa)
                {
                    Console.WriteLine("Nieprawidłowy format ciągu wejściowego. Wybierz ");
                }

                SaveFileDialog svd = new SaveFileDialog();
                svd.DefaultExt = ".rtf";
                svd.Filter = "Text files (*.rtf)|*.rtf|All files (*.*)|*.*";
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

                                calyOdczzytanyTextLinie.ForEach(x => sb.AppendLine(x));

                                sw.Write(sb.ToString());
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
    }
}
