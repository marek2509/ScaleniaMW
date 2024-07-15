using ScaleniaMW.Entities;
using ScaleniaMW.Helpers;
using ScaleniaMW.Repositories;
using ScaleniaMW.Repositories.Interfaces;
using ScaleniaMW.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        string currentDocumentToDownload = string.Empty;
        public WindowWZDE()
        {
            InitializeComponent();

            dbContext = new MainDbContext(ConnectionHelper.GetConnectionString());
            if (dbContext != null)
            {
                _WzdeService = new WZDEService(dbContext);
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

                listBoxDzialkiNieUjawnione.ItemsSource = currentParcelBefor.Where(x => string.IsNullOrWhiteSpace(x.KW)).Select(x => $"{x?.Obreb.ID}-{x.IDD}").ToList();
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


                HTMLGenerator.WzdeStep1TrDzialkiNieUjawnione(allParcelForKW.ToList());

                var zmapowaneDzialkiPo = allParcelForKWAfter.Select(x => new Dzialka { Obreb = x.Obreb, IDD = x.IDD, PEW = x.PEW, KW = x.KW});


                HTMLGenerator.WzdeStep2TrKW(zmapowaneDzialkiPo.ToList());
                var leftTable = HTMLGenerator.WzdeStep3GetTable();

                HTMLGenerator.WzdeStep1TrDzialkiNieUjawnione(allParcelForKW.ToList());
                HTMLGenerator.WzdeStep2TrKW(zmapowaneDzialkiPo.Take(2).ToList());
                var rightTable = HTMLGenerator.WzdeStep3GetTable();
                var table = HTMLGenerator.WzdeStep4InsertTablesIntoPage(leftTable, rightTable);
                currentDocumentToDownload = table;

                table = table.Replace("font-size: 13", "font-size: 18");
                table = table.Replace("font-size: 16", "font-size: 22");
                table = table.Replace("windows-1250", "UTF-8");
                webBrowser.NavigateToString(table);

            }
        }

        private void ButtonDownloadCurrentDocument_Click(object sender, RoutedEventArgs e)
        {
            Plik.ZapiszDoPlikuTXT(currentDocumentToDownload, "doc");
        }
    }
}