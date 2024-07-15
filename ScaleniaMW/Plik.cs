using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScaleniaMW
{
    public static class Plik
    {
        public static void ZapiszDoPlikuTXT(string tekstDoZapisu, string format = null)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = $".{format ?? "gml"}";
            svd.Filter = $"{format ?? "gml"} (*.{format ?? "gml"})|*.{format ?? "gml"}|All files (*.*)|*.*";
            if (svd.ShowDialog() == true)
            {
                using (Stream s = File.Open(svd.FileName, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(s, Encoding.Default))
                        try
                        {
                            try
                            {
                                sw.Write(tekstDoZapisu);
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
                        }
                }
            }

        }


        static List<char> cyfry = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };



        public static string UsuniecieInnychZnakowNizCyfry(string tekstDoPoprawki)
        {
            char jakaLiterkeUsunac = ' ';
        wroc:;
            tekstDoPoprawki = tekstDoPoprawki.Replace(jakaLiterkeUsunac.ToString(), "");
            foreach (var item in tekstDoPoprawki)
            {
                if (!cyfry.Exists(x => x == item))
                {
                    jakaLiterkeUsunac = item;
                    goto wroc;
                }
                else
                {
                    continue;
                }
            }
            return tekstDoPoprawki;
        }
    }
}
