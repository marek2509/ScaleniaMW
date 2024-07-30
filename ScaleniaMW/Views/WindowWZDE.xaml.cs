﻿using ScaleniaMW.Entities;
using ScaleniaMW.Helpers;
using ScaleniaMW.Repositories;
using ScaleniaMW.Repositories.Interfaces;
using ScaleniaMW.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;

namespace ScaleniaMW.Views
{
    /// <summary>
    /// Logika interakcji dla klasy WindowWZDE.xaml
    /// </summary>
    public partial class WindowWZDE : Window
    {
        MainDbContext dbContext;
        WZDEService _WzdeService;
        DzialkaRepository _dzialkaRepository;
        Dzialki_NRepository _dzialki_NRepository;
        WZDEDzKWRepository _WZDEDzKWRepository;
        Jedn_rejRepository _jedn_RejRepository;
        List<string> KWList = new List<string>();
        string currentDocumentToDownload = string.Empty;
        string currentKW = string.Empty;
        int dzialkiNieUjawnioneCurrentIdx = 0;
        List<Dzialka> dzialkiNieUjawnione = new List<Dzialka>();


        public WindowWZDE()
        {
            InitializeComponent();
            init();

        }

        private void init()
        {
            try
            {
                dbContext = new MainDbContext(ConnectionHelper.GetConnectionString());
                // create new table if not exist for insert not assigne Dzliaka
                // integer generated by default as identity primary key
                dbContext.Database.ExecuteSqlCommand("EXECUTE BLOCK AS BEGIN If (not exists(select 1 from rdb$relations where rdb$relation_name = 'WZDEDZKW')) then execute statement 'create table WZDEDZKW (ID int not null primary key, DZIALKAID_ID int not null, KW VARCHAR(15) not null, FOREIGN KEY (DZIALKAID_ID) REFERENCES DZIALKA(ID_ID), IsDeleted smallint not null);'; END");
                dbContext.Database.ExecuteSqlCommand("EXECUTE BLOCK AS BEGIN If (not exists(SELECT 1 FROM RDB$Generators WHERE RDB$GENERATOR_NAME = 'WZDEDZKW_ID_SEQUENCE')) THEN execute STATEMENT 'CREATE SEQUENCE WZDEDZKW_ID_sequence;'; END");
                dbContext.Database.ExecuteSqlCommand("EXECUTE BLOCK AS BEGIN If (not exists(SELECT 1 FROM RDB$TRIGGERS WHERE RDB$TRIGGER_NAME = 'WZDEDZKW_BI')) THEN  execute STATEMENT 'CREATE TRIGGER WZDEDZKW_BI FOR WZDEDZKW\tACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (NEW.ID is NULL) then NEW.ID = next value for WZDEDZKW_ID_sequence; END;'; END");

                _WzdeService = new WZDEService(dbContext);
                _dzialkaRepository = new DzialkaRepository(dbContext);
                _dzialki_NRepository = new Dzialki_NRepository(dbContext);
                _WZDEDzKWRepository = new WZDEDzKWRepository(dbContext);
                _jedn_RejRepository = new Jedn_rejRepository(dbContext);
                ResetDbData();

                labelConnection.Content = "Połączono";
                labelConnection.Foreground = Brushes.ForestGreen;
            }
            catch (Exception e)
            {
                labelConnection.Content = "Błąd połączenia";
                labelConnection.Foreground = Brushes.IndianRed;
            }
        }

        private void ResetDbData(bool loadKW = true)
        {
            try
            {
                List<Dzialka> currentParcelBefor = _dzialkaRepository.GetAll().ToList();
                List<Dzialki_n> currentParcelAfter = _dzialki_NRepository.GetAll().ToList();
                if (loadKW)
                {
                    KWList = new List<string>();
                    KWList.AddRange(currentParcelBefor.Where(x => !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
                    KWList.AddRange(currentParcelAfter.Where(x => !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
                    KWList = KWList.Distinct().OrderBy(x => x).ToList();
                    listBoxKW.ItemsSource = KWList;

                    if (listBoxKW.Items.Count > 0)
                    {
                        if (listBoxKW.SelectedIndex > 0 && listBoxKW.SelectedIndex < listBoxKW.Items.Count)
                        {
                            // zostaje obecny index
                        }
                        else
                        {
                            listBoxKW.SelectedIndex = 0;
                        }
                    }
                }

                var dzNieUjawnionePrzpypisane = _WZDEDzKWRepository.GetAll();
                dzialkiNieUjawnione = currentParcelBefor.Where(x => string.IsNullOrWhiteSpace(x.KW) && !dzNieUjawnionePrzpypisane.Any(y => y.DZIALKAID_ID == x.ID_ID)).OrderBy(x => x.Obreb.ID).ThenBy(x => x.SIDD).ToList();
                if (cbxDzZJrWybranejKW.IsChecked == true)
                {
                    var beforeRjdrs = currentParcelBefor.Where(x => !string.IsNullOrWhiteSpace(x.KW) && x.KW.Trim() == currentKW)?.Select(x => x.RJDR).ToList();
                    if (beforeRjdrs != null)
                    {
                        dzialkiNieUjawnione = dzialkiNieUjawnione?.Where(x => beforeRjdrs.Contains(x.RJDR)).ToList();
                    }
                }
                listBoxDzialkiNieUjawnione.ItemsSource = dzialkiNieUjawnione.Select(x => $"{x?.Obreb.ID}-{x.IDD}").ToList();
                listBoxDzialkiNieUjawnione.SelectedIndex = dzialkiNieUjawnioneCurrentIdx;
            }
            catch (Exception e)
            {
                labelConnection.Content = "Błąd połączenia";
                labelConnection.Foreground = Brushes.IndianRed;
            }
        }

        private void ReloadDzialkiNieujwnione()
        {
            try
            {
                if (_dzialkaRepository != null)
                {
                    List<Dzialka> currentParcelBefor = _dzialkaRepository.GetAll().ToList();
                    List<Dzialki_n> currentParcelAfter = _dzialki_NRepository.GetAll().ToList();

                    var dzNieUjawnionePrzpypisane = _WZDEDzKWRepository.GetAll();
                    dzialkiNieUjawnione = currentParcelBefor.Where(x => string.IsNullOrWhiteSpace(x.KW) && !dzNieUjawnionePrzpypisane.Any(y => y.DZIALKAID_ID == x.ID_ID)).OrderBy(x => x.Obreb.ID).ThenBy(x => x.SIDD).ToList();
                    if (cbxDzZJrWybranejKW.IsChecked == true)
                    {
                        var beforeRjdrs = currentParcelBefor.Where(x => !string.IsNullOrWhiteSpace(x.KW) && x.KW.Trim() == currentKW)?.Select(x => x.RJDR).ToList();
                        if (beforeRjdrs != null)
                        {
                            dzialkiNieUjawnione = dzialkiNieUjawnione?.Where(x => beforeRjdrs.Contains(x.RJDR)).ToList();
                        }
                    }
                    listBoxDzialkiNieUjawnione.ItemsSource = dzialkiNieUjawnione.Select(x => $"{x?.Obreb.ID}-{x.IDD}").ToList();
                    listBoxDzialkiNieUjawnione.SelectedIndex = dzialkiNieUjawnioneCurrentIdx;
                }
            }
            catch (Exception e)
            {
                labelConnection.Content = "Błąd połączenia";
                labelConnection.Foreground = Brushes.IndianRed;
            }
        }

        // KW z jednostek które maja nieujawnione działki
        private void FilterKWByCheckBox(bool kWTylkoZJednostekZNieprzypisanymiDzialkami)
        {
            List<Dzialka> currentParcelBefor = _dzialkaRepository.GetAll().ToList();
            KWList = new List<string>();
            if (kWTylkoZJednostekZNieprzypisanymiDzialkami)
            {
                var dzNieUjawnionePrzpypisane = _WZDEDzKWRepository.GetAll();
                var jrZDzialkamiNieujawnionymi = currentParcelBefor.Where(x => string.IsNullOrWhiteSpace(x.KW) && !dzNieUjawnionePrzpypisane.Any(y => y.DZIALKAID_ID == x.ID_ID)).Select(x => x.RJDR).ToList();
                KWList.AddRange(currentParcelBefor.Where(x => jrZDzialkamiNieujawnionymi.Contains(x.RJDR) && !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
            }
            else
            {
                List<Dzialki_n> currentParcelAfter = _dzialki_NRepository.GetAll().ToList();
                KWList.AddRange(currentParcelAfter.Where(x => !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
                KWList.AddRange(currentParcelBefor.Where(x => !string.IsNullOrWhiteSpace(x.KW)).Select(x => x.KW.Trim()).ToList());
            }


            KWList = KWList.Distinct().OrderBy(x => x).ToList();
            listBoxKW.ItemsSource = KWList;

            if (listBoxKW.Items.Count > 0)
            {
                if (listBoxKW.SelectedIndex > 0 && listBoxKW.SelectedIndex < listBoxKW.Items.Count)
                {
                    // zostaje obecny index
                }
                else
                {
                    listBoxKW.SelectedIndex = 0;
                }
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

        private void ListBoxKW_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (KWList.Any())
            {
                currentKW = KWList[listBoxKW.SelectedIndex < 0 ? 0 : listBoxKW.SelectedIndex];
                var allParcelForKW = _dzialkaRepository.GetAll(x => x.KW.Trim() == currentKW);

                var allParcelForKWAfter = _dzialki_NRepository.GetAll(x => x.KW.Trim() == currentKW);
                var nieujawnionePrzypisanDzialka = _WZDEDzKWRepository.GetAll(x => x.KW.Trim() == (currentKW)).ToList();
                var nieujawnionePrzypisanDzialkaID_ID = nieujawnionePrzypisanDzialka.Select(x => x.DZIALKAID_ID).ToList();
                var dzialkiNieUjawnioneRaport = _dzialkaRepository.GetAll(x => nieujawnionePrzypisanDzialkaID_ID.Contains(x.ID_ID)).ToList();
                HTMLGenerator.WzdeStep1TrDzialkiNieUjawnione(dzialkiNieUjawnioneRaport);
                HTMLGenerator.WzdeStep2TrKW(allParcelForKW.ToList());
                var leftTable = HTMLGenerator.WzdeStep3GetTable();

                // tabela prawa nie będzie miała nie ujawnionych działek
                var zmapowaneDzialkiPo = allParcelForKWAfter.Select(x => new Dzialka { Obreb = x.Obreb, IDD = x.IDD, PEW = x.PEW, KW = x.KW, SIDD = x.SIDD });
                HTMLGenerator.WzdeStep2TrKW(zmapowaneDzialkiPo.ToList());
                var rightTable = HTMLGenerator.WzdeStep3GetTable(true);

                var table = HTMLGenerator.WzdeStep4InsertTablesIntoPage(leftTable, rightTable);
                currentDocumentToDownload = table;

                table = table.Replace("font-size: 13", "font-size: 18");
                table = table.Replace("font-size: 16", "font-size: 22");
                table = table.Replace("windows-1250", "UTF-8");
                webBrowser.NavigateToString(table);

                labelCurrentKW.Content = currentKW;

                // sekcja z przypisanymi dzialkami
                containerDzialkieNieprzypisane.Children.Clear();
                RoutedEventHandler routedEventHandler = (s, es) =>
                {
                    //ResetDbData();
                    ListBoxKW_SelectionChanged(s, e);
                };
                foreach (var item in nieujawnionePrzypisanDzialka.OrderBy(x => x.Dzialka.Obreb.ID).ThenBy(x => x.Dzialka.SIDD))
                {
                    containerDzialkieNieprzypisane.Children.Add(WPFElementHelper.GetParcelWithDeleteBtn(item, _WZDEDzKWRepository, routedEventHandler));
                }

                StringBuilder sbKWAdditioinaInfo = new StringBuilder();
                foreach (var rjdr in allParcelForKW.Select(x => x.RJDR).Distinct())
                {
                    var firstOrDefaultDzialka = allParcelForKW.FirstOrDefault(x => x.RJDR == rjdr);
                    var ownerList = _jedn_RejRepository.GetOwnersForJR(firstOrDefaultDzialka.RJDR);
                    sbKWAdditioinaInfo.Append(HTMLGenerator.KWInfoData(firstOrDefaultDzialka, ownerList));
                }
                webBrowserInfKW.NavigateToString(sbKWAdditioinaInfo.ToString());

                ReloadDzialkiNieujwnione();

                if (dzialkiNieUjawnione.Count > 0)
                {
                    dzialkiNieUjawnioneCurrentIdx = dzialkiNieUjawnioneCurrentIdx < 0 ? 0 : dzialkiNieUjawnioneCurrentIdx;
                    listBoxDzialkiNieUjawnione.SelectedIndex = dzialkiNieUjawnioneCurrentIdx;
                    var kwListPowiazaneZDzialkamiNieujawnionymi = dzialkiNieUjawnione[dzialkiNieUjawnioneCurrentIdx]?.JednRej?.Dzialki?.Select(x => x.KW)?.Distinct()?.ToList();
                    if (kwListPowiazaneZDzialkamiNieujawnionymi.Any())
                    {
                        webBrowserPowiazaneKW.NavigateToString(HTMLGenerator.KWLista(kwListPowiazaneZDzialkamiNieujawnionymi));
                    }
                }
            }
        }

        private void ButtonDownloadCurrentDocument_Click(object sender, RoutedEventArgs e)
        {
            Plik.ZapiszDoPlikuTXT(currentDocumentToDownload, "doc");
        }

        private void ConnectDb_Click(object sender, RoutedEventArgs e)
        {
            init();
        }

        private void PrzypiszDzialke_Click(object sender, RoutedEventArgs e)
        {
            if (dzialkiNieUjawnioneCurrentIdx >= 0 && dzialkiNieUjawnioneCurrentIdx <= dzialkiNieUjawnione.Count && !string.IsNullOrWhiteSpace(currentKW))
            {
                var entity = new WZDEDzKW
                {
                    DZIALKAID_ID = dzialkiNieUjawnione[dzialkiNieUjawnioneCurrentIdx].ID_ID,
                    KW = currentKW
                };
                var isSaved = _WZDEDzKWRepository.Save(entity);
                if (isSaved)
                {
                    dzialkiNieUjawnione.RemoveAt(dzialkiNieUjawnioneCurrentIdx);
                    listBoxDzialkiNieUjawnione.ItemsSource = dzialkiNieUjawnione.Select(x => $"{x?.Obreb.ID}-{x.IDD}").ToList();
                    listBoxDzialkiNieUjawnione.SelectedIndex = dzialkiNieUjawnione.Count > dzialkiNieUjawnioneCurrentIdx ? dzialkiNieUjawnioneCurrentIdx : dzialkiNieUjawnione.Count - 1;
                    ListBoxKW_SelectionChanged(null, null);
                }
            }
        }

        private void listBoxDzialkiNieUjawnione_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dzialkiNieUjawnioneCurrentIdx = listBoxDzialkiNieUjawnione.SelectedIndex;
        }

        private void cbxDzZJrWybranejKW_Checked(object sender, RoutedEventArgs e)
        {
            ReloadDzialkiNieujwnione();
        }

        private void cbxDzZJrWybranejKW_Unchecked(object sender, RoutedEventArgs e)
        {
            ReloadDzialkiNieujwnione();
        }

        private void cbxKwKtoreMajaDzialki_Checked(object sender, RoutedEventArgs e)
        {
            FilterKWByCheckBox(true);
        }

        private void cbxKwKtoreMajaDzialki_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterKWByCheckBox(false);
        }
    }
}