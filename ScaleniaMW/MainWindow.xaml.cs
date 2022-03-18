using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Deployment;
using System.Windows.Threading;
using System.Threading;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        static Button btNKR_KW;
        static Button btNowychDzialek;
        static Button btRejGR;
        static Button btStanPo;
        static Button btPorownaniePrzedPo;
        static Button btModyfDokumentow;

        public MainWindow()
        {
            InitializeComponent();
            // Console.WriteLine("Assembly.GetExecutingAssembly().GetName().Version.ToString():" +Assembly.GetExecutingAssembly().GetName().Version.ToString());
            try
            {
                Console.WriteLine("ASSMBLY VERSJA: " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
                windowScaleniaMW.Title += " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                btNKR_KW = buttonRodzajPracyNKR_KW;
                btNowychDzialek = buttonPrzypiszKwDlaNowychDzialek;
                btRejGR = buttonRodzajPracyPrzypisanieRejGr;
                btStanPo = buttonRodzPracyKWnaMapeStanPO;
                btPorownaniePrzedPo = buttonPorownanieStanuPrzedIPo;
                btModyfDokumentow = buttonModyfikacjaDokumentow;

                Constants.PortFB = Properties.Settings.Default.FBPort;
                if (Constants.PortFB == 3050)
                {
                    FbPort3051.IsChecked = false;
                    FbPort3050.IsChecked = true;
                }
                else
                {
                    FbPort3051.IsChecked = true;
                    FbPort3050.IsChecked = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + " problem z oknem");
            }

            if (Environment.MachineName.ToString().Trim().Contains("WBG-II-151")) Email.CzyJuzWysylano = true;

            Console.WriteLine("Czy juz wyslano: " + Email.CzyJuzWysylano);
            if (!Email.CzyJuzWysylano) // okodowac
            {

                Email.CzyJuzWysylano = true;
                try
                {

                    Email.przypiszIP();
                    Console.WriteLine("przypisz ip");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("error przypisz ip");
                }

                try
                {
                    Console.WriteLine("czy wyslano: " + Email.SendEmail("SCALENAMW", "Właśnie użyto programu SCALENIAMW\n\nWersja programu: " + Assembly.GetExecutingAssembly().GetName().Version.ToString(), "SCALENIA_MW"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    AktualizacjaOprogramowania.aktualizuj();
                }
                catch
                {

                }
            }

            try
            {
                if (AktualizacjaOprogramowania.CzyBlokowacProgram() == true)
                {
                    Properties.Settings.Default.CzyBlokowacProgram = true;
                    Properties.Settings.Default.Save();
                    windowScaleniaMW.IsEnabled = false;
                    MessageBox.Show("Aby rozwiązać problem z działaniem programu skontaktuj się z autorem pod adresem email:\nmarek.wojciechowicz25@gmail.com", "Brak dostępu", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (AktualizacjaOprogramowania.CzyBlokowacProgram() == false)
                {
                    Properties.Settings.Default.CzyBlokowacProgram = false;
                    Properties.Settings.Default.Save();
                    windowScaleniaMW.IsEnabled = true;
                }
                else
                {
                    if (Properties.Settings.Default.CzyBlokowacProgram == true)
                    {
                        MessageBox.Show("Aby rozwiązać problem z działaniem programu skontaktuj się z autorem pod adresem email:\nmarek.wojciechowicz25@gmail.com", "Brak dostępu", MessageBoxButton.OK, MessageBoxImage.Information);
                        windowScaleniaMW.IsEnabled = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("excep : " + e.Message);
            }
        }

        List<DzialkaEDZ> listaZEDZ = new List<DzialkaEDZ>();
        List<DzialkaNkrZSQL> listaDzNkrzSQL = new List<DzialkaNkrZSQL>();
        //DataTable dt;
        public static string connectionString;

        public static void aktualizujSciezkeZPropertis()
        {
            connectionString = @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        private void ButtonRodzajPracyNKR_KW_Click(object sender, RoutedEventArgs e)
        {
            WindowNkrKwObrot windowNkrKW = new WindowNkrKwObrot();
            windowNkrKW.Show();
            windowScaleniaMW.Close();
        }

        private void ButtonRodzajPracyPrzypisanieRejGr_Click(object sender, RoutedEventArgs e)
        {
            WindowPrzypiszRejGr windowPrzypiszRejGr = new WindowPrzypiszRejGr();
            windowPrzypiszRejGr.Show();
            windowScaleniaMW.Close();
        }

        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonRodzPracyPrzypiszKwDlaNowychDzialek_Click(object sender, RoutedEventArgs e)
        {
            WindowPrzypiszKW windowPrzypiszKW = new WindowPrzypiszKW();
            windowPrzypiszKW.Show();
            windowScaleniaMW.Close();
        }

        private void ButtonRodzPracyKWnaMapeStanPO_Click(object sender, RoutedEventArgs e)
        {
            WindowKwNaMapePO windowKwNaMapePO = new WindowKwNaMapePO();
            windowKwNaMapePO.Show();
            windowScaleniaMW.Close();
        }

        private void ButtonRodzajPracyPrzypisanieRejGr_MouseLeave(object sender, MouseEventArgs e)
        {
            imgNrJRprzypisz.Visibility = Visibility.Hidden;
            label1RodzajPracyPrzypisanieRejGr.Foreground = Brushes.White;
            for (int i = 0; i < 25; i++)
            {
                btRejGR.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonRodzajPracyPrzypisanieRejGr), DispatcherPriority.Background);
            }
        }

 

        private void ButtonPrzypiszKwDlaNowychDzialek_MouseLeave(object sender, MouseEventArgs e)
        {
            imgKWprzypisz.Visibility = Visibility.Hidden;
            label1PrzypiszKwDlaNowychDzialek.Foreground = Brushes.White;
            for (int i = 0; i < 25; i++)
            {
                btNowychDzialek.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonPrzypiszKwDlaNowychDzialek), DispatcherPriority.Background);
            }
        }

        private void ButtonRodzPracyKWnaMapeStanPO_MouseLeave(object sender, MouseEventArgs e)
        {
            imgKWpNaMape.Visibility = Visibility.Hidden;
            label1RodzPracyKWnaMapeStanPO.Foreground = Brushes.White;
            for (int i = 0; i < 25; i++)
            {
                btStanPo.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonRodzPracyKWnaMapeStanPO), DispatcherPriority.Background);
            }
        }

        private void ButtonRodzPracyKWnaMapeStanPO_MouseEnter(object sender, MouseEventArgs e)
        {
            imgKWpNaMape.Visibility = Visibility.Visible;
            label1RodzPracyKWnaMapeStanPO.Foreground = Brushes.Black;
            for (int i = 0; i < 25; i++)
            {
                btStanPo.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonRodzPracyKWnaMapeStanPO), DispatcherPriority.Background);
            }
        }

        private void ButtonPrzypiszKwDlaNowychDzialek_MouseEnter(object sender, MouseEventArgs e)
        {
            imgKWprzypisz.Visibility = Visibility.Visible;
            label1PrzypiszKwDlaNowychDzialek.Foreground = Brushes.Black;
            for (int i = 0; i < 25; i++)
            {
                btNowychDzialek.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonPrzypiszKwDlaNowychDzialek), DispatcherPriority.Background);
            }
        }

        private void ButtonRodzajPracyPrzypisanieRejGr_MouseEnter(object sender, MouseEventArgs e)
        {
            imgNrJRprzypisz.Visibility = Visibility.Visible;
            label1RodzajPracyPrzypisanieRejGr.Foreground = Brushes.Black;
            for (int i = 0; i < 25; i++)
            {
                btRejGR.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonRodzajPracyPrzypisanieRejGr), DispatcherPriority.Background);
            }
        }



        private void ButtonRodzajPracyNKR_KW_MouseLeave(object sender, MouseEventArgs e)
        {
            labelNkrKwOdch.Visibility = Visibility.Hidden;
            imgNKR.Visibility = Visibility.Hidden;
            label1RodzajPracyNKR_KW.Foreground = Brushes.White;
            for (int i = 0; i < 25; i++)
            {
                btNKR_KW.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonRodzajPracyNKR_KW), DispatcherPriority.Background);
            }
        }
        private void ButtonRodzajPracyNKR_KW_MouseEnter(object sender, MouseEventArgs e)
        {
            labelNkrKwOdch.Visibility = Visibility.Visible;
            imgNKR.Visibility = Visibility.Visible;
            label1RodzajPracyNKR_KW.Foreground = Brushes.Black;
            for (int i = 0; i < 25; i++)
            {
                btNKR_KW.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonRodzajPracyNKR_KW), DispatcherPriority.Background);
            }
        }

        private void UpdateProgressbuttonRodzajPracyNKR_KW()
        {
            btNKR_KW.Width += 0.4;
            if (btNKR_KW.ActualWidth > 275)
                btNKR_KW.Width = 250;
        }

        private void UpdateRegresbuttonRodzajPracyNKR_KW()
        {
            btNKR_KW.Width -= 0.4;
            if (btNKR_KW.ActualWidth < 250)
                btNKR_KW.Width = 250;
        }

        private void UpdateProgressbuttonRodzajPracyPrzypisanieRejGr()
        {
            btRejGR.Width += 0.4;
            if (btRejGR.ActualWidth > 275)
                btRejGR.Width = 250;
        }

        private void UpdateRegresbuttonRodzajPracyPrzypisanieRejGr()
        {
            btRejGR.Width -= 0.4;
            if (btRejGR.ActualWidth < 250)
                btRejGR.Width = 250;
        }
        /// <summary>
        /// //////////////////////////
        /// </summary>
        private void UpdateProgressbuttonPrzypiszKwDlaNowychDzialek()
        {
            btNowychDzialek.Width += 0.4;
            if (btNowychDzialek.ActualWidth > 275)
                btNowychDzialek.Width = 250;
        }

        private void UpdateRegresbuttonPrzypiszKwDlaNowychDzialek()
        {
            btNowychDzialek.Width -= 0.4;
            if (btNowychDzialek.ActualWidth < 250)
                btNowychDzialek.Width = 250;
        }

        private void UpdateProgressbuttonRodzPracyKWnaMapeStanPO()
        {
            btStanPo.Width += 0.4;
            if (btStanPo.ActualWidth > 275)
                btStanPo.Width = 250;
        }

        private void UpdateRegresbuttonRodzPracyKWnaMapeStanPO()
        {
            btStanPo.Width -= 0.4;
            if (btStanPo.ActualWidth < 250)
                btStanPo.Width = 250;
        }

        private void UpdateProgressbuttonPorownaniePrzedPo()
        {
            btPorownaniePrzedPo.Width += 0.4;
            if (btPorownaniePrzedPo.ActualWidth > 275)
                btPorownaniePrzedPo.Width = 250;
        }

        private void UpdateRegresbuttonPorownaniePrzedPo()
        {
            btPorownaniePrzedPo.Width -= 0.4;
            if (btPorownaniePrzedPo.ActualWidth < 250)
                btPorownaniePrzedPo.Width = 250;
        }

        private void UpdateProgressbuttonModyfDokum()
        {
            btModyfDokumentow.Width += 0.4;
            if (btModyfDokumentow.ActualWidth > 275)
                btModyfDokumentow.Width = 250;
        }

        private void UpdateRegresbuttonModyfDokum()
        {
            btModyfDokumentow.Width -= 0.4;
            if (btModyfDokumentow.ActualWidth < 250)
                btModyfDokumentow.Width = 250;
        }

        private void UpdateProgressbuttonKontrolaBazy()
        {
            buttonKontrolaBazy.Width += 0.4;
            if (buttonKontrolaBazy.ActualWidth > 275)
                buttonKontrolaBazy.Width = 250;
        }

        private void UpdateRegresbuttonKontrolaBazy()
        {
            buttonKontrolaBazy.Width -= 0.4;
            if (buttonKontrolaBazy.ActualWidth < 250)
                buttonKontrolaBazy.Width = 250;
        }

        private delegate void ProgressBarDelegate();

        private void PorownanieStanuPrzedIPo_MouseLeave(object sender, MouseEventArgs e)
        {
            labelPorownanieWartPrzedPo.Visibility = Visibility.Hidden;
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/wagaWhite.png", UriKind.Relative);
            bi3.EndInit();
            imagePorownanieWaga.Source = bi3;

            labelPorownaniePrzePo.Foreground = Brushes.White;
            for (int i = 0; i < 25; i++)
            {
                btPorownaniePrzedPo.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonPorownaniePrzedPo), DispatcherPriority.Background);
            }
        }

        private void PorownanieStanuPrzedIPo_MouseEnter(object sender, MouseEventArgs e)
        {
            labelPorownanieWartPrzedPo.Visibility = Visibility.Visible;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/waga.png", UriKind.Relative);
            bi3.EndInit();
            imagePorownanieWaga.Source = bi3;
            labelPorownaniePrzePo.Foreground = Brushes.Black;

            for (int i = 0; i < 25; i++)
            {
                btPorownaniePrzedPo.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonPorownaniePrzedPo), DispatcherPriority.Background);
            }


        }

        private void PorownanieStanuPrzedIPo_Click(object sender, RoutedEventArgs e)
        {
            WindowPorownajPrzedPo windowPorownajPrzedPo = new WindowPorownajPrzedPo();
            windowPorownajPrzedPo.Show();
            windowScaleniaMW.Close();
        }

        private void ModyfDokum_MouseLeave(object sender, MouseEventArgs e)
        {
            labelEdycjaDpkumentów.Visibility = Visibility.Hidden;
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/writingWhite.png", UriKind.Relative);
            bi3.EndInit();
            imageWritiing.Source = bi3;
            imgEdycDokum.Visibility = Visibility.Hidden;
            labelModyfDokumentow.Foreground = Brushes.White;

            for (int i = 0; i < 25; i++)
            {
                btModyfDokumentow.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonModyfDokum), DispatcherPriority.Background);
            }
        }

        private void ModyfDokum_MouseEnter(object sender, MouseEventArgs e)
        {
            labelEdycjaDpkumentów.Visibility = Visibility.Visible;
            imgEdycDokum.Visibility = Visibility.Visible;
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/writing.png", UriKind.Relative);
            bi3.EndInit();
            imageWritiing.Source = bi3;
            labelModyfDokumentow.Foreground = Brushes.Black;

            for (int i = 0; i < 25; i++)
            {
                btModyfDokumentow.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonModyfDokum), DispatcherPriority.Background);
            }
        }

        private void ButtonModyfikacjaDokumentow_Click(object sender, RoutedEventArgs e)
        {
            WindowEdycjaDokumentow windidEdycjaDok = new WindowEdycjaDokumentow();
            windidEdycjaDok.Show();
            windowScaleniaMW.Close();
        }

        private void ButtonKontrolaBazy_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/controlBlack.ico", UriKind.Relative);
            bi3.EndInit();
            imageKontrolaBazy.Source = bi3;
            labelKontrolaBazy.Foreground = Brushes.Black;

            for (int i = 0; i < 25; i++)
            {
                buttonKontrolaBazy.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonKontrolaBazy), DispatcherPriority.Background);
            }
        }

        private void ButtonKontrolaBazy_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/controlWhite.png", UriKind.Relative);
            bi3.EndInit();
            imageKontrolaBazy.Source = bi3;
            labelKontrolaBazy.Foreground = Brushes.White;

            for (int i = 0; i < 25; i++)
            {
                btModyfDokumentow.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonKontrolaBazy), DispatcherPriority.Background);
            }
        }

        private void ButtonKontrolaBazy_Click(object sender, RoutedEventArgs e)
        {
            WindowKontrolaBazy windidKontrolaBazy = new WindowKontrolaBazy();
            windidKontrolaBazy.Show();
            windowScaleniaMW.Close();
        }


        private void FbPort3051FB2_5_Checked(object sender, RoutedEventArgs e)
        {
            Constants.PortFB = (int)Constants.PortyFB.FB2_5;
            Properties.Settings.Default.FBPort = Constants.PortFB;
            FbPort3051.IsChecked = true;
            FbPort3050.IsChecked = false;
            Properties.Settings.Default.Save();
        }

        private void FbPort3050FB3_0_Checked(object sender, RoutedEventArgs e)
        {
            Constants.PortFB = (int)Constants.PortyFB.FB3_0;
            Properties.Settings.Default.FBPort = Constants.PortFB;
            FbPort3051.IsChecked = false;
            FbPort3050.IsChecked = true;
            Properties.Settings.Default.Save();
        }

        private void ButtonWspolnota_Click(object sender, RoutedEventArgs e)
        {
            WindowPodzialWspolnoty windowPodzWsp = new WindowPodzialWspolnoty();
            windowPodzWsp.Show();
            windowScaleniaMW.Close();
        }


        private void ButtonWspolnota_MouseEnter(object sender, MouseEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/wspolnota.png", UriKind.Relative);
            bi3.EndInit();
            imageWspolnota.Source = bi3;
            labelPodzialWspolnoty.Foreground = Brushes.Black;

            for (int i = 0; i < 25; i++)
            {
                buttonWspolnota.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateProgressbuttonWspolnota), DispatcherPriority.Background);
            }
        }

        private void ButtonWspolnota_MouseLeave(object sender, MouseEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Resources/wspolnotaWhite.png", UriKind.Relative);
            bi3.EndInit();
            imageWspolnota.Source = bi3;
            labelPodzialWspolnoty.Foreground = Brushes.White;

            for (int i = 0; i < 25; i++)
            {
                buttonWspolnota.Dispatcher.BeginInvoke(new ProgressBarDelegate(UpdateRegresbuttonWspolnota), DispatcherPriority.Background);
            }
        }

        private void UpdateProgressbuttonWspolnota()
        {
            buttonWspolnota.Width += 0.4;
            if (buttonWspolnota.ActualWidth > 275)
                buttonWspolnota.Width = 250;
        }

        private void UpdateRegresbuttonWspolnota()
        {
            buttonWspolnota.Width -= 0.4;
            if (buttonWspolnota.ActualWidth < 250)
                buttonWspolnota.Width = 250;
        }

        private void Ewopis_Click(object sender, RoutedEventArgs e)
        {
            EWOPIS.EwopisMainWindow ewopisMainWindow = new EWOPIS.EwopisMainWindow();
            ewopisMainWindow.Show();
            windowScaleniaMW.Close();

        }
    }
}
