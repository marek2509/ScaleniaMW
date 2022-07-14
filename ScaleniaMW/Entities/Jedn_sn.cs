using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Entities
{
    [Table("JEDN_SN")]
    public class Jedn_sn
    {
        [Key]
        public int ID_ID { get; set; }
        public int ID_JEDNN { get; set; }
        public int ID_JEDNS { get; set; }
    }
}
