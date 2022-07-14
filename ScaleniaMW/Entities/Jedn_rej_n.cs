using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Entities
{
    [Table("JEDN_REJ_N")]
    public class Jedn_rej_n
    {
        [Key]
        [Column("ID_ID")]
        public int ID_ID { get; set; }
        public int IJR { get; set; }

        [ForeignKey("Obreb")]
        public int? ID_OBR { get; set; }
        public virtual Obreby Obreb { get; set; }

        public virtual List<Dzialki_n> Dzialki_Nowe { get; set; }
    }
}
