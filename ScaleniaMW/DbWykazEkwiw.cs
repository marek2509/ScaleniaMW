using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public class Obreb
    {
        public int IdObrebu { get; set; }
        public int NrObrebu { get; set; }
        public string Nazwa { get; set; }

        public Obreb(int id, int nr, string nazwa)
        {
            IdObrebu = id;
            NrObrebu = nr;
            Nazwa = nazwa;
        }

    }

    public static class ListaObrebow
    {
        public static List<Obreb> Obreby = new List<Obreb>();
        public static void DodajObreb(int id, int nr, string nazwa)
        {
            Obreby.Add(new Obreb(id, nr, nazwa));
        }
        public static void ClearData()
        {
            Obreby = new List<Obreb>();
        }
    }

    public class JR_Nowa
    {
        public int IdJednRejN { get; set; }
        public int Ijr { get; set; }
        public int Nkr { get; set; }
        public bool Odcht { get; set; }
        public bool Zgoda { get; set; }
        public string Uwaga { get; set; }
        public string NazwaObrebu
        {
            get => ListaObrebow.Obreby.Exists(x => x.IdObrebu == _id_obr) ? ListaObrebow.Obreby.Find(x => x.IdObrebu == _id_obr).Nazwa : "BRAK_OBREBU";

            private set
            {
            }
        }
        public int NrObr
        {
            get => ListaObrebow.Obreby.Exists(x => x.IdObrebu == _id_obr) ? ListaObrebow.Obreby.Find(x => x.IdObrebu == _id_obr).NrObrebu : 0;

            private set
            {
            }
        }

        public List<Wlasciciel> Wlasciciele = new List<Wlasciciel>();

        public int _id_obr;

        public JR_Nowa(int idJednRejN, int ijr, int nkr, bool odcht, bool zgoda, string uwaga, int id_obr)
        {
            IdJednRejN = idJednRejN;
            Ijr = ijr;
            Nkr = nkr;
            Odcht = odcht;
            Zgoda = zgoda;
            Uwaga = uwaga;
            _id_obr = id_obr;
        }

        public void DodajWlasciciela(Wlasciciel wlasciciel)
        {
            if (wlasciciel.IdMalzenstwa > 0)
            {
                Wlasciciele.Add(new Wlasciciel(wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa));
                Wlasciciele.Add(new Wlasciciel(wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(1, wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(1, wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa));
            }
            else
            {
                Wlasciciele.Add(wlasciciel);
            }

        }

        public void Wypisz()
        {
            Console.WriteLine(IdJednRejN + " " + Ijr + " " + Nkr + " " + Odcht + " " + Zgoda + " " + Uwaga + " OBR:" + NazwaObrebu);
        }
    }



    public static class JednostkiRejestroweNowe
    {
        public static List<JR_Nowa> Jedn_REJ_N = new List<JR_Nowa>();
        public static void DodajJrNowa(int idJednRejN, int ijr, int nkr, bool odcht, bool zgoda, string uwaga, int id_obr)
        {
            Jedn_REJ_N.Add(new JR_Nowa(idJednRejN, ijr, nkr, odcht, zgoda, uwaga, id_obr));
        }

        public static void ClearData()
        {
            Jedn_REJ_N = new List<JR_Nowa>();
        }

        public static void Wypisz()
        {
            foreach (var item in Jedn_REJ_N)
            {
                item.Wypisz();
            }
        }
    }

    public class Wlasciciel
    {
        public Wlasciciel( string udzial, double udzial_NR, string nazwaWlasciciela, string adres, int idMalzenstwa)
        {
            Udzial = udzial;
            Udzial_NR = udzial_NR;
            NazwaWlasciciela = nazwaWlasciciela;
            Adres = adres;
            IdMalzenstwa = idMalzenstwa;
        }

        public string Udzial { get; set; }
        public double Udzial_NR { get; set; }
        public string NazwaWlasciciela { get; set; }
        public string Adres { get; set; }
        public int IdMalzenstwa { get; set; }

        public void WypiszWKoncoli()
        {
            Console.WriteLine(Udzial + " " + NazwaWlasciciela + " " + Adres);
        }

    }

}
