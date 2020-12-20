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

        public static PolyType[] ToTypes(this List<ArgumentNode> list)
        {
            return list.ToTypesList().ToArray();
        }

        public static List<PolyType> ToTypesList(this List<ArgumentNode> list)
        {
            List<PolyType> res = new List<PolyType>();
            foreach (ArgumentNode node in list)
                res.Add(node.ArgType.Type);
            return res;
        }
    }
}
