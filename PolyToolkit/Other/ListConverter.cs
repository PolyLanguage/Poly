using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit
{
    public static class ListConverter
    {
        public static List<IAstNode> ToIAstList<T>(this List<T> list)
        {
            List<IAstNode> res = new List<IAstNode>();
            foreach (T el in list)
                res.Add(((IAstNode)el));
            return res;
        }
        public static Type[] ToNativeArray(this List<PolyType> list)
        {
            List<Type> res = new List<Type>();
            foreach (PolyType type in list)
                res.Add(type.ToNativeType());
            return res.ToArray();
        }
    }
}
