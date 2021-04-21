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
    /// Logika interakcji dla klasy WindowLogowanie.xaml
    /// </summary>
    public partial class WindowLogowanie : Window
    {
        public WindowLogowanie()
        {
            InitializeComponent();

            textBoxLogin.Text = Properties.Settings.Default.Login;
            textBoxHaslo.Password = Properties.Settings.Default.Haslo;
        }

        private void ButtonZapiszLogIHaslo(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Properties.Settings.Default.Login + " " + Properties.Settings.Default.Haslo);
            Properties.Settings.Default.Login = textBoxLogin.Text;
            Properties.Settings.Default.Haslo = textBoxHaslo.Password;
            Properties.Settings.Default.Save();
            windowLogowanie.Close();
            Console.WriteLine(Properties.Settings.Default.Login + " " + Properties.Settings.Default.Haslo);
        }

        private void Button_Anuluj(object sender, RoutedEventArgs e)
        {
            windowLogowanie.Close();
        }
    }
}
