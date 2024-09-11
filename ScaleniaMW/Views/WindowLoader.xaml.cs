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

namespace ScaleniaMW.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Loader.xaml
    /// </summary>
    public partial class WindowLoader : Window
    {
        private bool _isCancell = false;
        public WindowLoader(int countItem)
        {
            InitializeComponent();
            countItems.Content = countItem;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        public void UpdateValue(int currentValue)
        {
            current.Content = currentValue;
            //return _isCancell;
        }

        public void Cancell()
        {
            _isCancell = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancell();
        }
    }
}
