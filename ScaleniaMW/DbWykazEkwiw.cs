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
        public int IjrPo { get; set; }
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
        public List<ZJednRejStarej> zJednRejStarej = new List<ZJednRejStarej>();
        public List<Dzialka_N> Dzialki_Nowe = new List<Dzialka_N>();
        public int _id_obr;

        public JR_Nowa(int idJednRejN, int ijr, int nkr, bool odcht, bool zgoda, string uwaga, int id_obr)
        {
            IdJednRejN = idJednRejN;
            IjrPo = ijr;
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

        public void DodajZJrStarej(ZJednRejStarej zJednRej)
        {
            zJednRejStarej.Add(zJednRej);
        }

        public void DodajDzialke(Dzialka_N dzialka)
        {
            Dzialki_Nowe.Add(dzialka);
        }

        public void Wypisz()
        {
            Console.WriteLine(IdJednRejN + " " + IjrPo + " " + Nkr + " " + Odcht + " " + Zgoda + " " + Uwaga + " OBR:" + NazwaObrebu);
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
        public Wlasciciel(string udzial, double udzial_NR, string nazwaWlasciciela, string adres, int idMalzenstwa)
        {
            Udzial = udzial;
            Udzial_NR = udzial_NR;
            NazwaWlasciciela = nazwaWlasciciela;
            Adres = adres.Trim() == ";" ? " " : adres;
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

    public class WlascicielStanPrzed : Wlasciciel
    {
        public int IdJednPrzed { get; set; }
        public WlascicielStanPrzed(int idJednPrzed, string udzial, double udzial_NR, string nazwaWlasciciela, string adres, int idMalzenstwa) : base(udzial,  udzial_NR,  nazwaWlasciciela,  adres,  idMalzenstwa)
        {
            IdJednPrzed = idJednPrzed;
        }
    }

    public class ZJednRejStarej
    {
        public int Id_Jedns { get; set; }
        public int Ijr_Przed { get; set; }
        public string Ud_Z_Jrs { get; set; }
        public decimal WrtJednPrzed { get; set; }
        public double Pow_Przed { get; set; }
        public List<Dzialka> Dzialki = new List<Dzialka>();
        public List<WlascicielStanPrzed> Wlasciciele = new List<WlascicielStanPrzed>();
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
        public int _id_obr;

        public ZJednRejStarej(int id_Jedns, int ijr_Przed, string ud_Z_Jrs, decimal wrtJednPrzed, double pow_Przed, int idObrebu)
        {
            Id_Jedns = id_Jedns;
            Ijr_Przed = ijr_Przed;
            Ud_Z_Jrs = ud_Z_Jrs;
            WrtJednPrzed = wrtJednPrzed;
            Pow_Przed = pow_Przed;
            _id_obr = idObrebu;
        }

        public void DodajWlascicielaWStaniePrzed(WlascicielStanPrzed wlasciciel)
        {
            if (wlasciciel.IdMalzenstwa > 0)
            {
                Wlasciciele.Add(new WlascicielStanPrzed(wlasciciel.IdJednPrzed, wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa));
                Wlasciciele.Add(new WlascicielStanPrzed(wlasciciel.IdJednPrzed, wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(1, wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(1, wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa));
            }
            else
            {
                Wlasciciele.Add(wlasciciel);
            }
        }

        public void DodajDzialkePrzed(Dzialka dzialka)
        {
            Dzialki.Add(dzialka);
        }
    }

    public class Dzialka
    {
        public int Id_dz { get; set; }
        public int Id_obr { get; set; }
        public string NrDz { get; set; }
        public double PowDz { get; set; }
        public int Rjdr { get; set; }
        public string KW { get; set; }
        public decimal Wartosc { get; set; }

        public int NrObr
        {
            get => ListaObrebow.Obreby.Exists(x => x.IdObrebu == Id_obr) ? ListaObrebow.Obreby.Find(x => x.IdObrebu == Id_obr).NrObrebu : 0;

            private set
            {
            }
        }

        public string NazwaObrebu
        {
            get => ListaObrebow.Obreby.Exists(x => x.IdObrebu == Id_obr) ? ListaObrebow.Obreby.Find(x => x.IdObrebu == Id_obr).Nazwa : "BRAK_OBREBU";

            private set
            {
            }
        }

        public Dzialka(int Id_dz, int Id_obr, string NrDz, double PowDz, int Rjdr, string KW, decimal Wartosc)
        {
            this.Id_dz = Id_dz;
            this.Id_obr = Id_obr;
            this.NrDz = NrDz;
            this.PowDz = PowDz;
            this.Rjdr = Rjdr;
            this.KW = KW;
            this.Wartosc = Wartosc;

        }
    }

    public class Dzialka_N : Dzialka
    {

        public int RjdrPrzed { get; set; }


        public Dzialka_N(int Id_dz, int Id_obr, string NrDz, double PowDz, int Rjdr, int RjdrPrzed, string KW, decimal Wartosc) : base(Id_dz, Id_obr, NrDz, PowDz, Rjdr, KW, Wartosc)
        {
            this.RjdrPrzed = RjdrPrzed;
        }
    }

}
