using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Entities
{
    [Table("WZDEDZKW")]
    public class WZDEDzKW
    {
        [Key]
        public int ID { get; set; }
        public string KW { get; set; }
        [ForeignKey("Dzialka")]
        public int DZIALKAID_ID { get; set; }
        [Column("ISDELETED")]
        public bool IsDeleted { get; set; }

        public virtual Dzialka Dzialka { get; set; }
    }
}
