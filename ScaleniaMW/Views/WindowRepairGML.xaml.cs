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
    /// Logika interakcji dla klasy WindowRepairGML.xaml
    /// </summary>
    public partial class WindowRepairGML : Window
    {
        public WindowRepairGML()
        {
            InitializeComponent();
        }


        private void openFile_Click(object sender, RoutedEventArgs e)
        {
          var lista =  EWOPIS.Infrstruktura.Plik.OtworzPlik();
            GMLRepair.FindObreb(lista);
            Plik.ZapiszDoPlikuTXT(GMLRepair.WstawTagWDzialki(lista));
    


        }
    }
}
