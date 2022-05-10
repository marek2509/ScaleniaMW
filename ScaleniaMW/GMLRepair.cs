using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{

    public class TagGml
    {
        public string StartTag { get; set; }
        public string EndTag { get; set; }
        public TagGml InsidaTag;

        public TagGml(string startTag, string endTag)
        {
            StartTag = startTag;
            EndTag = endTag;
        }

        public TagGml(string startTag, string endTag, TagGml insideTag)
        {
            StartTag = startTag;
            EndTag = endTag;
            InsidaTag = insideTag;
        }
    }

    public class ObrTerydIdLocal
    {
        public string Teryt { get; set; }
        public string LocalId { get; set; }

        public ObrTerydIdLocal(string teryt, string localId)
        {
            Teryt = teryt;
            LocalId = localId;
        }
    }


    public class DzialkaTerytLikalizacja
    {
        public string TerytDzialki { get; set; }
        public string TagLokalizDzialki { get; set; }

        public DzialkaTerytLikalizacja(string teryt, string lokalizacjaDzialki)
        {
            TerytDzialki = teryt;
            TagLokalizDzialki = lokalizacjaDzialki;
        }
    }



    public static class GMLRepair
    {
        public static List<ObrTerydIdLocal> listObrTerytAndLocalId = new List<ObrTerydIdLocal>();
        public static List<DzialkaTerytLikalizacja> listIdDzialkiTagDoDzialki = new List<DzialkaTerytLikalizacja>();

        static public void FindObreb(List<string> listaGML)
        {
            TagGml TagObrebEwidencyjny = new TagGml("<egb:EGB_ObrebEwidencyjny", "</egb:EGB_ObrebEwidencyjny>");
            TagGml TagLokalneId = new TagGml("<bt:lokalnyId>", "</bt:lokalnyId>");
            TagGml TagTerytObrebu = new TagGml("<egb:idObrebu>", "</egb:idObrebu>");

            bool wasStart = false;
            string teryt = null;
            string localId = null;

            foreach (var lineGml in listaGML)
            {
                if (wasStart == false)
                {
                    if (lineGml.StartsWith(TagObrebEwidencyjny.StartTag))
                    {
                        wasStart = true;
                    }
                }
                else if (wasStart && (lineGml.StartsWith(TagLokalneId.StartTag) || lineGml.StartsWith(TagTerytObrebu.StartTag)))
                {
                    // local id
                    if (lineGml.StartsWith(TagLokalneId.StartTag))
                    {
                        localId = lineGml.Replace(TagLokalneId.StartTag, null).Replace(TagLokalneId.EndTag, null);
                    }
                    // teryt
                    else if (lineGml.StartsWith(TagTerytObrebu.StartTag))
                    {
                        teryt = lineGml.Replace(TagTerytObrebu.StartTag, null).Replace(TagTerytObrebu.EndTag, null);
                    }
                }
                else
                {
                    if (lineGml.Contains(TagObrebEwidencyjny.EndTag))
                    {
                        listObrTerytAndLocalId.Add(new ObrTerydIdLocal(teryt, localId));
                        wasStart = false;
                    }
                }
            }


        }

        static string usunTagiZLiniGml(string linia, TagGml tag)
        {
            return linia.Replace(tag.StartTag, null).Replace(tag.EndTag, null);
        }

        static public string WstawTagWDzialki(List<string> listaGML)
        {
            // tagi do działek
            TagGml TagDzialkaEwid = new TagGml("<egb:EGB_DzialkaEwidencyjna", "</egb:EGB_DzialkaEwidencyjna>");
            TagGml TagIdDzialki = new TagGml("<egb:idDzialki>", "</egb:idDzialki>");

            // tagi do jednostek
            TagGml TagJednRejestrowa = new TagGml("<egb:EGB_JednostkaRejestrowaGruntow", "</egb:EGB_JednostkaRejestrowaGruntow>");
            TagGml TagIdJednRejestrowej = new TagGml("<egb:idJednostkiRejestrowej>", "</egb:idJednostkiRejestrowej>");

            // tagi uniwersalne znajdujace się w działkach i jednoskach rejestrowych
            TagGml TagPrzestrzenNazw = new TagGml("<bt:przestrzenNazw>", "</bt:przestrzenNazw>");

            // zmienne przy wyszukiwaniu działek
            string terytDzialki = null;
            string przestrzenNazwDzialki = null;
            bool wasStartDzialka = false;

            // zmienne przy wyszukiwaniu jednostek
            string terytJednostkaRej = null;
            string przestrzenNazwJedn = null;
            bool wasStartJednstka = false;

            // plik wyjściowy
            StringBuilder stringBuilderGML = new StringBuilder();

            string pobierzTagDoDzialki()
            {
                if (listObrTerytAndLocalId.Find(x => terytDzialki.Contains(x.Teryt)) == null)
                {
                    Console.WriteLine("Dzialka bez znalezionego obrebu: " + terytDzialki);
                    return "";
                }
                else
                {
                return "<egb:lokalizacjaDzialki2 xlink:href=\"urn:pzgik:id:" + przestrzenNazwDzialki + ":" + listObrTerytAndLocalId.Find(x => terytDzialki.Contains(x.Teryt)).LocalId + "\" />";
                }
            }


            string pobierzTagDoJednostki()
            {
                if(listObrTerytAndLocalId.Find(x => terytJednostkaRej.Contains(x.Teryt)) == null)
                {
                    Console.WriteLine("jednostka bez znalezionego obrebu: " + terytJednostkaRej);
                    return "";
                }
                return "<egb:lokalizacjaJRG xlink:href=\"urn:pzgik:id:" + przestrzenNazwJedn + ":" + listObrTerytAndLocalId.Find(x => terytJednostkaRej.Contains(x.Teryt)).LocalId + "\" />";

            }



            foreach (var lineGml in listaGML)
            {

                // wstawianie tagu w działki
                if (wasStartDzialka == false)
                {
                    if (lineGml.StartsWith(TagDzialkaEwid.StartTag))
                    {
                        wasStartDzialka = true;
                    }
                }
                else if (wasStartDzialka && (lineGml.StartsWith(TagPrzestrzenNazw.StartTag) || lineGml.StartsWith(TagIdDzialki.StartTag)))
                {
                    // local id
                    if (lineGml.StartsWith(TagPrzestrzenNazw.StartTag))
                    {
                        przestrzenNazwDzialki = lineGml.Replace(TagPrzestrzenNazw.StartTag, null).Replace(TagPrzestrzenNazw.EndTag, null);
                    }
                    // teryt
                    else if (lineGml.StartsWith(TagIdDzialki.StartTag))
                    {
                        terytDzialki = lineGml.Replace(TagIdDzialki.StartTag, null).Replace(TagIdDzialki.EndTag, null);
                    }
                }
                else
                {
                    if (lineGml.Contains(TagDzialkaEwid.EndTag))
                    {
                        //listIdDzialkiTagDoDzialki.Add(new DzialkaTerytLikalizacja(terytDzialkiLinia, pobierzTagDoDzialki()));
                        wasStartDzialka = false;
                        stringBuilderGML.AppendLine(pobierzTagDoDzialki());
                    }
                }


                // wstawianie tagu w jednosce rejestrowej
                if (wasStartJednstka == false)
                {
                    if (lineGml.StartsWith(TagJednRejestrowa.StartTag))
                    {
                        wasStartJednstka = true;
                    }
                }
                else if (wasStartJednstka && (lineGml.StartsWith(TagPrzestrzenNazw.StartTag) || lineGml.StartsWith(TagIdJednRejestrowej.StartTag)))
                {
                    // przestrzen nazw
                    if (lineGml.StartsWith(TagPrzestrzenNazw.StartTag))
                    {
                        przestrzenNazwJedn = lineGml.Replace(TagPrzestrzenNazw.StartTag, null).Replace(TagPrzestrzenNazw.EndTag, null);
                    }

                    // teryt
                    else if (lineGml.StartsWith(TagIdJednRejestrowej.StartTag))
                    {
                        terytJednostkaRej = lineGml.Replace(TagIdJednRejestrowej.StartTag, null).Replace(TagIdJednRejestrowej.EndTag, null);
                    }
                }
                else
                {
                    if (lineGml.Contains(TagJednRejestrowa.EndTag))
                    {
                        //listIdDzialkiTagDoDzialki.Add(new DzialkaTerytLikalizacja(terytDzialkiLinia, pobierzTagDoDzialki()));
                        wasStartJednstka = false;
                        stringBuilderGML.AppendLine(pobierzTagDoJednostki());
                    }
                }


                stringBuilderGML.AppendLine(lineGml);
            }

            return stringBuilderGML.ToString();
        }
    }
}
