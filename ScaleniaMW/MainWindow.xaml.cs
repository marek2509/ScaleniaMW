using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScaleniaMW
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static FbConnection connection = new FbConnection(@"User=SYSDBA;Password=wbg;Database= C:\Users\User\source\repos\ScaleniaMW\ScaleniaMW\bin\Debug\PLANTA I INNE.FDB;" + "DataSource=localhost; Port=3050;Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                           "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;");
       
        public MainWindow()
        {
          

            DataTable dt = null;
            connection.Open();
            FbCommand command = new FbCommand();
           
            FbTransaction transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            command.CommandText = "select obreby.id || '-' || dzialka.idd as NR_DZ, case WHEN JEDN_REJ.nkr is null then obreby.id * 1000 + JEDN_REJ.grp else JEDN_REJ.nkr end as NKR_Z_GRUPAMI from DZIALKA left outer join OBREBY on dzialka.idobr = OBREBY.id_id left outer join JEDN_REJ on dzialka.rjdr = JEDN_REJ.id_id order by NKR_Z_GRUPAMI";
            
            FbDataAdapter adapter = new FbDataAdapter(command);
            dt = new DataTable();
            //  adapter.Fill(dt);
            adapter.Fill(dt);
            foreach (var item in dt.Columns)
            {

                Console.Write(item + " ");
            }
           Console.WriteLine();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    Console.Write(dt.Rows[i][j] + " \t");
                }
                Console.WriteLine();


                //Console.WriteLine(i+ " " + dt.Rows[i][0]); // "" + dt.Rows[i][1] + " " );

            }



            connection.Close();
          
            InitializeComponent();
        }

        public static string[] odczytZPlikuLinie(string a) //odczyt z pliku z wyjatkami niepowodzenia należy podać ścieżkę, zwraca tablicę odczytaną z pliku
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

        public static string[] pobranieWartoscZLinii(string LiniaTekstu)
        {
            char[] charSeparators = new char[] { '\t', ' ' };
            string[] wartosciZlini = LiniaTekstu.Trim().Split(charSeparators);
            return wartosciZlini;
        }


        List<Punkt> listaPunktów = new List<Punkt>();
        private void otworzEDZ(object sender, RoutedEventArgs e)
        {
            listaPunktów = new List<Punkt>();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".edz";
            dlg.Filter = "All files(*.*) | *.*|TXT Files (*.txt)|*.txt| CSV(*.csv)|*.csv| EDZ(*.edz)|*.edz";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                   string[] calyOdczzytanyTextLinie = odczytZPlikuLinie(dlg.FileName);

                    List<string> listaBezPustychLinii = new List<string>();
                    for (int i = 0; i < calyOdczzytanyTextLinie.Length; i++)
                    {
                       if(calyOdczzytanyTextLinie[i].Trim() == "" || calyOdczzytanyTextLinie[i].Trim().Equals(null))
                       {
                            continue;
                       }
                        else
                        {
                            listaBezPustychLinii.Add(calyOdczzytanyTextLinie[i].Trim().Replace(".", ","));
                        }
                    }

                    int przekierownik = 0;
                    for (int i = 0; i < listaBezPustychLinii.Count; i++)
                    {
                        string[] wartosciZlini = pobranieWartoscZLinii(listaBezPustychLinii[i]);
                        List<string> listTmp = new List<string>();
                        foreach (var item in wartosciZlini)
                        {
                            if (item.Trim().Equals("") || item.Trim().Equals(null))
                            {
                                continue;
                            }
                            else
                            {
                                listTmp.Add(item.Trim()); 
                            }
                        }

                        if (przekierownik>8 && listTmp.Count.Equals(1))
                        {
                            przekierownik = 0; Console.WriteLine("ustaw 0");
                        }

                        przekierownik += listTmp.Count;
                        if(przekierownik==1)
                        {
                            listaPunktów.Add(new Punkt() { NazwaDz = listTmp[0] });
                        }else if(przekierownik == 7)
                        {
                            listaPunktów[listaPunktów.Count - 1].DzX1 = float.Parse(listTmp[0], CultureInfo.InvariantCulture); 
                            listaPunktów[listaPunktów.Count - 1].DzY1= float.Parse(listTmp[1], CultureInfo.InvariantCulture); 
                        }
                        else if(przekierownik == 8)
                        {
                            listaPunktów[listaPunktów.Count - 1].ilePktow = int.Parse(listTmp[0], CultureInfo.InvariantCulture);
                        }
                        else if (przekierownik > 8)
                        {
                            listaPunktów[listaPunktów.Count - 1].listaWspPktu.Add(new Punkt.WspPktu() { NR = listTmp[0], X = float.Parse(listTmp[1], CultureInfo.InvariantCulture), Y = float.Parse(listTmp[2], CultureInfo.InvariantCulture) });
                        }

                    }


                    foreach (var item in listaPunktów)
                    {
                        Console.WriteLine(item.NazwaDz + " " + item.DzX1 + " " + item.DzY1 + " ile: " + item.ilePktow);
                        foreach (var items in listaPunktów[listaPunktów.Count - 1].listaWspPktu)
                        {
                            Console.WriteLine( "NRP " + items.NR + " PX " + items.X + " PY " + items.Y);
                        }
                     
                    }

                }
                catch (Exception esa)
                {
                    var resultat = MessageBox.Show(esa.ToString() + " Przerwać?", "ERROR", MessageBoxButton.YesNo);

                    if (resultat == MessageBoxResult.Yes)
                    {
                        Application.Current.Shutdown();
                    }

                    Console.WriteLine(esa + "Błędny format importu działek");
                }
            }
        }

        private void ZapiszDoPliku(object sender, RoutedEventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = ".txt";
            svd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (svd.ShowDialog() == true)
            {
                using (Stream s = File.Open(svd.FileName, FileMode.Create))
                //  using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                    try
                    {
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var item in listaPunktów)
                            {
                                sb.AppendLine(item.NazwaDz + "\t" + item.podajeKatUstawienia());
                            }

                            sw.Write(sb.ToString());
                            sw.Close();
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.ToString() + "  problem z plikiem");
                        }
                    }
                    catch (Exception ex)
                    {
                        var resultat = MessageBox.Show(ex.ToString() + " Przerwać?", "ERROR", MessageBoxButton.YesNo);

                        if (resultat == MessageBoxResult.Yes)
                        {
                            Application.Current.Shutdown();
                        }
                    }

            }
        }
    }
}
