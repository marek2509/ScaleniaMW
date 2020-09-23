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
    /// Logika interakcji dla klasy EdycjaDokumentow.xaml
    /// </summary>
    public partial class WindowEdycjaDokumentow : Window
    {
        public WindowEdycjaDokumentow()
        {
            InitializeComponent();
        }

        private void otworzOknoPoczatkowe_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            windowEdycjaDokumentow.Close();
        }
        private void zamknijProgram_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Otworz_RTF_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
