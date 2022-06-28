using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Models
{
    class NkrColNameValueDto
    {
        public int NKR { get; set; }
        public string ColumneName { get; set; }
        public bool Value { get; set; }

        public NkrColNameValueDto(int nkr, string colDbName, bool value)
        {
            NKR = nkr;
            ColumneName = colDbName;
            Value = value;
        }
    }
}
