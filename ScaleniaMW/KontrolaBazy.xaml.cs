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


        WindowLogowanie windowLogowanie = new WindowLogowanie();
        private void UstawLoginIHaslo(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("CZY OKNO JEST AKTYWNE");
            Console.WriteLine(windowLogowanie.IsActive);
            windowLogowanie.Show();
            Console.WriteLine(windowLogowanie.IsActive);
        }


        private void MenuItem_ClickWykonajKontrole(object sender, RoutedEventArgs e)
        {
            bool ZmienKolorKarciePrzed = false;
            bool ZmienKolorKarciePo = false;

            // tab item przed
            dgStanPrzedBledyKW.ItemsSource = KontroleDanychZFDB.sprawdzKwPrzedScaleniem();
            int ileElemKwPrzed = KontroleDanychZFDB.sprawdzKwPrzedScaleniem().Count;
            if (ileElemKwPrzed > 0)
            {
                ZmienKolorKarciePrzed = true;
                tabItemKWPRzed.Foreground = Brushes.Red;
            }
            else tabItemKWPRzed.Foreground = Brushes.Black;


            dgBrakJednRejPrzed.ItemsSource = KontroleDanychZFDB.jednostkiBezGrupRejestrowychPrzed().AsDataView();
            int ileElemBrakJRPrzed = KontroleDanychZFDB.jednostkiBezGrupRejestrowychPrzed().Rows.Count;
            if (ileElemBrakJRPrzed > 0)
            {
                ZmienKolorKarciePrzed = true;
                tabItemGrRejPrzed.Foreground = Brushes.Red;
            }else
            {
                tabItemGrRejPrzed.Foreground = Brushes.Black;
            }



            if (ZmienKolorKarciePrzed)
            {
                tabItemStanPRZED.Foreground = Brushes.Red;
            }
            else
            {
                tabItemStanPRZED.Foreground = Brushes.Black;
            }

            //tab Item Po
            //KW PO czy błędne
            dgStanPoBledyKW.ItemsSource = KontroleDanychZFDB.sprawdzKwPoScaleniu();
            int ileElemKwPo = KontroleDanychZFDB.sprawdzKwPoScaleniu().Count;
            if (ileElemKwPo > 0)
            {
                tabItemKWPo.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
            }
            else
            {
                tabItemKWPo.Foreground = Brushes.Black;
            }

            //przypisanie jednostki RJDRprzed w stanie Po
            dgStanPoNrJrPrzedWDz.ItemsSource = KontroleDanychZFDB.WypiszNkrZNieprzypiasnymNrIJR();
            dgStanPoNrJrPodejrzanyWNkr.ItemsSource = KontroleDanychZFDB.NkrZPodejrzanymIJRem();
            int ileElemRjdrPrzedWDz = KontroleDanychZFDB.WypiszNkrZNieprzypiasnymNrIJR().Count;
            int ileElemPodejrzanyNKR = KontroleDanychZFDB.NkrZPodejrzanymIJRem().Count;

            if (ileElemRjdrPrzedWDz > 0 || ileElemPodejrzanyNKR > 0)
            {
                tabItemNRJRPrzedWDzPo.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
            }
            else
            {
                tabItemNRJRPrzedWDzPo.Foreground = Brushes.Black;
            }


            // udzialu przed w stanie Po
            var wynikZapytaniaSqlUdzialyPrzedWStaniePo = KontroleDanychZFDB.UdzialyPrzedWStaniePo();
            int ileElemWTabeli = wynikZapytaniaSqlUdzialyPrzedWStaniePo.Rows.Count;
            dgUdzialyPrzedWStaniePo.ItemsSource = wynikZapytaniaSqlUdzialyPrzedWStaniePo.AsDataView();
            if (ileElemWTabeli > 0)
            {
                tabItemUdzialyPrzedWPo.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
            }
            else
            {
                tabItemUdzialyPrzedWPo.Foreground = Brushes.Black;
            }
            
            // wartosci przed z jednostek i z działek - porównanie
            var wynikListaWartoscizJednIDzialek = KontroleDanychZFDB.sprawdzenieSumWartosci();
            int ileElemWarZJednIDzialki = wynikListaWartoscizJednIDzialek.Count;
            dgSumaWartZDzIJedn.ItemsSource = wynikListaWartoscizJednIDzialek;
            if (ileElemWarZJednIDzialki > 0)
            {
                tabItemSumyZDzialekIZJednostki.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
            }
            else
            {
                tabItemSumyZDzialekIZJednostki.Foreground = Brushes.Black;
            }

            //porównanie własności(właścicieli) w stanie przed i po            
            textBoxWlasnosciPrzedPo.Text = KontroleDanychZFDB.GenerujTabeleRroznychWlasnosci();
            if (textBoxWlasnosciPrzedPo.Text != "")
            {
                ZmienKolorKarciePo = true;
                tabItemWlasnoscPrzePo.Foreground = Brushes.Red;
            }
            else
            {
                tabItemWlasnoscPrzePo.Foreground = Brushes.Red;
            }


            // jednRej bez grup rejestrowych
            dgBrakJednRejPo.ItemsSource = KontroleDanychZFDB.jednostkiBezGrupRejestrowychPo().AsDataView();
            int ileElemBrakJRPo = KontroleDanychZFDB.jednostkiBezGrupRejestrowychPo().Rows.Count;
            if (ileElemBrakJRPo > 0)
            {
                ZmienKolorKarciePrzed = true;
                tabItemGrRejPo.Foreground = Brushes.Red;
            }
            else
            {
                tabItemGrRejPo.Foreground = Brushes.Black;
            }

            //this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //zmiana koloru zakładki karty PO scaleniu
            if (ZmienKolorKarciePo)
            {
                tabItemStanPO.Foreground = Brushes.Red;
                MessageBox.Show("Znaleziono błędy. Zaznaczono karty na czerwono");
            }
            else
            {
                tabItemStanPO.Foreground = Brushes.Black;
                MessageBox.Show("Nie znaleziono błędów.");
            }


        }
    }
}
