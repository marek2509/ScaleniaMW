using ScaleniaMW.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Models
{
    public class JednRejNDto
    {
        public int Id_id { get; set; }
        public int Ijr { get; set; }
        public int? ObrNr { get; set; }
        public string ObrNaz { get; set; }
        public List<JednRejDto> JednostkiPrzed { get; set; }
        public List<DzialkaDto> Dzialki { get; set; }

        public JednRejNDto(Entities.Jedn_rej_n jn, IEnumerable<Jedn_rej> jr, IEnumerable<Dzialki_n> dz )
        {
            Id_id = jn.ID_ID;
            Ijr = jn.IJR;
            ObrNr = jn.Obreb?.ID;
            ObrNaz = jn.Obreb?.NAZ;
            JednostkiPrzed = jr.Select(x => new JednRejDto(x)).ToList();
            Dzialki = dz.Select(x => new DzialkaDto(x)).ToList();
        }
    }
}
