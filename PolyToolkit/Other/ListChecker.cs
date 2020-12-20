using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit
{
    public static class ListChecker
    {
        public static bool ContainsType<T>(this List<T> list, Type t)
        {
            foreach(object el in list)
            {
                if (el.GetType() == t)
                    return true;
            }
            return false;
        }
    }
}
