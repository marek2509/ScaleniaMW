using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
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


            char[] charSeparators = new char[] { '\t' };
            string[] wartosciZlini = LiniaTekstu.Trim().Split(charSeparators);

            return wartosciZlini;
        }

        private void otworzEDZ(object sender, RoutedEventArgs e)
        {
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
                       if(calyOdczzytanyTextLinie[i].Trim() == "" || calyOdczzytanyTextLinie[i].Trim().Equals(null))
                       {
                            continue;
                       }
                        else
                        {
                            listaBezPustychLinii.Add(calyOdczzytanyTextLinie[i].Trim());
                        }
                    }
                    Console.WriteLine("LISTALISTALISTALISTALISTALISTALISTALISTALISTA");
                    foreach (var item in listaBezPustychLinii)
                    {
                        Console.WriteLine(item);
                    }

                    for (int i = 0; i < listaBezPustychLinii.Count; i++)
                    {
                        string[] wartosciZlini = pobranieWartoscZLinii(listaBezPustychLinii[i]);

                    }




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


    }
}
