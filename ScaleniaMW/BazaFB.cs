using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using FirebirdSql.Data.FirebirdClient;

namespace ScaleniaMW
{
    static class BazaFB
    {
        public static void ustawProperties(string FileName, ref TextBlock textBlockSciezka)
        {
            Properties.Settings.Default.PathFDB = FileName;
            textBlockSciezka.Text = Properties.Settings.Default.PathFDB;
            Properties.Settings.Default.Save();
        }

        public static string connectionString()
        {
            return @"User=" + Properties.Settings.Default.Login + ";Password=" + Properties.Settings.Default.Haslo + ";Database= " + Properties.Settings.Default.PathFDB + "; DataSource=localhost; Port=" + Constants.PortFB + ";Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true;" +
                                   "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
        }

        public static FbConnection Get_DB_Connection()
        {
            string cn_String = connectionString();
            FbConnection cn_connection = new FbConnection(cn_String);
            if (cn_connection.State != ConnectionState.Open) cn_connection.Open();
            return cn_connection;

        }

        public static DataTable Get_DataTable(string SQL_Text)
        {
            try
            {
                FbConnection cn_connection = Get_DB_Connection();
                DataTable table = new DataTable();
                FbDataAdapter adapter = new FbDataAdapter(SQL_Text, cn_connection);
                adapter.Fill(table);
                return table;
            }
            catch (Exception s)
            {
                przejdzDoOknaLogowania(s.Message);
                Console.WriteLine(s.Message);
                return new DataTable();
            }
        }

        public static void Execute_SQL(string SQL_command_execute)
        {
            FbConnection cn_connection = Get_DB_Connection();

            FbCommand cmd_Command = new FbCommand(SQL_command_execute, cn_connection);
            cmd_Command.ExecuteNonQuery();
        }

        public static void Close_DB_Connection()
        {
            string cn_String = connectionString();

            FbConnection cn_connection = new FbConnection(cn_String);

            if (cn_connection.State != ConnectionState.Closed) cn_connection.Close();
        }


        private static void StaThreadWrapper(Action action)
        {
            var t = new Thread(o =>
            {
                action();
                System.Windows.Threading.Dispatcher.Run();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }


        static void przejdzDoUstawLoginIHaslo()
        {
            StaThreadWrapper(() =>
            {
                WindowLogowanie windowLogowanie = new WindowLogowanie();
                windowLogowanie.Show();
            });
        }

        static void przejdzDoOknaLogowania(string s)
        {
            if (s.Contains("password"))
            {
                przejdzDoUstawLoginIHaslo();
            }
        }
    }
}
