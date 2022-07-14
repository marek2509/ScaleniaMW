using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Models
{
    public class DzialkaDto
    {
        public int Id_Id { get; set; }
        public string Idd { get; set; }
        public int Pew { get; set; }
        public string Sidd { get; set; }
        public string KW { get; set; }
        public int? RjdrPrzed { get; set; }
        public int Rjdr { get; set; }
        public int ObrNr { get; set; }
        public string ObrNaz { get; set; }

        public DzialkaDto(Dzialki_n dz)
        {
            Id_Id = dz.ID_ID;
            Idd = dz.IDD;
            Pew = dz.PEW;
            Sidd = dz.SIDD;
            KW = dz.KW;
            RjdrPrzed = dz.RJDRPRZED;
            Rjdr = dz.RJDR;
            ObrNr = dz.Obreb.ID;
            ObrNaz = dz.Obreb.NAZ;
        }
    }
}
