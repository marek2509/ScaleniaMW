using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Entities
{
    [Table("DZIALKI_N")]
    public class Dzialki_n
    {
        [Key]
        public int ID_ID { get; set; }
        public string IDD { get; set; }
        public int PEW { get; set; }
        public string SIDD { get; set; }
        public string KW { get; set; }
        public int? RJDRPRZED { get; set; }

        [ForeignKey("Obreb")]
        public int IDOBR { get; set; }
        public virtual Obreby Obreb { get; set; }


        [ForeignKey("JednRejN")]
        public int RJDR { get; set; }
        public Jedn_rej_n JednRejN { get; set; }

    }
}
