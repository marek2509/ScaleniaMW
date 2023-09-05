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
    /// Logika interakcji dla klasy WindowWzorzec.xaml
    /// </summary>
    public partial class WindowWzorzec : Window
    {
        public WindowWzorzec()
        {
            InitializeComponent();
            radioTabZyczenDomyslne.IsChecked = Properties.Settings.Default.radioTabZyczenDomyslne;
            radioTabZyczenWlasne.IsChecked = Properties.Settings.Default.radioTabZyczenWlasne;
            radioTabZyczenWylacz.IsChecked = Properties.Settings.Default.radioTabZyczenWylacz;
            textBoxWzorzec_naglowekOmowienieZastrzezen.Text = Properties.Settings.Default.textBoxWzorzec_naglowekOmowienieZastrzezen;
            textBoxWzorzec_naglowekOswiadczenUczestnika.Text = Properties.Settings.Default.textBoxWzorzec_naglowekOswiadczenUczestnika;
        }

        private void RadioTabZyczen_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.radioTabZyczenDomyslne = (bool)radioTabZyczenDomyslne.IsChecked;
            Properties.Settings.Default.radioTabZyczenWlasne = (bool)radioTabZyczenWlasne.IsChecked;
            Properties.Settings.Default.radioTabZyczenWylacz = (bool)radioTabZyczenWylacz.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void TextBoxWzorzec_naglowekOswiadczenUczestnika_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.textBoxWzorzec_naglowekOswiadczenUczestnika = textBoxWzorzec_naglowekOswiadczenUczestnika.Text;
        }

        private void TextBoxWzorzec_naglowekOmowienieZastrzezen_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.textBoxWzorzec_naglowekOmowienieZastrzezen = textBoxWzorzec_naglowekOmowienieZastrzezen.Text;
        }
    }
}
