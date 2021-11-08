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
            Wspolnota.progressBarJedn_rej = progresBarJedn_rej_n;
            Wspolnota.progressBarUzdialy = progresBarUdzialy_n;
            Wspolnota.progressBarJednSN = ProgresBarJedn_SN;
            Wspolnota.dpTworzenie = dockTworzenieJednUdzPodm;
            Wspolnota.checkBoxCzyDopisywacDoIstniejaczych = checkBoxCzyDopisywacDoIstniejacychJedostek;
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
                    labelWybraneJednostki.Text += Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex].IJR + " " + Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex].NazwaObrebu + "\n";
                }
                else
                {
                    labelWybraneJednostki.Text += "Ta jednostka jest już wybrana\n";
                }
            }
        }

        private void MenuItemPolaczZbaza_Click(object sender, RoutedEventArgs e)
        {
            labelWybraneJednostki.Text = "";
            Wspolnota.pobierzGminy();
            Wspolnota.pobierzObreby();
            Wspolnota.pobierzJednostki();
            menuItemPolaczZbaza.Background = Brushes.LightGreen;
            listBoxGm.ItemsSource = Wspolnota.listGminy.Select(x => x.Nazwa);
            listBoxGm.SelectedIndex = 0;
            listBoxObreby.ItemsSource = Wspolnota.listObreby.Select(x => x.Nazwa);
            listBoxObreby.SelectedIndex = 0;
            dgJednostkiNowe.ItemsSource = Wspolnota.listaJednostki_s;
            comboRWD.ItemsSource = Wspolnota.listaRWD.Select(x => x.Symbol);
            comboRWD.SelectedIndex = Wspolnota.listaRWD.FindIndex(x => x.Symbol.ToUpper() == "WŁ");
        }

        private void TworzNoweJedn_Click(object sender, RoutedEventArgs e)
        {
            if (Wspolnota.sprawdzSpojnoscWybranychJednostek() != null)
            {
                dgJednostkiNowe.ItemsSource = Wspolnota.sprawdzSpojnoscWybranychJednostek();
                labelWybraneJednostki.Text = "BRAK SPOJNOŚCI W WYBRANYCH JEDNOSTKACH!";
            }
            else
            {
                int nrPierwszej = 0;
                try
                {
                    Wspolnota.ileJednostekTrzebaUtworzyc(); // pobranie danych do utworzenia nowych jednostek
                    Console.WriteLine(">" + textBoxNrPierwszejJednostki.Text + "<");
                    nrPierwszej = Convert.ToInt32(textBoxNrPierwszejJednostki.Text);
                    Console.WriteLine(nrPierwszej);
                    Console.WriteLine("Wybrana gm: " + Wspolnota.listGminy[listBoxGm.SelectedIndex].Nazwa);
                    Console.WriteLine("Wybrany obręb: " + Wspolnota.listObreby[listBoxObreby.SelectedIndex].Nazwa);

                }
                catch (Exception ec)
                {
                    MessageBox.Show("Błędny format numeru pierwszej jednostki\n" + ec.Message);
                    goto koniec;
                }
                 Wspolnota.TworzenieJednostek(nrPierwszej, Wspolnota.listGminy[listBoxGm.SelectedIndex].ID_ID, Wspolnota.listObreby[listBoxObreby.SelectedIndex].IdObrebu, Wspolnota.listaRWD[comboRWD.SelectedIndex].ID_ID , comboGrupa.SelectionBoxItem.ToString());
            }
        koniec:;
        }

        private void TextBoxNrPierwszejJednostki_TextChanged(object sender, TextChangedEventArgs e)
        {
        
            String textDoZbadania = textBoxNrPierwszejJednostki.Text;
    
            textBoxNrPierwszejJednostki.Text = Plik.UsuniecieInnychZnakowNizCyfry(textDoZbadania);
            Console.WriteLine(textBoxNrPierwszejJednostki.GetLastVisibleLineIndex());
            Console.WriteLine(textBoxNrPierwszejJednostki.GetFirstVisibleLineIndex());
            Console.WriteLine(textBoxNrPierwszejJednostki.CaretIndex);
   
            textBoxNrPierwszejJednostki.CaretIndex = textBoxNrPierwszejJednostki.Text.Length;
        }

        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowPodzialWspolnoty.Close();
        }
    }
}
