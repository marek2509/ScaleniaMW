using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.EWOPIS.Modele
{
    public class ModelObrebu
    {
        public int Idobr { get; set; }
        public string Teryt { get; set; }


        public ModelObrebu(int idobr, string teryt)
        {
            Idobr = idobr;
            Teryt = teryt;
        }
    }
}
