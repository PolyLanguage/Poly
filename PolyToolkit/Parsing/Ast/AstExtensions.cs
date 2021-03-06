﻿using System.Collections.Generic;
using System.Linq;

using PolyToolkit;
using PolyToolkit.Parsing;
namespace PolyToolkit.Parsing.Ast
{
    public static class AstExtensions
    {
        #region IsContains
        /// <summary>
        /// Check if container has node of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static bool IsContainsNodeIn<T>(this IAstNode container) where T: IAstNode
        {
            foreach (IAstNode node in container.Childs)
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
            foreach (IAstNode node in container.Childs)
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
            foreach (IAstNode node in container.Childs)
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
            foreach (IAstNode node in container.Childs)
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
        public static bool IsAllowedInGlobal<T>() where T : IAstNode
        {
            if (typeof(T) == typeof(ClassNode) ||
                typeof(T) == typeof(NamespaceStmtNode) ||
                typeof(T) == typeof(ImportStmtNode))
                return true;
            else
                return false;
        }
        public static bool IsAllowedInClass<T>()where T : IAstNode
        {
            if (typeof(T) == typeof(VarDeclarationStmtNode) ||
                typeof(T) == typeof(ClassCtorNode) ||
                typeof(T) == typeof(MethodNode))
                return true;
            else
                return false;
        }
        public static bool IsAllowedInMethod<T>()where T : IAstNode
        {
            if (typeof(T) == typeof(VarDeclarationStmtNode) ||
                typeof(T) == typeof(VarAssignStmtNode) ||
                typeof(T) == typeof(ReturnStmtNode))
                return true;
            else
                return false;
        }
        #endregion

        #region Get
        /// <summary>
        /// Get first parent of node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T GetFirstParent<T>(this IAstNode node) where T : IAstNode
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
            foreach (IAstNode node in container.Childs)
            {
                if (node is ClassNode && ((ClassNode)node).ClassName == name)
                    return (ClassNode)node;
            }
            return null;
        }
        /// <summary>
        /// Find variable in scope
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PolyType GetFirstParentVar(this IAstNode container, string name)
        {
            //check in args (if method or ctor)
            if (container is MethodNode)
            {
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
                        ((ClassCtorNode)container).CtorArgs.TryGetValue(arg,out val);
                        return val;
                    }
                }
            }

            //in current container
            if (container is IWithBody)
            {
                foreach (IAstNode node in container.Childs)
                {
                    if (node is VarDeclarationStmtNode && ((VarDeclarationStmtNode)node).VarName == name)
                        return ((VarDeclarationStmtNode)node).VarType;
                }
            }

            //in parent container
            if(container.Parent != null && container.Parent is CodeTree == false)
            {
                return container.Parent.GetFirstParentVar(name);
            }

            return PolyType.UnknownType;
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
            foreach (IAstNode node in container.Childs)
            {
                if (node is MethodNode && ((MethodNode)node).MethodName == name &&
                    ((MethodNode)node).MethodArgs.Values.ToList() == args)
                    return (MethodNode)node;
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
            foreach (IAstNode node in container.Childs)
            {
                if (node is ClassCtorNode && ((ClassCtorNode)node).CtorArgs.Values.ToList() == args)
                    return (ClassCtorNode)node;
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
            return container.GetCtor(args.ToList());
        }
        #endregion

        #region Other
        /// <summary>
        /// Check if not all code paths is returns value
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static bool IsAllCodePathsReturns(this IWithBody container)
        {
            //in current container
            bool current = false;
            foreach(IAstNode node in container.Childs)
            {
                //current returns if has ReturnStatement or has Else node
                if (node is ReturnStmtNode || node is ElseNode)
                    current = true;

                if (node is IWithBody && !((IWithBody)node).IsAllCodePathsReturns())
                    return false;
            }
            return current;
        }
        /// <summary>
        /// Checks if variable available in available scopes
        /// </summary>
        /// <param name="curnode"></param>
        /// <param name="varname"></param>
        /// <returns></returns>
        public static bool IsVariableAvailable(this IAstNode container, string varname)
        {
            //check in current body
            if (container is IWithBody)
            {
                if (IsVariableAvailableIn((IWithBody)container, varname))
                    return true;
            }

            //check in parent body
            if (container.Parent != null && container.Parent is CodeTree == false)
            {
                return container.Parent.IsVariableAvailable(varname);
            }

            return false;
        }
        /// <summary>
        /// Checks if variable available in current scopes
        /// </summary>
        /// <param name="curnode"></param>
        /// <param name="varname"></param>
        /// <returns></returns>
        public static bool IsVariableAvailableIn(this IWithBody container, string varname)
        {
            //check in args (if method or ctor)
            if (container is MethodNode)
            {
                foreach (string arg in ((MethodNode)container).MethodArgs.Keys)
                {
                    if (arg == varname)
                        return true;
                }
            }
            else if (container is ClassCtorNode)
            {
                foreach (string arg in ((ClassCtorNode)container).CtorArgs.Keys)
                {
                    if (arg == varname)
                        return true;
                }
            }

            //check in current body
            foreach (IAstNode node in container.Childs)
            {
                if (node is VarDeclarationStmtNode && ((VarDeclarationStmtNode)node).VarName == varname)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
