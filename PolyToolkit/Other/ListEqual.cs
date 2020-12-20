using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit
{
    public static class ListEqual
    {
        public static bool IsEqual(this PolyType[] types1,PolyType[] types2)
        {
            if(types1.Length == types2.Length)
            {
                int typeIndex = 0;
                foreach(PolyType type in types1)
                {
                    if (type.Name != types2[typeIndex].Name)
                        return false;
                    typeIndex++;
                }
                return true;
            }

            return false;
        }
    }
}
