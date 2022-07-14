using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Models
{
    public class JednRejDto
    {
        public int Id_id { get; set; }
        public int Ijr { get; set; }
        public int Nkr { get; set; }
        public int ObrNr { get; set; }
        public string ObrNaz { get; set; }

        public JednRejDto(Jedn_rej jr)
        {
            Id_id = jr.ID_ID;
            Ijr = jr.IJR;
            Nkr = jr.NKR;
            ObrNr = jr.Obreb.ID;
            ObrNaz = jr.Obreb.NAZ;
        }
    }
}
