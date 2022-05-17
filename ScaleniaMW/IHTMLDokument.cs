using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW
{
    public interface IHTMLDokument
    {
        string GenerujWWE(List<JR_Nowa> JRN);
    }
}
