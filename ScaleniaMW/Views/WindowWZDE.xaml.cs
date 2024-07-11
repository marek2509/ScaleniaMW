using ScaleniaMW.Entities;
using ScaleniaMW.Helpers;
using ScaleniaMW.Repositories;
using ScaleniaMW.Repositories.Interfaces;
using ScaleniaMW.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ScaleniaMW.Views
{
    /// <summary>
    /// Logika interakcji dla klasy WindowWZDE.xaml
    /// </summary>
    public partial class WindowWZDE : Window
    {
        MainDbContext dbContext;
        WZDEService _WzdeService;
        List<string> KWList = new List<string>();
        DzialkaRepository _dzialkaRepository;
        Dzialki_NRepository _dzialki_NRepository;
        public WindowWZDE()
        {
            InitializeComponent();

            dbContext = new MainDbContext(ConnectionHelper.GetConnectionString());
            if (dbContext != null)
            {
                _WzdeService = new WZDEService(dbContext);
                _dzialkaRepository = new DzialkaRepository(dbContext);
                _dzialkaRepository = new DzialkaRepository(dbContext);
                _dzialki_NRepository = new Dzialki_NRepository(dbContext);

                List<Dzialka> currentParcelBefor = _dzialkaRepository.GetAll().ToList();
                List<Dzialki_n> currentParcelAfter = _dzialki_NRepository.GetAll().ToList();
                KWList.AddRange(currentParcelBefor.Where(x => !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
                KWList.AddRange(currentParcelAfter.Where(x => !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
                KWList = KWList.Distinct().ToList();
                listBoxKW.ItemsSource = KWList;
                if (listBoxKW.Items.Count > 0)
                {
                    listBoxKW.SelectedIndex = 0;
                }

                //listBoxDzialki.ItemsSource = _dzialkaRepository.GetAll(x => x.KW == currentParcel[listBoxNkr.SelectedIndex].KW).Select(x => $"{x.Obreb.ID}-{x.IDD}");

            }
        }

        private void SetConnectionString_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ConnectionHelper.SetConnectionStringByFileDialog();
        }

        private void SetLoginAndPassword_Click(object sender, RoutedEventArgs e)
        {
            Helpers.ConnectionHelper.SetLoginAndPassword();
        }

        private void CheckBoxTopmost_Unchecked(object sender, RoutedEventArgs e)
        {
            windowWZDE.Topmost = false;
        }

        private void CheckBoxTopmost_Checked(object sender, RoutedEventArgs e)
        {
            windowWZDE.Topmost = true;
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowWZDE.Close();
        }

        private void ListBoxNkr_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (KWList.Any())
            {
                var currentKW = KWList[listBoxKW.SelectedIndex];
                var allParcelForKW = _dzialkaRepository.GetAll(x => x.KW.Trim() == currentKW);
                listBoxDzialki.ItemsSource = allParcelForKW.Select(x => $"{x.Obreb?.ID}-{x.IDD}").ToList();

                var allParcelForKWAfter = _dzialki_NRepository.GetAll(x => x.KW.Trim() == currentKW);
                listBoxDzialkiN.ItemsSource = allParcelForKWAfter.Select(x => $"{x.Obreb?.ID}-{x.IDD}").ToList();
            }
        }
    }
}