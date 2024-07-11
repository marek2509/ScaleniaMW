using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Entities
{
    [Table("JEDN_REJ")]
    public class Jedn_rej
    {
        [Key]
        public int ID_ID { get; set; }
        public int IJR { get; set; }
        public int NKR { get; set; }
        public int? ID_STI { get; set; }

        [ForeignKey("Obreb")]
        public int ID_OBR { get; set; }
        public virtual Obreby Obreb { get; set; }


        public virtual List<Dzialka> Dzialki { get; set; }

    }
}
