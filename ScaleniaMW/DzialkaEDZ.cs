using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class DzialkaEDZ
    {
        public string Nr_Dz { get; set; }

        public double DzX1;
        public double DzY1;
        public double DzX2;
        public double DzY2;
        public int ilePktow;

        public List<WspPktu> listaWspPktu = new List<WspPktu>();
        public class WspPktu
        {
            public string NR;
            public double X;
            public double Y;
        }

        double dlZeWsp(double x1, double y1, double x2, double y2)
        {
            double dlugoscMiedzPktami = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return dlugoscMiedzPktami;
        }

        public double podajeKatUstawienia()
        {
            double tmpDlugosc = 0;
            double tmpAzymut = 0;
            for (int i = 0; i < listaWspPktu.Count - 1; i++)
            {
                double oblDl = dlZeWsp(listaWspPktu[i].X, listaWspPktu[i].Y, listaWspPktu[i + 1].X, listaWspPktu[i + 1].Y);
                double azymut = oblAzymut(listaWspPktu[i].X, listaWspPktu[i].Y, listaWspPktu[i + 1].X, listaWspPktu[i + 1].Y);
                if (tmpDlugosc < oblDl)
                {
                    tmpDlugosc = oblDl;
                    tmpAzymut = azymut;
                }
            }

            double katObrotu = tmpAzymut + 300;
            while (katObrotu > 400)
            {
                katObrotu = katObrotu - 400;
            }

            if (katObrotu > 100 && katObrotu < 300)
            {
                katObrotu = katObrotu + 200;
            }

            while (katObrotu > 400)
            {
                katObrotu = katObrotu - 400;
            }
            return katObrotu * 180 / 200;
        }

        double oblAzymut(double x1, double y1, double x2, double y2)
        {
            double tgCzwartaka = 0;
            double czwartak = 0;

            double deltaY = y2 - y1;
            double deltaX = x2 - x1;

            if (deltaX != 0)
            {
                tgCzwartaka = (deltaY / deltaX);
            }
            czwartak = (Math.Atan(tgCzwartaka));
            czwartak = czwartak * 200 / Math.PI;
            czwartak = Math.Abs(czwartak);
            double azymut = 0;

            if (deltaY > 0)
            {
                if (deltaX > 0)
                {


                    azymut = czwartak;

                }
                else if (deltaX < 0)
                {

                    azymut = 200 - czwartak;

                }
                else if (deltaX == 0)
                {
                    azymut = 100;
                }

            }
            else if (deltaY < 0)
            {
                if (deltaX > 0)
                {
                    azymut = 400 - czwartak;
                }
                else if (deltaX < 0)
                {
                    azymut = 200 + czwartak;
                }
                else if (deltaX == 0)
                {
                    azymut = 300;
                }
            }
            else if (deltaY == 0)
            {


                if (deltaX > 0)
                {
                    azymut = 0;
                }
                else if (deltaX < 0)
                {
                    azymut = 200;
                }

                else if (deltaX == 0)
                {
                    azymut = 0;
                }
            }

            return azymut;
        }
    }
}
