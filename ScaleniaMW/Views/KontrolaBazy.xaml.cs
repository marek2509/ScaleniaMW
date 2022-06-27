using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScaleniaMW.Views
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



        private void UstawLoginIHaslo(object sender, RoutedEventArgs e)
        {
            WindowLogowanie windowLogowanie = new WindowLogowanie();
            windowLogowanie.Show();
        }


        //async Task<T> KwPrzedAsync<T>(Action action, String message = "")
        //{
        //    Console.WriteLine("PRZED " + message);
        //     result = await Task.Run(action);
        //    Console.WriteLine("PO " + message);
        //    return result.Result;
        //}


        async Task<T> KwPrzedAsync<T>(Func<T> func)
        {
            var result = await Task.Run(func);
            return result;
        }


        private async void MenuItem_ClickWykonajKontrole(object sender, RoutedEventArgs e)
        {
            bool ZmienKolorKarciePrzed = false;
            bool ZmienKolorKarciePo = false;


            // tab item przed
            // sprawdzenie poprawności KW w działkach przed scaleniem       
            var resultsprawdzKwPrzedScaleniem = await Task.Run(() => KontroleDanychZFDB.sprawdzKwPrzedScaleniem());


            dgStanPrzedBledyKW.ItemsSource = resultsprawdzKwPrzedScaleniem;

            int ileElemKwPrzed = resultsprawdzKwPrzedScaleniem.Count;
            if (ileElemKwPrzed > 0)
            {
                //ZmienKolorKarciePrzed = true;
                Console.WriteLine("1. sprawdzKwPrzedScaleniem TRUE");
                tabItemKWPRzed.Foreground = Brushes.Red;
            }
            else
            {
                Console.WriteLine("1. sprawdzKwPrzedScaleniem false");
                tabItemKWPRzed.Foreground = Brushes.Black;
            }




            //suma udzialow przed scaleniem
            var resultUdzialyPrzed = await Task.Run(() => KontroleDanychZFDB.udzialyRozneOd1Przed().AsDataView());
            dgUdzialyWJednostkachPRZED.ItemsSource = resultUdzialyPrzed;
            int ileElemUdzPrzed = resultUdzialyPrzed.Count;
            if (ileElemUdzPrzed > 0)
            {
                ZmienKolorKarciePrzed = true;
                tabItemUdzialyWJednostkach.Foreground = Brushes.Red;
                Console.WriteLine("2 udzialyRozneOd1Przed TRUE");
            }
            else
            {
                tabItemUdzialyWJednostkach.Foreground = Brushes.Black;
                Console.WriteLine("2 udzialyRozneOd1Przed False");
            }



            var resultJednBezGrRejPrzed = await Task.Run(() => KontroleDanychZFDB.jednostkiBezGrupRejestrowychPrzed().AsDataView());
            dgBrakJednRejPrzed.ItemsSource = resultJednBezGrRejPrzed;
            int ileElemBrakJRPrzed = resultJednBezGrRejPrzed.Count;
            if (ileElemBrakJRPrzed > 0)
            {
                ZmienKolorKarciePrzed = true;
                tabItemGrRejPrzed.Foreground = Brushes.Red;
                Console.WriteLine("3 jednostkiBezGrupRejestrowychPrzed TRUE");
            }
            else
            {
                tabItemGrRejPrzed.Foreground = Brushes.Black;
                Console.WriteLine("3 jednostkiBezGrupRejestrowychPrzed false");
            }


            // ustawienie koloru czerwonego karcie stanu przed scaleniem
            if (ZmienKolorKarciePrzed)
            {
                tabItemStanPRZED.Foreground = Brushes.Red;
                Console.WriteLine("4 KARTA PRZED TRUE");
            }
            else
            {
                tabItemStanPRZED.Foreground = Brushes.Black;
                Console.WriteLine("4 KARTA PRZED False");
            }


            //tab Item Po
            //suma udzialow przed scaleniem
            var resultUdzialyRozneOd1Po = await Task.Run(() => KontroleDanychZFDB.udzialyRozneOd1Po().AsDataView());
            dgUdzialyWJednostkachPo.ItemsSource = resultUdzialyRozneOd1Po;
            int ileElemUdzPo = resultUdzialyRozneOd1Po.Count;
            if (ileElemUdzPo > 0)
            {
                ZmienKolorKarciePrzed = true;
                tabItemUdzialyWJednostkachPo.Foreground = Brushes.Red;
                Console.WriteLine("5 udzialyRozneOd1Po TRUE");
            }
            else
            {
                tabItemUdzialyWJednostkachPo.Foreground = Brushes.Black;
                Console.WriteLine("5 udzialyRozneOd1Po False");
            }


            //KW PO czy błędne
            var resultKwPo = await Task.Run(() => KontroleDanychZFDB.sprawdzKwPoScaleniu());
            dgStanPoBledyKW.ItemsSource = resultKwPo;
            int ileElemKwPo = resultKwPo.Count;
            if (ileElemKwPo > 0)
            {
                tabItemKWPo.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
                Console.WriteLine("6 udzialyRozneOd1Po TRUE");
            }
            else
            {
                tabItemKWPo.Foreground = Brushes.Black;
                Console.WriteLine("6 udzialyRozneOd1Po False");
            }


            //przypisanie jednostki RJDRprzed w stanie Po
            var rasultNkrZNieprzypiasnymNrIJR = await Task.Run(() => KontroleDanychZFDB.WypiszNkrZNieprzypiasnymNrIJR());
            dgStanPoNrJrPrzedWDz.ItemsSource = rasultNkrZNieprzypiasnymNrIJR;
            int ileElemRjdrPrzedWDz = KontroleDanychZFDB.WypiszNkrZNieprzypiasnymNrIJR().Count;

            var rasultPodejrzanyNkr = await Task.Run(() => KontroleDanychZFDB.NkrZPodejrzanymIJRem());
            dgStanPoNrJrPodejrzanyWNkr.ItemsSource = rasultPodejrzanyNkr;

            int ileElemPodejrzanyNKR = KontroleDanychZFDB.NkrZPodejrzanymIJRem().Count;

            if (ileElemRjdrPrzedWDz > 0 || ileElemPodejrzanyNKR > 0)
            {
                tabItemNRJRPrzedWDzPo.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
                Console.WriteLine("7 WypiszNkrZNieprzypiasnymNrIJR TRUE");
            }
            else
            {
                tabItemNRJRPrzedWDzPo.Foreground = Brushes.Black;
                Console.WriteLine("7 WypiszNkrZNieprzypiasnymNrIJR TRUE");
            }


            // udzialu przed w stanie Po
            var resultUdzialyPo = await Task.Run(() => KontroleDanychZFDB.UdzialyPrzedWStaniePo().AsDataView());
            int ileElemWTabeli = resultUdzialyPo.Count;
            dgUdzialyPrzedWStaniePo.ItemsSource = resultUdzialyPo;
            if (ileElemWTabeli > 0)
            {
                tabItemUdzialyPrzedWPo.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
                Console.WriteLine("7 UdzialyPrzedWStaniePo TRUE");
            }
            else
            {
                tabItemUdzialyPrzedWPo.Foreground = Brushes.Black;
                Console.WriteLine("7 UdzialyPrzedWStaniePo False");
            }

            // wartosci przed z jednostek i z działek - porównanie
            var wynikListaWartoscizJednIDzialek = await Task.Run(() => KontroleDanychZFDB.sprawdzenieSumWartosci());
            int ileElemWarZJednIDzialki = wynikListaWartoscizJednIDzialek.Count;
            dgSumaWartZDzIJedn.ItemsSource = wynikListaWartoscizJednIDzialek;
            if (ileElemWarZJednIDzialki > 0)
            {
                tabItemSumyZDzialekIZJednostki.Foreground = Brushes.Red;
                ZmienKolorKarciePo = true;
                Console.WriteLine("8 sprawdzenieSumWartosci TRUE");
            }
            else
            {
                tabItemSumyZDzialekIZJednostki.Foreground = Brushes.Black;
                Console.WriteLine("8 sprawdzenieSumWartosci false");
            }

            //porównanie własności(właścicieli) w stanie przed i po
            var resultRozneWlasnosci = await Task.Run(() => KontroleDanychZFDB.GenerujTabeleRroznychWlasnosci());
            textBoxWlasnosciPrzedPo.Text = resultRozneWlasnosci;
            if (textBoxWlasnosciPrzedPo.Text != "")
            {
                ZmienKolorKarciePo = true;
                tabItemWlasnoscPrzePo.Foreground = Brushes.Red;
                Console.WriteLine("9 GenerujTabeleRroznychWlasnosci TRUE");
            }
            else
            {
                tabItemWlasnoscPrzePo.Foreground = Brushes.Red;
                Console.WriteLine("9 GenerujTabeleRroznychWlasnosci TRUE");
            }








            // jednRej bez grup rejestrowych


            Task<DataTable> resultJednBezGrRejPo = Task.Run(() => KontroleDanychZFDB.jednostkiBezGrupRejestrowychPo());

                dgBrakJednRejPo.ItemsSource = resultJednBezGrRejPo.Result.AsDataView();
                int ileElemBrakJRPo = resultJednBezGrRejPo.Result.AsDataView().Count;
                if (ileElemBrakJRPo > 0)
                {
                    ZmienKolorKarciePrzed = true;
                    tabItemGrRejPo.Foreground = Brushes.Red;

                Console.WriteLine("10");
                }
                else
                {
                    tabItemGrRejPo.Foreground = Brushes.Black;
                Console.WriteLine("10");
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
