using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class Punkt
    {
        public string NazwaDz;
        public float DzX1;
        public float DzY1;
        public int ilePktow;


        public List<WspPktu> listaWspPktu = new List<WspPktu>();


        public class WspPktu
        {
            public string NR;
            public float X;
            public float Y;
        }

        float dlZeWsp(float x1, float y1, float x2, float y2)
        {
            double dlugoscMiedzPktami = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            return (float)dlugoscMiedzPktami;
        }

        public float podajeKatUstawienia()
        {
            float tmpDlugosc = 0;
            float tmpAzymut = 0;
            for (int i = 0; i < listaWspPktu.Count - 1; i++)
            {
                float oblDl = dlZeWsp(listaWspPktu[i].X, listaWspPktu[i].Y, listaWspPktu[i + 1].X, listaWspPktu[i + 1].Y);
                float azymut = (float)oblAzymut(listaWspPktu[i].X, listaWspPktu[i].Y, listaWspPktu[i + 1].X, listaWspPktu[i + 1].Y);
                if (tmpDlugosc < oblDl)
                {
                    tmpDlugosc = oblDl;
                    tmpAzymut = azymut;
                }
            }

            float katObrotu = tmpAzymut + 300;
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
            return katObrotu;
        }

        double oblAzymut(float x1, float y1, float x2, float y2)
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
            return azymut*180/200;
        }
    }
}
