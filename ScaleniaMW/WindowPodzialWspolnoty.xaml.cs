using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            textBlockSciezka.Text = Properties.Settings.Default.PathFDB.ToString();
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
                    Properties.Settings.Default.PathFDB = textBlockSciezka.Text;
                    Properties.Settings.Default.Save();
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

        private void ButtonWybierzZaznJednostke_Click(object sender, RoutedEventArgs e)
        {
            if (dgJednostkiNowe.SelectedIndex < Wspolnota.listaJednostki_s.Count && dgJednostkiNowe.SelectedIndex >=0)
            {
                if (!Wspolnota.listaWybranychJednstek_s.Exists(x => x == Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex]))
                {

                    if (Wspolnota.sprawdzSpojnoscWybranychJednostek() != null)
                    {
                        dgJednostkiNowe.ItemsSource = Wspolnota.sprawdzSpojnoscWybranychJednostek();
                        MessageBox.Show("BRAK SPOJNOŚCI W WYBRANYCH JEDNOSTKACH!");
                    }
                    else
                    {

                  
                    Wspolnota.listaWybranychJednstek_s.Add(Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex]);
                    labelWybraneJednostki.Text += Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex].IJR + " " + Wspolnota.listaJednostki_s[dgJednostkiNowe.SelectedIndex].NazwaObrebu + "\n";
                    Wspolnota.zaladujDaneDoTworzeniaJednostek(); // pobranie danych do utworzenia nowych jednostek
                                                            //dgDopasujDoIstniejacych.ItemsSource = Wspolnota.podmiotyOrazIchIstniejaceJednostki;
                    }
                }
                else
                {
                    labelWybraneJednostki.Text += "Ta jednostka jest już wybrana\n";
                }
            }

          
           

            //foreach (var item in Wspolnota.podmiotyOrazIchIstniejaceJednostki)
            //{
            //    TmpCl tmp = new TmpCl();
            //    tmp.Id_podmiotu = item.IdPodm;

            //    foreach (var nkr in item.IdJedn_Nkr)
            //    {
            //        tmp.NKR.Add(nkr.NKR);
            //    }
            //    Console.WriteLine(">?oo: " + item);
            //    ListTmpCls.Add(tmp);
            //}

            listBoxOwner.ItemsSource = Wspolnota.podmiotyOrazIchIstniejaceJednostki.Select(x => x.IdPodm);
            listBoxOwner.Items.Refresh();
        }
      

        private void ListBoxOwner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxOwner.SelectedIndex = listBoxOwner.SelectedIndex >= 0 && listBoxOwner.SelectedIndex < listBoxOwner.Items.Count ? listBoxOwner.SelectedIndex : 0;
            refreshList();


        }

        private void SetNkrMinimum_Click(object sender, RoutedEventArgs e)
        {
            Wspolnota.podmiotyOrazIchIstniejaceJednostki.ForEach(x => x.setNKRMinWybrane());
            refreshList();
        }

        private void SetNkrSelected_Click(object sender, RoutedEventArgs e)
        {

            Wspolnota.podmiotyOrazIchIstniejaceJednostki[listBoxOwner.SelectedIndex].setSelectedNkrpoIndexie(listBoxNkr.SelectedIndex);
            refreshList();

              // wyswietlenie ile jednostek jest nieprzypisanych
        }

        private void SetNkrZero_Click(object sender, RoutedEventArgs e)
        {
            Wspolnota.podmiotyOrazIchIstniejaceJednostki[listBoxOwner.SelectedIndex].setNkrZero();
            refreshList();
        }

        private void CheckBoxOlnyUnset_Checked(object sender, RoutedEventArgs e)
        {
            //  checkBoxOlnyUnset
        }

        private void CheckBoxOlnyUnset_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        public void refreshList()
        {
            if( Wspolnota.podmiotyOrazIchIstniejaceJednostki.Count > 0)
            {
                labelWybranaJesnostka.Content = Wspolnota.podmiotyOrazIchIstniejaceJednostki[listBoxOwner.SelectedIndex].WybranyNKR.ToString();
                listBoxNkr.ItemsSource = Wspolnota.podmiotyOrazIchIstniejaceJednostki[listBoxOwner.SelectedIndex].IdJedn_Nkr.Select(n => n.NKR);
                LabelIlePozostaloJednostek.Content = Wspolnota.podmiotyOrazIchIstniejaceJednostki.FindAll(x => x.WybraneId == 0).Count + "/" + Wspolnota.podmiotyOrazIchIstniejaceJednostki.Count;
                listBoxNkr.Items.Refresh();
            }
        }



        //public class TmpCl
        //{
        //    public int Id_podmiotu { get; set; }
        //    public List<int> NKR { get; set; }

        //    public TmpCl()
        //    {
        //        initTmpCl();
        //    }
        //    void initTmpCl()
        //    {
        //        NKR = new List<int>();
        //    }
        //}

        //List<TmpCl> ListTmpCls = new List<TmpCl>();
        private void TworzNoweJedn_Click(object sender, RoutedEventArgs e)
        {
            if (Wspolnota.sprawdzSpojnoscWybranychJednostek() != null)
            {
                dgJednostkiNowe.ItemsSource = Wspolnota.sprawdzSpojnoscWybranychJednostek();
                MessageBox.Show("BRAK SPOJNOŚCI W WYBRANYCH JEDNOSTKACH!");
            }
            else
            {
               if( Wspolnota.listaJednostki_s.Count > 0){

         

                    int nrPierwszej = 1;
                    try
                    {
                        Wspolnota.zaladujDaneDoTworzeniaJednostek();
                        /////////////////
                        ///
                        //dgDopasujDoIstniejacych.DataContext = ListTmpCls;
                 
                        ////////////////
                        Console.WriteLine(">" + textBoxNrPierwszejJednostki.Text + "<");
                        if(textBoxNrPierwszejJednostki.Text.Trim() != "")
                        {
                            nrPierwszej = Convert.ToInt32(textBoxNrPierwszejJednostki.Text.Trim());
                        }
                    }
                    catch (Exception ec)
                    {
                        MessageBox.Show("Błędny format numeru pierwszej jednostki\n" + ec.Message);
                        goto koniec;
                    }

            
                     Wspolnota.TworzenieJednostek(nrPierwszej, Wspolnota.listGminy[listBoxGm.SelectedIndex].ID_ID, Wspolnota.listObreby[listBoxObreby.SelectedIndex].IdObrebu, Wspolnota.listaRWD[comboRWD.SelectedIndex].ID_ID , comboGrupa.SelectionBoxItem.ToString());
                     //Wspolnota.SprawdzCzyMoznaDopisacDoIsniejacej();
                }
            }
            MenuItemPolaczZbaza_Click(sender, e);
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
