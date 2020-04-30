using System;
using System.Collections.Generic;
using System.Text;

namespace ScaleniaMW
{
    class BadanieKsiagWieczystych
    {
        List<string> zbadaneKsiegi = new List<string>();

        static char[] szyfr = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'X', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'W', 'Y', 'Z' };


        public static string SprawdzCyfreKontrolna(string KsiegaWieczysta, string obreb) // zwraca null gdy księga jest ok lub 
        {

            StringBuilder bledySB = new StringBuilder();
            if (KsiegaWieczysta == null || KsiegaWieczysta == "")
            {
               
                bledySB.Append("");
                goto koniec;

            }
            int sumaKontrolna;

            char[] textBox1 = new char[15];

            KsiegaWieczysta = KsiegaWieczysta.Trim();


            textBox1 = KsiegaWieczysta.ToCharArray();

            if (textBox1.Length != 15)
            {
                bledySB.Append("Problem z ksiega: " + KsiegaWieczysta + "\n");
                goto koniec;
            }
            for (int i = 5; i < 13; i++)
            {
                try
                {

                    if (textBox1[i] == '0' || textBox1[i] == '1' || textBox1[i] == '2' || textBox1[i] == '3' || textBox1[i] == '4' || textBox1[i] == '5' || textBox1[i] == '6' || textBox1[i] == '7' || textBox1[i] == '8' || textBox1[i] == '9')
                    {

                    }
                    else
                    {

                        bledySB.Append("\nElement ->" + textBox1[i] + " <-jest nie poprawny w ksiedze nr " + KsiegaWieczysta);

                    }
                }
                catch
                {
                    bledySB.Append("problem z ksiega: " + KsiegaWieczysta + "\n");
                }
            }

            int[] liczbaSadu = new int[4];
            int[] liczbaKsiegi = new int[8];
            //   Console.WriteLine(liczbaKsiegi[3] + " kurla");
            //string[] Foo = s1.Split(new char[] { ' ' });
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < szyfr.Length; i++)
                {
                    if (textBox1[j].Equals(szyfr[i]))
                    {
                        liczbaSadu[j] = i;
                        //Console.WriteLine("LK" + liczbaSadu[j]);
                    }
                }
            }

            int koncowa = 7;
            for (int j = 12; j >= 5; j--)
            {
                for (int i = 0; i < szyfr.Length; i++)
                {
                    if (j == 4 || j == 13) continue;
                    if (textBox1[j].Equals(szyfr[i]))
                    {
                        liczbaKsiegi[koncowa] = i;
                        // Console.WriteLine("LK" + liczbaKsiegi[j]);
                        koncowa--;
                    }
                }
            }



            sumaKontrolna = liczbaSadu[0] * 1 + liczbaSadu[1] * 3 + liczbaSadu[2] * 7 + liczbaSadu[3] * 1 + liczbaKsiegi[0] * 3 + liczbaKsiegi[1] * 7 + liczbaKsiegi[2] * 1 + liczbaKsiegi[3] * 3 + liczbaKsiegi[4] * 7 + liczbaKsiegi[5] * 1 + liczbaKsiegi[6] * 3 + liczbaKsiegi[7] * 7;
            string cyfraKontr = (sumaKontrolna % 10).ToString();

            if (cyfraKontr[0] == KsiegaWieczysta[KsiegaWieczysta.Length - 1])
            {

            }
            else
            {
                bledySB.Append("\nZły nr KW, poprawna cyfra kontrolna =" + sumaKontrolna % 10 + " w KW " + KsiegaWieczysta + " w obrębie nr " + obreb);
            }
            //  Console.WriteLine("CYFRA KONTROLNA " + sumaKontrolna%10 + "KW: " + KsiegaWieczysta);
            koniec:
            
                return bledySB.ToString();         

        }
    }
}


