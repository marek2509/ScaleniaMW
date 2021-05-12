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
using System.Windows.Shapes;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy WindowPodzialWspolnoty.xaml
    /// </summary>
    public partial class WindowPodzialWspolnoty : Window
    {
        public WindowPodzialWspolnoty()
        {
            InitializeComponent();

        }

        private void ustawSciezkeFDB(object sender, RoutedEventArgs e)
        {
            menuItemPolaczZbaza.Background = Brushes.Transparent;
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
            menuItemPolaczZbaza.Background = Brushes.Transparent;
            windowLogowanie.Show();
        }



        List<TextBox> listTextbox = new List<TextBox>();
        int ileIjrBox = 0;
        int ileObrBox = 0;

        private void ButtonWybierzZaznJednostke_Click(object sender, RoutedEventArgs e)
        {
      
            if (dgJednostkiNowe.SelectedIndex < Wspolnota.listaJednostki_s.Count && dgJednostkiNowe.SelectedIndex >=0)
            {
                if (!Wspolnota.listaWybranychJednstek_s.Exists(x => x == Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex]))
                {
                    Wspolnota.listaWybranychJednstek_s.Add(Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex]);
                    labelWybraneJednostki.Content += Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex].IJR + " " + Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex].NazwaObrebu + "\n";

                }
                else
                {
                    labelWybraneJednostki.Content += "Ta jednostka jest już wybrana\n";
                }
               

            }
           
        }

        private void MenuItemPolaczZbaza_Click(object sender, RoutedEventArgs e)
        {
            labelWybraneJednostki.Content = "";
            Wspolnota.pobierzGminy();
            Wspolnota.pobierzObreby();
            Wspolnota.pobierzJednostki_n();
            menuItemPolaczZbaza.Background = Brushes.LightGreen;
            listBoxGm.ItemsSource = Wspolnota.listGminy.Select(x => x.Nazwa);
            listBoxGm.SelectedIndex = 0;
            listBoxObreby.ItemsSource = Wspolnota.listObreby.Select(x => x.Nazwa);
            listBoxObreby.SelectedIndex = 0;

            dgJednostkiNowe.ItemsSource = Wspolnota.listaJednostki_s;

        }

        private void TworzNoweJedn_Click(object sender, RoutedEventArgs e)
        {



            if (Wspolnota.sprawdzSpojnoscWybranychJednostek() != null)
            {
                dgJednostkiNowe.ItemsSource = Wspolnota.sprawdzSpojnoscWybranychJednostek();
                labelWybraneJednostki.Content = "BRAK SPOJNOŚCI W WYBRANYCH JEDNOSTKACH!";
            }
            else
            {
                int nrPierwszej = 0;
                try
                {
                    Wspolnota.ileJednostekTrzebaUtworzyc();
                    nrPierwszej = Convert.ToInt32(textBoxNrPierwszejJednostki.Text);
                    Console.WriteLine(nrPierwszej);
                    Console.WriteLine("Wybrana gm: " + Wspolnota.listGminy[listBoxGm.SelectedIndex].Nazwa);
                    Console.WriteLine("Wybrany obręb: " + Wspolnota.listObreby[listBoxObreby.SelectedIndex].Nazwa);
                }
                catch
                {
                    MessageBox.Show("Błędny format numeru pierwszej jednostki");
                    goto koniec;
                }

                 Wspolnota.TworzenieJednostek(nrPierwszej, Wspolnota.listGminy[listBoxGm.SelectedIndex].ID_ID, Wspolnota.listObreby[listBoxObreby.SelectedIndex].IdObrebu, comboGrupa.SelectionBoxItem.ToString());


            }
        koniec:;
        }
    }
}
