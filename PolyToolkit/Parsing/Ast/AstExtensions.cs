using PolyToolkit;
using PolyToolkit.Parsing;
namespace PolyToolkit.Parsing.Ast
{
    public static class AstExtensions
    {
        #region Check Actions
        /// <summary>
        /// Find class inside tree
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsContainsNamespaceNode(this IWithBody container)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                if (node is NamespaceStatementNode)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Find class inside tree
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsContainsClass(this IWithBody container, string name)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                if (node is ClassNode && ((ClassNode)node).Name.Value == name)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Check if node got variable inside (not inside of childs)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsContainsVariable(this IWithBody container, string name)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                //if:
                //1. is variable declaration (registration)
                //2. names is identical
                if (node is VarDeclarationNode && ((VarDeclarationNode)node).VarName.Value == name)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Check if class got method inside (not inside of childs)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsContainsMethod(this ClassNode container, string name, PolyType[] args)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                //if:
                //1. is method
                //2. method is identical
                //3. args types is identical
                if (node is MethodNode && ((MethodNode)node).MethodName.Value == name &&
                    ((MethodNode)node).MethodArgs.Args.ToTypes() == args)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Checks if class got ctor inside (not inside of childs)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsContainsCtor(this ClassNode container, PolyType[] args)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                //if:
                //1. is ctor
                //2. args is identical
                if (node is ClassCtorNode && ((ClassCtorNode)node).Args.Args.ToTypes() == args)
                    return true;
            }
            return false;
        }

        public static bool IsVariableAvailable(this IAstNode curnode,string varname)
        {
            if(curnode is IWithBody)
            {
                foreach(IAstNode node in ((IWithBody)curnode).Body.Childs)
                {
                    if (node is VarDeclarationNode && ((VarDeclarationNode)node).VarName.Value == varname)
                        return true;
                }
            }
            if (curnode.Parent is CodeTree || curnode.Parent==null)
                return false;
            if (curnode.Parent is IWithBody)
            {
                foreach(IAstNode node in ((IWithBody)curnode.Parent).Body.Childs)
                {
                    if (node is VarDeclarationNode && ((VarDeclarationNode)node).VarName.Value == varname)
                        return true;
                }
            }
            return curnode.Parent.IsVariableAvailable(varname);
        }
        #endregion

        #region Check Childs Actions
        /// <summary>
        /// Check if node got variable inside (not inside of childs)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsChildsContainsVariable(this IAstNode container, string name)
        {
            foreach (IAstNode node in container.Childs)
            {
                //if:
                //1. is variable declaration (registration)
                //2. names is identical
                if (node is VarDeclarationNode && ((VarDeclarationNode)node).VarName.Value == name)
                    return true;
                else if (node is IAstNode && ((IAstNode)node).IsChildsContainsVariable(name))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Check if node got variable inside (not inside of childs)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsChildsContainsVariable(this IWithBody container, string name)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                //if:
                //1. is variable declaration (registration)
                //2. names is identical
                if (node is VarDeclarationNode && ((VarDeclarationNode)node).VarName.Value == name)
                    return true;
                else if (node is IWithBody && ((IWithBody)node).IsChildsContainsVariable(name))
                    return true;
            }
            return false;
        }
        #endregion

        #region Get Actions
        /// <summary>
        /// Find class inside tree
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ClassNode GetClass(this CodeTree container, string name)
        {
            foreach (IAstNode node in container.Childs)
            {
                if (node is ClassNode && ((ClassNode)node).Name.Value == name)
                    return (ClassNode)node;
            }
            return null;
        }
        /// <summary>
        /// Find variable inside
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static VarDeclarationNode GetVar(this IWithBody container, string name)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                if (node is VarDeclarationNode && ((VarDeclarationNode)node).VarName.Value == name)
                    return (VarDeclarationNode)node;
            }
            return null;
        }
        /// <summary>
        /// Find method inside
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static MethodNode GetMethod(this ClassNode container, string name, PolyType[] args)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                if (node is MethodNode && ((MethodNode)node).MethodName.Value == name &&
                    ((MethodNode)node).MethodArgs.Args.ToTypes() == args)
                    return (MethodNode)node;
            }
            return null;
        }
        /// <summary>
        /// Find class constructor inside
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ClassCtorNode GetCtor(this ClassNode container, PolyType[] args)
        {
            foreach (IAstNode node in container.Body.Childs)
            {
                if (node is ClassCtorNode && ((ClassCtorNode)node).Args.Args.ToTypes() == args)
                    return (ClassCtorNode)node;
            }
            return null;
        }
        #endregion

        #region Allow Check Actions
        public static bool IsAllowedInNs<T>() where T : IAstNode
        {
            if (typeof(T) == typeof(ClassNode) ||
                typeof(T) == typeof(NamespaceStatementNode) ||
                typeof(T) == typeof(ImportStatementNode))
                return true;
            else
                return false;
        }
        public static bool IsAllowedInClass<T>()where T : IAstNode
        {
            if (typeof(T) == typeof(VarDeclarationNode) ||
                typeof(T) == typeof(ClassCtorNode) ||
                typeof(T) == typeof(MethodNode))
                return true;
            else
                return false;
        }
        public static bool IsAllowedInMethod<T>()where T : IAstNode
        {
            if (typeof(T) == typeof(VarDeclarationNode) ||
                typeof(T) == typeof(VarAssignNode))
                return true;
            else
                return false;
        }
        #endregion
    }
}
