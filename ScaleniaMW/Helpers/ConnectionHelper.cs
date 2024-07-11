using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ScaleniaMW.Helpers
{
    public static class ConnectionHelper
    {
        public static string GetConnectionString()
        {
            return @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource= localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                  "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        public static (bool status, string connectionString) SetConnectionStringByFileDialog()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                if (!(Properties.Settings.Default.PathFDB.Equals("") || Properties.Settings.Default.PathFDB.Equals(null)))
                {
                    dlg.InitialDirectory = Properties.Settings.Default.PathFDB.ToString().Substring(0, Properties.Settings.Default.PathFDB.LastIndexOf("\\"));
                }

                dlg.DefaultExt = ".fdb";
                dlg.Filter = "All files(*.*) | *.*|FDB (*.fdb)|*.fdb";
                Nullable<bool> resulDialog = dlg.ShowDialog();
                if (resulDialog == true)
                {
                    Properties.Settings.Default.PathFDB = dlg.FileName;
                    Properties.Settings.Default.Save();
                    return (status: true, connectionString: dlg.FileName);
                }

                return (status: false, connectionString: string.Empty);
            }
            catch (Exception esa)
            {
                var resultat = MessageBox.Show(esa.ToString() + " Przerwać?", "ERROR", MessageBoxButton.YesNo);

                if (resultat == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }

                Console.WriteLine(esa + "Błędny format importu działek");
                return (false, null);
            }
        }

        public static void SetLoginAndPassword() 
        {
                WindowLogowanie windowLogowanie = new WindowLogowanie();
                windowLogowanie.Show();
        }
    }
}
