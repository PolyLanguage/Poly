using System;
using System.Collections.Generic;
using System.Text;

namespace PolyToolkit
{
    public static class Utils
    {
        public static string Multiply(this string str,int mult)
        {
            StringBuilder res = new StringBuilder(str);
            for (int i = 1; i != mult; i++)
                res.Append(str);
            return res.ToString();
        }
    }
}
