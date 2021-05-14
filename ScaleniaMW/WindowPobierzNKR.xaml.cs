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
    /// Logika interakcji dla klasy WindowPobierzNKR.xaml
    /// </summary>
    public partial class WindowPobierzNKR : Window
    {
        public WindowPobierzNKR()
        {
            InitializeComponent();
        }

        private void Button_ClickPotwierdzWpisanyNKR(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(textBoxNkrDoWWE.Text);
            WindowEdycjaDokumentow wED = new WindowEdycjaDokumentow();
            Console.WriteLine(JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.IjrPo.ToString() == textBoxNkrDoWWE.Text.Trim()).Count);
            if(JednostkiRejestroweNowe.Jedn_REJ_N.Exists(x => x.IjrPo.ToString() == textBoxNkrDoWWE.Text))
            {
                wED.zapisDoPliku(wED.GenerujWWE(JednostkiRejestroweNowe.Jedn_REJ_N.FindAll(x => x.IjrPo.ToString() == textBoxNkrDoWWE.Text)));
            }
            else
            {
                MessageBox.Show("Nie odnaleziono jednostki nr " + textBoxNkrDoWWE.Text + "!");
            }
            windowPobierzNKR.Close();
        }
    }
}
