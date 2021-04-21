using System;
using System.Collections.Generic;
using System.Data;
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
    /// Logika interakcji dla klasy KontrolaBazy.xaml
    /// </summary>
    public partial class WindowKontrolaBazy : Window
    {
        public WindowKontrolaBazy()
        {
            InitializeComponent();
            textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
           KontroleDanychZFDB.NkrZPodejrzanymIJRem();
            //  BazaFB.Get_DB_Connection();

            // MyDataGrid.ItemsSource = BazaFB.Get_DataTable("select * from dzialka").AsDataView();
        }

        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowKontrolaBazy.Close();
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
                    BazaFB.ustawProperties(dlg.FileName, ref textBlockSciezka);
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

        private void UstawLoginIHaslo(object sender, RoutedEventArgs e)
        {
            WindowLogowanie windowLogowanie = new WindowLogowanie();
            windowLogowanie.Show();
        }


        private void MenuItem_ClickWykonajKontrole(object sender, RoutedEventArgs e)
        {
            dgStanPrzedBledyKW.ItemsSource = KontroleDanychZFDB.sprawdzKwPrzedScaleniem();
            dgStanPoBledyKW.ItemsSource = KontroleDanychZFDB.sprawdzKwPoScaleniu();
            dgStanPoNrJrPrzedWDz.ItemsSource = KontroleDanychZFDB.WypiszNkrZNieprzypiasnymNrIJR();
        
            //  dgStanPoNrJrPodejrzanyWNkr
        }
    }
}
