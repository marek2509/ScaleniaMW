using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleniaMW.Helpers
{
    public static class ExtensionsClass
    {
        public static bool IsSemicolon { get; set; } = Properties.Settings.Default.przecinekWWWe;
        public static string ToSemicolon(this string str)
        {
            return IsSemicolon ? str.Replace(".", ",") : str;
        }
    }
}
