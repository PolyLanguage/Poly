using System.Collections.Generic;
using System.Linq;

using PolyToolkit;
using PolyToolkit.Parsing;
namespace PolyToolkit.Parsing.Ast
{
    public static class AstExtensions
    {
        #region IsInside
        /// <summary>
        /// Is node inside loop?
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static bool IsInsideLoop(this AstNode container)
        {
            return container.GetFirstParent<RepeatNode>() != null;
        }
        #endregion
        #region IsContains
        /// <summary>
        /// Check if container has node of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static bool IsContainsNodeIn<T>(this AstNode container) where T: AstNode
        {
            foreach (AstNode node in container.Childs)
            {
                if (node is T)
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
        public static bool IsContainsClassIn(this CodeTree container, string name)
        {
            foreach (AstNode node in container.Childs)
            {
                if (node is ClassNode && ((ClassNode)node).ClassName == name)
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
        public static bool IsContainsMethodIn(this ClassNode container, string name, List<PolyType> args)
        {
            foreach (AstNode node in container.Childs)
            {
                //if:
                //1. is method
                //2. method is identical
                //3. args types is identical
                if (node is MethodNode && ((MethodNode)node).MethodName == name &&
                    ((MethodNode)node).MethodArgs.Values.ToList() == args)
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
        public static bool IsContainsMethodIn(this ClassNode container, string name, PolyType[] args)
        {
            return container.IsContainsMethodIn(name, args.ToList());
        }
        /// <summary>
        /// Checks if class got ctor inside (not inside of childs)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsContainsCtorIn(this ClassNode container, List<PolyType> args)
        {
            foreach (AstNode node in container.Childs)
            {
                //if:
                //1. is ctor
                //2. args is identical
                if (node is ClassCtorNode && ((ClassCtorNode)node).CtorArgs.Values.ToList() == args)
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
        public static bool IsContainsCtorIn(this ClassNode container, PolyType[] args)
        {
            return container.IsContainsCtorIn(args.ToList());
        }
        #endregion

        #region IsAllowedIn
        public static bool IsAllowedInGlobal<T>() where T : AstNode
        {
           return typeof(T) == typeof(ClassNode) ||
                typeof(T) == typeof(NamespaceStmtNode) ||
                typeof(T) == typeof(ImportStmtNode);
        }
        public static bool IsAllowedInClass<T>()where T : AstNode
        {
            return typeof(T) == typeof(FieldNode) ||
                typeof(T) == typeof(ClassCtorNode) ||
                typeof(T) == typeof(MethodNode);
        }
        public static bool IsAllowedInMethod<T>()where T : AstNode
        {
            return typeof(T) == typeof(ExpressionNode) ||
                typeof(T) == typeof(VarDeclarationStmtNode) ||
                typeof(T) == typeof(VarAssignStmtNode) ||
                typeof(T) == typeof(ReturnStmtNode) ||
                typeof(T) == typeof(BreakStmtNode) ||
                typeof(T) == typeof(IfNode) ||
                typeof(T) == typeof(ElseIfNode) ||
                typeof(T) == typeof(ElseNode) ||
                typeof(T) == typeof(RepeatNode);
        }
        public static bool IsAllowedInCondition<T>() where T : AstNode
        {
            return IsAllowedInMethod<T>();
        }
        public static bool IsAllowedInLoop<T>() where T : AstNode
        {
            return IsAllowedInMethod<T>();
        }
        #endregion

        #region Get
        /// <summary>
        /// Get first parent of node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T GetFirstParent<T>(this AstNode node) where T : AstNode
        {
            if (node.Parent != null)
            {
                if (node.Parent is T)
                    return (T)node.Parent;
                else
                    return node.Parent.GetFirstParent<T>();
            }
            else
                return default(T);
        }
        /// <summary>
        /// Find class inside tree
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ClassNode GetClassIn(this CodeTree container, string name)
        {
            foreach (AstNode node in container.Childs)
            {
                if (node is ClassNode && ((ClassNode)node).ClassName == name)
                    return (ClassNode)node;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetIsConstant(this AstNode container, string name)
        {
            //check in args (if method or ctor)
            if (container is MethodNode)
            {
                foreach (string arg in ((MethodNode)container).MethodArgs.Keys)
                    //args are constants
                    if (arg == name)
                        return true;
            }
            else if (container is ClassCtorNode)
            {
                foreach (string arg in ((ClassCtorNode)container).CtorArgs.Keys)
                    //args are constants
                    if (arg == name)
                        return true;
            }

            //in current container
            if (container is BlockNode)
                foreach (AstNode node in container.Childs)
                    //is var constant
                    if (node is VarDeclarationStmtNode && ((VarDeclarationStmtNode)node).VarName == name)
                        return ((VarDeclarationStmtNode)node).IsConstant;
                    //is field constant
                    else if (node is FieldNode && ((FieldNode)node).VarName == name)
                        return ((FieldNode)node).IsConstant;
                    //methods and classes are constants
                    else if (node is MethodNode && ((MethodNode)node).MethodName == name)
                        return true;
                    else if (node is ClassNode && ((ClassNode)node).ClassName == name)
                        return true;
                    //TODO: CtorNode

            //in parent container
            if (container.Parent != null && container.Parent is CodeTree == false)
                return container.Parent.GetIsConstant(name);


            IDescriptor libEntityDescriptor = SystemLibrary.GetModule("@").Get(name);
            //in system library (method)
            if (libEntityDescriptor != null && libEntityDescriptor is MethodDescriptor)
                return true;
            //in system library (field)
            if (libEntityDescriptor != null && libEntityDescriptor is FieldDescriptor)
                return !((FieldDescriptor)libEntityDescriptor).SetAllowed;

            return false;
        }

        /// <summary>
        /// Find name type in scope
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PolyType GetNameType(this AstNode container, string name)
        {
            if (container is MethodNode)
            {
                // Method itself
                if (((MethodNode)container).MethodName == name)
                    return PolyTypes.Method;
                // Method arg
                foreach (string arg in ((MethodNode)container).MethodArgs.Keys)
                {
                    if (arg == name)
                    {
                        PolyType val;
                        ((MethodNode)container).MethodArgs.TryGetValue(arg, out val);
                        return val;
                    }
                }
            }
            else if (container is ClassCtorNode)
            {
                foreach (string arg in ((ClassCtorNode)container).CtorArgs.Keys)
                {
                    if (arg == name)
                    {
                        PolyType val;
                        ((ClassCtorNode)container).CtorArgs.TryGetValue(arg, out val);
                        return val;
                    }
                }
            }
            else if (container is ClassNode)
            {
                // Class itself
                if (((ClassNode)container).ClassName == name)
                    return PolyTypes.Class;
            }

            //in current container
            if (container is BlockNode)
                foreach (AstNode node in container.Childs)
                    if (node is VarDeclarationStmtNode && ((VarDeclarationStmtNode)node).VarName == name)
                        return ((VarDeclarationStmtNode)node).VarType;
                    else if (node is FieldNode && ((FieldNode)node).VarName == name)
                        return ((FieldNode)node).VarType;
                    else if (node is MethodNode && ((MethodNode)node).MethodName == name)
                        return PolyTypes.Method;
                    //TODO: else if(node is ClassNode && .ClassName == name) return type
                    //TODO: CtorNode

            //in parent container
            if (container.Parent != null && container.Parent is CodeTree == false)
                return container.Parent.GetNameType(name);

            IDescriptor libEntityDescriptor = SystemLibrary.GetModule("@").Get(name);
            //in system library (method)
            if (libEntityDescriptor != null && libEntityDescriptor is MethodDescriptor)
                return PolyTypes.Method;
            //in system library (field)
            if (libEntityDescriptor != null && libEntityDescriptor is FieldDescriptor)
                return ((FieldDescriptor)libEntityDescriptor).Type;

            return PolyTypes.Unknown;
        }
        /// <summary>
        /// Find name in scope
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PolyType GetMethodType(this AstNode container, string name)
        {
            //container itself
            if (container is MethodNode && ((MethodNode)container).MethodName == name)
                return ((MethodNode)container).MethodReturnType;

            //in current container
            if (container is BlockNode)
                foreach (AstNode node in container.Childs)
                    if (node is MethodNode && ((MethodNode)node).MethodName == name)
                        return ((MethodNode)node).MethodReturnType;

            //in parent container
            if (container.Parent != null && container.Parent is CodeTree == false)
                return container.Parent.GetMethodType(name);



            MethodDescriptor methodDescriptor = SystemLibrary.GetModule("@").GetMethod(name);
            //in system library (method)
            if (methodDescriptor != null)
                return methodDescriptor.Returns;

            return PolyTypes.Unknown;
        }

        /// <summary>
        /// Find method inside
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static MethodNode GetMethod(this ClassNode container, string name, List<PolyType> args)
        {
            foreach (AstNode node in container.Childs)
                if (node is MethodNode && ((MethodNode)node).MethodName == name &&
                        ((MethodNode)node).MethodArgs.Values.ToList() == args)
                            return (MethodNode)node;

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
            return container.GetMethod(name, args.ToList());
        }
        /// <summary>
        /// Find class constructor inside
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ClassCtorNode GetCtor(this ClassNode container, List<PolyType> args)
        {
            foreach (AstNode node in container.Childs)
                if (node is ClassCtorNode && ((ClassCtorNode)node).CtorArgs.Values.ToList() == args)
                    return (ClassCtorNode)node;

            return null;
        }
        #endregion

        #region Other
        /// <summary>
        /// Check if not all code paths is returns value
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static bool IsAllCodePathsReturns(this BlockNode container)
        {
            //in current container
            bool current = false;
            foreach(AstNode node in container.Childs)
            {
                //current returns if has ReturnStatement or has Else node
                if (node is ReturnStmtNode || node is ElseNode)
                    current = true;

                if (node is BlockNode && !((BlockNode)node).IsAllCodePathsReturns())
                    return false;
            }

            return current;
        }

        /// <summary>
        /// Checks if name available in scopes
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsNameIn(this AstNode container, string name)
        {
            return container.GetNameType(name) != PolyTypes.Unknown;
        }
        #endregion
    }
}
