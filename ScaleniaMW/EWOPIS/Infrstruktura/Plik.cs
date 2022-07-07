﻿using ScaleniaMW.EWOPIS.Widoki;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace ScaleniaMW.EWOPIS.Infrstruktura
{
    public static class Plik
    {
        public static Window windowDzialkiDoEwopis;

        public static string PobierzSciezke()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (!(Properties.Settings.Default.EwopisSciezkaFDB.Equals("") || Properties.Settings.Default.EwopisSciezkaFDB.Equals(null)))
            {
                dlg.InitialDirectory = Properties.Settings.Default.EwopisSciezkaFDB.ToString().Substring(0, Properties.Settings.Default.EwopisSciezkaFDB.LastIndexOf("\\"));
            }
            dlg.DefaultExt = ".fdb";
            dlg.Filter = "All files(*.*) | *.*|FDB Files (*.fdb)|*.fdb";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    ustawProperties(dlg.FileName);
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
            return dlg.FileName;
        }

        public static void OdswierzScierzkeWPasku()
        {
            windowDzialkiDoEwopis.Title = "EWOPISMW - inż. Marek Wojciechowicz " + Properties.Settings.Default.EwopisSciezkaFDB;
        }

        public static void ustawProperties(string FileName)
        {
            Properties.Settings.Default.EwopisSciezkaFDB = FileName;
            Properties.Settings.Default.Save();
            OdswierzScierzkeWPasku();
        }

        public static List<string> OtworzPlik()
        {
            List<string> calyOdczzytanyTextLinie = new List<string>();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                calyOdczzytanyTextLinie = odczytZPlikuLinie(dlg.FileName).ToList();
            }
            return calyOdczzytanyTextLinie;
        }

        private static string[] odczytZPlikuLinie(string a) //odczyt z pliku z wyjatkami niepowodzenia należy podać ścieżkę, zwraca tablicę odczytaną z pliku
        {
            string[] all = null;
            //string[] lines = null;
            try
            {
                all = System.IO.File.ReadAllLines(a, Encoding.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine("Do dupy: {0}", e.Message);
                MessageBox.Show("Błąd odcztu pliku txt lub csv.\nUpewnij się, że plik, \nktóry chcesz otworzyć jest zamknięty!", "ERROR", MessageBoxButton.OK);
            }
            return all;
        }

        public static List<EWOPIS.Modele.ModelDzialkiParametry> odczytajParametry()
        {
            List<Modele.ModelDzialkiParametry> modelDzialkiParametries = new List<Modele.ModelDzialkiParametry>();
            List<string> listaParametrów = new List<string>();
            var item = OtworzPlik();
            try
            {
                for (int i = 0; i < item.Count; i++)
                {
                    modelDzialkiParametries.Add(new Modele.ModelDzialkiParametry()
                    {
                        IdObr = Convert.ToInt32(item[i].Split('\t').ToList()[0]),
                        Nrdz = item[i].Split('\t').ToList()[1],
                        NrJednRej = Convert.ToInt32(item[i].Split('\t').ToList()[2])
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return modelDzialkiParametries;
        }
        //koniec class

        public static void Save(string contentToWrite)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                DefaultExt = "txt",
                Filter = "TXT (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if ((bool)sfd.ShowDialog())
            {
                File.WriteAllText(sfd.FileName, contentToWrite, Encoding.Default);
            }
        }
    }
}
