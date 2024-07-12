using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace ScaleniaMW
{
    class AktualizacjaOprogramowania
    {
        public static string StringFileFromServer(string uriStr)
        {
            Uri serverUri = new Uri(uriStr);
            // The serverUri parameter should start with the ftp:// scheme.
            if (serverUri.Scheme != Uri.UriSchemeFtp)
            {
                return "";
            }
            // Get the object used to communicate with the server.
            WebClient request = new WebClient();

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("marek2509@generator-raportow.cba.pl", "");
            try
            {
                byte[] newFileData = request.DownloadData(serverUri.ToString());
                string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
                return fileString;
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";
        }

        public static bool? CzyBlokowacProgram(string FTPstring = @"ftp://generator-raportow.cba.pl/SCALENIAMWCZYBLOKOWACTAKNIE")
        {
            string tekstCzyBlokowacProgram = StringFileFromServer(FTPstring);
            //Console.WriteLine("czy blokowac: " + tekstCzyBlokowacProgram);
            if (tekstCzyBlokowacProgram.ToUpper().Equals("TAK"))
            {
                return true;
            }
            else if (tekstCzyBlokowacProgram.ToUpper().Equals("NIE"))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        public static bool czyJestNowszaWersja(string FTPstring = @"ftp://generator-raportow.cba.pl/SCALENIAMW2Version")
        {
            string wersjaZFtp = StringFileFromServer(FTPstring);

            List<string> wersjaAppString = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.').ToList();
            List<int> wersjaAppInt = new List<int>();
            // wersjaAppInt = wersjaAppString.ForEach(x =>  Int32.TryParse( x, out intwersjaAppInt.Add(num)));
            foreach (var item in wersjaAppString)
            {
                int wyjscie;
                Int32.TryParse(item, out wyjscie);
                wersjaAppInt.Add(wyjscie);

            }
            List<string> wersjaFtpString = wersjaZFtp.Split('.').ToList();
            List<int> wersjaFtpInt = new List<int>();

            foreach (var item in wersjaFtpString)
            {
                int wyjscie;
                Int32.TryParse(item, out wyjscie);
                wersjaFtpInt.Add(wyjscie);
            }

            for (int i = 0; i < wersjaAppInt.Count; i++)
            {
                if (wersjaAppInt[i] < wersjaFtpInt[i])
                {
                    var resul = MessageBox.Show("Jest dostępna nowsza wersja programu. \nPobrać pliki instalacyjne?", "Aktualizacja", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (resul == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("Odinstaluj istniejącą wersję programu, aby zainstalować nową.", "Aktualizacja", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    
                    return false;
                }
                else
                {
                    if (wersjaAppInt[i] > wersjaFtpInt[i])
                    {
                        return false;
                    }
                }
            }
            return false;
        }




        public static void usunKatalogiAktualizacji(string pathNewFolder)
        {
            if (System.IO.Directory.Exists(pathNewFolder))
            {
                try
                {
                    System.IO.Directory.Delete(pathNewFolder, true);
                }

                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        static public void aktualizuj(string plikHttpAktualizacji = "http://generator-raportow.cba.pl/AktualizacjaSCALENIAMW2.zip")
        {
            // Console.WriteLine(StringFileFromServer(plikFTPaktualizacji));

            if (czyJestNowszaWersja())
            {

                try
                {
                    System.Diagnostics.Process.Start(plikHttpAktualizacji);
                }
                catch
                    (
                     System.ComponentModel.Win32Exception noBrowser)
                {
                    if (noBrowser.ErrorCode == -2147467259)
                        MessageBox.Show(noBrowser.Message);
                }
                catch (System.Exception other)
                {
                    MessageBox.Show(other.Message);
                }
                /*  string pathNewFolder = "";
                  string currentDirName = System.IO.Directory.GetCurrentDirectory();
                  for (int i = 0; i <= 1000; i++)
                  {
                      pathNewFolder = currentDirName + @"\Aktualizacja" + i;
                      if (!System.IO.Directory.Exists(pathNewFolder))
                      {
                          Console.WriteLine(i + " iterator Folderów");
                          Console.WriteLine("czy nie istnieje ");
                          System.IO.Directory.CreateDirectory(pathNewFolder);

                          break;
                      }
                      if (i == 1000) goto koniec;
                  }



                  WebClient client = new WebClient();
                  client.Credentials = new NetworkCredential("marek2509@generator-raportow.cba.pl", "Generator@2509");
                  Console.WriteLine(plikFTPaktualizacji + " \n" + pathNewFolder + @"\Aktualizacja.zip");
                  client.DownloadFile(plikFTPaktualizacji, (pathNewFolder + @"\Aktualizacja.zip"));



                      Console.WriteLine(currentDirName);
                  Console.WriteLine(pathNewFolder + @"\Aktualizacja.zip");
                  try
                  {

                      ZipFile.ExtractToDirectory(pathNewFolder + @"\Aktualizacja.zip", pathNewFolder+  @"\extract");
                  }
                  catch (Exception e)
                  {
                      MessageBox.Show("Błąd aktualizacji." + e.Message);
                  }
                  System.Diagnostics.Process.Start(pathNewFolder + @"\extract\" + nazwaPlikuAktualizacji);

                  Properties.Settings.Default.PathDeleteFile = pathNewFolder;
                  Properties.Settings.Default.Save();
                  koniec:;*/
            }
        }
    }
}
