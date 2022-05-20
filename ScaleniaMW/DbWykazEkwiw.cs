using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScaleniaMW
{
    public class Potracenie
    {
        public double WartoscPotracenia { get; set; }
        public Potracenie()
        {
            DataTable dataTable = BazaFB.Get_DataTable("select replace( substring(konfig from POSITION('PROCENT=' in upper( konfig))+8 for (POSITION('GRCENNE' in upper(konfig)) - 10 - POSITION('PROCENT=' in upper(konfig)))), '.', ',') from SYSTEM ");
            double _wartoscPotracenia;
            Console.WriteLine("potracenie: " + dataTable.Rows[0][0].ToString());
            bool convertSucces = double.TryParse(dataTable.Rows[0][0].ToString(), out _wartoscPotracenia);
            Console.WriteLine(_wartoscPotracenia);
            if (convertSucces)
            {
                WartoscPotracenia = _wartoscPotracenia / 100;
            }
            else
            {
                WartoscPotracenia = 0;
            }
        }
    }

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
        public bool DoplZaDrNieNaliczaj { get; set; }
        public bool ZerujDoplaty { get; set; }
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

        public JR_Nowa()
        {
        }

        public JR_Nowa(int idJednRejN, int ijr, int nkr, bool odcht, bool zgoda, string uwaga, int id_obr, bool doplataZaDrogi, bool zerujDoplaty)
        {
            IdJednRejN = idJednRejN;
            IjrPo = ijr;
            Nkr = nkr;
            Odcht = odcht;
            Zgoda = zgoda;
            Uwaga = uwaga;
            _id_obr = id_obr;
            DoplZaDrNieNaliczaj = doplataZaDrogi;
            ZerujDoplaty = zerujDoplaty;
        }

        public JR_Nowa(JR_Nowa nr_N, List<Dzialka_N> dzialki_n)
        {
            IdJednRejN = nr_N.IdJednRejN;
            IjrPo = nr_N.IjrPo;
            Nkr = nr_N.Nkr;
            Odcht = nr_N.Odcht;
            Zgoda = nr_N.Zgoda;
            Uwaga = nr_N.Uwaga;
            _id_obr = nr_N._id_obr;
            DoplZaDrNieNaliczaj = nr_N.DoplZaDrNieNaliczaj;
            ZerujDoplaty = nr_N.ZerujDoplaty;
            Dzialki_Nowe = dzialki_n;
        }

        public JR_Nowa JednostkaZDzialkamiZObrebu(int id_Obrebu)
        {
            return new JR_Nowa(this, Dzialki_Nowe.FindAll(x => x.Id_obr == id_Obrebu));
        }
        public JR_Nowa JednostkaZDzialkamiZRJDRPrzed(int rjdrPrzed)
        {
            return new JR_Nowa(this, Dzialki_Nowe.FindAll(x => x.RjdrPrzed == rjdrPrzed));
        }

        public string KontrolaPrzypisaniaDoRjdr()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var dzialka in Dzialki_Nowe)
            {
                if (!(zJednRejStarej.FindAll(x => x.Id_Jedns == dzialka.RjdrPrzed).Count > 0))
                {
                    if (dzialka.RjdrPrzed == 0)
                    {
                        sb.AppendLine($"Brak przypisana do JR przed: {dzialka.NrObr}-{dzialka.NrDz} ");
                    }
                    else
                    {
                        sb.AppendLine($"Nieprawidłowe przypisanie do JR: {dzialka.NrObr}-{dzialka.NrDz} ");
                    }
                }
            }

            //var dzialkiPrzypisane =  Dzialki_Nowe.FindAll(x => x.Rjdr != 0);
            //foreach (var dzialka in dzialkiPrzypisane)
            //{

            //}


            return sb.ToString();
        }

        public void DodajWlasciciela(Wlasciciel wlasciciel)
        {
            if (wlasciciel.IdMalzenstwa > 0)
            {
                Wlasciciele.Add(new Wlasciciel(wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa, wlasciciel.Symbol_Wladania));
                Wlasciciele.Add(new Wlasciciel(wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(1, wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(1, wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa, wlasciciel.Symbol_Wladania));
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

        public decimal SumaWartJednostekPrzed()
        {
            return zJednRejStarej.Sum(x => x.WrtJednPrzed);
        }

        public decimal SumaWartoJednPrzedPoPotraceinu()
        {
            return SumaWartJednostekPrzed() - SumaWartosciPotracen();
        }

        public double SumaPowJednostekPrzed()
        {
            return zJednRejStarej.Sum(x => x.Pow_Przed);
        }

        public string SumaPowierzchniDzialekNowych()
        {
            return Dzialki_Nowe.Sum(x => x.PowDz).ToString("F4", CultureInfo.InvariantCulture);
        }

        public string SumaWartosciDzialekNowych()
        {
            return Dzialki_Nowe.Sum(x => x.Wartosc).ToString("F2", CultureInfo.InvariantCulture);
        }
        public decimal SumaWartosciDzialekNowychDecimal()
        {
            return Dzialki_Nowe.Sum(x => x.Wartosc);
        }

        public decimal SumaWartosciPotracen()
        {
            return zJednRejStarej.Sum(x => x.PotrWart);
        }

        public string OdchylkaFaktyczna()
        {
            decimal wrtPo = Dzialki_Nowe.Sum(x => x.Wartosc);
            decimal wrtPrzed = SumaWartJednostekPrzed();
            decimal odchFaktyczna = wrtPo - wrtPrzed;
            return odchFaktyczna.ToString("F2", CultureInfo.InvariantCulture);
        }

        public static int CompareStringRodzWlada(Wlasciciel a, Wlasciciel b)
        {
            string x = a.Symbol_Wladania;
            string y = b.Symbol_Wladania;

            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    int retval = x.CompareTo(y);
                    if (retval != 0)
                    {
                        //if (x.ToUpper() == "WŁ")
                        //{
                        //    retval = -1;
                        //}
                        //else if (y.ToUpper() == "WŁ")
                        //{
                        //    retval = 1;
                        //}
                        //else 
                        if (x.ToUpper() == "DZ")
                        {
                            retval = 1;
                        }
                        else if (y.ToUpper() == "DZ")
                        {
                            retval = -1;
                        }
                        else
                        {
                            return a.NazwaWlasciciela.CompareTo(b.NazwaWlasciciela);
                        }
                        return retval;
                    }
                    else
                    {
                        return a.NazwaWlasciciela.CompareTo(b.NazwaWlasciciela);
                    }
                }
            }
        }

    }



    public static class JednostkiRejestroweNowe
    {
        public static List<JR_Nowa> Jedn_REJ_N = new List<JR_Nowa>();
        public static void DodajJrNowa(int idJednRejN, int ijr, int nkr, bool odcht, bool zgoda, string uwaga, int id_obr, bool doplDr, bool zerujDoplaty)
        {
            Jedn_REJ_N.Add(new JR_Nowa(idJednRejN, ijr, nkr, odcht, zgoda, uwaga, id_obr, doplDr, zerujDoplaty));
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

        public static string KontrolaPrzypisaniaDoRjdr()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var jednoskaN in Jedn_REJ_N)
            {
                sb.Append(jednoskaN.KontrolaPrzypisaniaDoRjdr());
            }

            return sb.ToString();
        }
    }

    public class Wlasciciel
    {
        public Wlasciciel(string udzial, double udzial_NR, string nazwaWlasciciela, string adres, int idMalzenstwa, string symbolWladania)
        {
            Udzial = udzial;
            Udzial_NR = udzial_NR;
            NazwaWlasciciela = nazwaWlasciciela;
            Adres = adres.Trim() == ";" ? " " : adres;
            IdMalzenstwa = idMalzenstwa;
            Symbol_Wladania = symbolWladania;
        }

        public string Udzial { get; set; }
        public double Udzial_NR { get; set; }
        public string NazwaWlasciciela { get; set; }
        public string Adres { get; set; }
        public int IdMalzenstwa { get; set; }
        public string Symbol_Wladania { get; set; }

        public void WypiszWKoncoli()
        {
            Console.WriteLine(Udzial + " " + NazwaWlasciciela + " " + Adres);
        }
    }

    public class WlascicielStanPrzed : Wlasciciel
    {
        public int IdJednPrzed { get; set; }
        public WlascicielStanPrzed(int idJednPrzed, string udzial, double udzial_NR, string nazwaWlasciciela, string adres, int idMalzenstwa, string symbolWladania) : base(udzial, udzial_NR, nazwaWlasciciela, adres, idMalzenstwa, symbolWladania)
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
        public decimal PotrWart { get; set; }
        public double PotrPow { get; set; }
        public bool PotracenieCzyStosowac { get; set; }

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

        public ZJednRejStarej(int id_Jedns, int ijr_Przed, string ud_Z_Jrs, decimal wrtJednPrzed, double pow_Przed, int idObrebu, double portacenie, bool czyJestPotracenie)
        {
            Id_Jedns = id_Jedns;
            Ijr_Przed = ijr_Przed;
            Ud_Z_Jrs = ud_Z_Jrs;
            WrtJednPrzed = Math.Round(wrtJednPrzed, 2);
            Pow_Przed = Math.Round(pow_Przed, 4);
            _id_obr = idObrebu;
            PotracenieCzyStosowac = czyJestPotracenie;
            if (PotracenieCzyStosowac)
            {
                PotrWart = Math.Round((decimal)portacenie * WrtJednPrzed, 2);
                PotrPow = Math.Round(portacenie * Pow_Przed, 4);
            }
            else
            {
                PotrWart = 0;
                PotrPow = 0;
            }

        }

        public void DodajWlascicielaWStaniePrzed(WlascicielStanPrzed wlasciciel)
        {
            if (wlasciciel.IdMalzenstwa > 0)
            {
                Wlasciciele.Add(new WlascicielStanPrzed(wlasciciel.IdJednPrzed, wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa, wlasciciel.Symbol_Wladania));
                Wlasciciele.Add(new WlascicielStanPrzed(wlasciciel.IdJednPrzed, wlasciciel.Udzial, wlasciciel.Udzial_NR, wlasciciel.NazwaWlasciciela.Remove(1, wlasciciel.NazwaWlasciciela.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.Adres.Remove(1, wlasciciel.Adres.IndexOf("Ż:")).Replace("M:", ""), wlasciciel.IdMalzenstwa, wlasciciel.Symbol_Wladania));
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

        public string SumaPowierzchniDzialek()
        {
            return Dzialki.Sum(x => Math.Round(x.PowDz, 4)).ToString("F4", CultureInfo.InvariantCulture);
        }

        public string SumaWartosciDzialek()
        {
            return Dzialki.Sum(x => Math.Round(x.Wartosc, 2)).ToString("F2", CultureInfo.InvariantCulture);
        }

        public decimal WartJednPrzedPoPotraceniu()
        {
            return WrtJednPrzed - PotrWart;
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

        public Dzialka_N(Dzialka_N dzialka_N) : base(dzialka_N.Id_dz, dzialka_N.Id_obr, dzialka_N.NrDz, dzialka_N.PowDz, dzialka_N.Rjdr, dzialka_N.KW, dzialka_N.Wartosc)
        {

            this.RjdrPrzed = dzialka_N.RjdrPrzed;
        }
    }

}
