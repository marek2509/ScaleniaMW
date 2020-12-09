using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    static class Email
    {
        public static bool CzyJuzWysylano { get; set; } //wykozystac do tylko jednego uruchamiania w programiee
        static bool CzyWysylac = false;
        public static string externalip;

        public static void przypiszIP()
        {

            externalip = new System.Net.WebClient().DownloadString("http://icanhazip.com");
            string strComputerName = Environment.MachineName.ToString();
            // SendEmail("GENERATOR RAPORTÓW", "Właśnie użyto programu GENERATOR RAPORTÓW\n" + strComputerName + "\n" + externalip, "GENERATOR RAPORTÓW");
        }

        public static bool SendEmailAttach(string email, string text, string subject, List<Attachment> zalacznik)
        {

            string strComputerName = Environment.MachineName.ToString();
            bool result = false;
            var message = new MailMessage();
            message.From = new MailAddress("generator@generator-raportow.cba.pl", email);
            message.To.Add(new MailAddress("marek.wojciechowicz25@gmail.com"));
            message.Subject = subject + " użytkownika " + strComputerName;
            message.Body = strComputerName + "\n" + externalip + "\n" + text;
            zalacznik.ForEach(x => message.Attachments.Add(x));

            var smtp = new SmtpClient("mail.CBA.pl");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new NetworkCredential("generator@generator-raportow.cba.pl", "Generator@2509");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            
            try
            {
                smtp.Send(message);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public static bool SendEmail(string email, string text, string subject)
        {
            string strComputerName = Environment.MachineName.ToString();
            bool result = false;
            var message = new MailMessage();
            message.From = new MailAddress("generator@generator-raportow.cba.pl", email);
            message.To.Add(new MailAddress("marek.wojciechowicz25@gmail.com"));
            message.Subject = subject + " użytkownika " + strComputerName;
            message.Body = strComputerName + "\n" + externalip + "\n" + text;
            var smtp = new SmtpClient("mail.CBA.pl");
            smtp.UseDefaultCredentials = true;

            smtp.Credentials = new NetworkCredential("generator@generator-raportow.cba.pl", "Generator@2509");
            smtp.EnableSsl = true;
            smtp.Port = 587;

            try
            {
                smtp.Send(message);
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

            return result;
        }
    }
}
