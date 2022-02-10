using System;
using System.Collections;
using System.Collections.Generic;

using PolyToolkit.Parsing.Ast;
namespace PolyToolkit
{
    /// <summary>
    /// Represents type
    /// </summary>
    public abstract class PolyType
    {
        public string Name { get; }
        public virtual object DefaultValue { get; } = null;

        public virtual bool CanBeNull { get; } = true;
        public virtual bool CanBeCreated { get; } = true;
        public virtual bool CanBeTemplated { get; } = false;

        public virtual bool IsEnumerable { get; } = false;

        private PolyType _base;
        public PolyType Base { get { return _base; } protected set { _base = value; } }

        public PolyType T { get; protected set; }

        public PolyType(string name)
        {
            Name = name;
            T = null;
        }

        /// <summary>
        /// Set T of type
        /// </summary>
        /// <param name="t"></param>
        public PolyType Of(PolyType t)
        {
            PolyType cloned = (PolyType)this.MemberwiseClone();
            cloned.T = t;

            return cloned;
        }

        /// <summary>
        /// Check if value is of that type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool IsValue(object value);
        /// <summary>
        /// As native type
        /// </summary>
        /// <returns></returns>
        public abstract Type ToNativeType();

        public static bool operator ==(PolyType a, PolyType b)
        {
            if (a is null || b is null)
                return false;
            else
            {
                if (a.T is null && b.T is null)
                    return a.Name == b.Name;
                else if (a.T is null || b.T is null)
                    return false;
                else
                    return a.T == b.T;
            }
        }
        public static bool operator !=(PolyType a, PolyType b)
        {
            if (a is null || b is null)
                return true;
            else
            {
                if (a.T is null && b.T is null)
                    return a.Name != b.Name;
                else if (a.T is null || b.T is null)
                    return true;
                else
                    return a.T != b.T;
            }
        }

        //TODO: true -> T is null/object.ReferenceEquals(T, null)
        public override string ToString() => T is null ? Name : $"{Name}<{T}>";

        /// <summary>
        /// Can operation perfomed between two values of this type
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool IsPerformable(MathOperation op, PolyType other)
        {
            switch (op)
            {
                case MathOperation.Plus:
                    return this is PolyTypeInt && other is PolyTypeInt ||
                        this is PolyTypeReal && other is PolyTypeReal ||
                        this is PolyTypeString && other is PolyTypeString ||
                        this is PolyTypeArray && other is PolyTypeArray;
                case MathOperation.Minus:
                case MathOperation.Multiply:
                case MathOperation.Divide:
                case MathOperation.Modulus:
                    return this is PolyTypeInt && other is PolyTypeInt ||
                        this is PolyTypeReal && other is PolyTypeReal;
                case MathOperation.Less:
                case MathOperation.LessOrEquals:
                case MathOperation.More:
                case MathOperation.MoreOrEquals:
                    return this is PolyTypeInt && other is PolyTypeInt ||
                        this is PolyTypeReal && other is PolyTypeReal ||
                        this is PolyTypeString && other is PolyTypeString ||
                        this is PolyTypeArray && other is PolyTypeArray;
                case MathOperation.Equals:
                case MathOperation.NotEquals:
                    return true;
                case MathOperation.And:
                case MathOperation.Or:
                    return this is PolyTypeBool && other is PolyTypeBool;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Type for type definitions
    /// </summary>
    public sealed class PolyTypeClass : PolyType
    {
        public override bool CanBeNull { get; } = true;
        public override bool CanBeCreated { get; } = true;

        public PolyTypeClass() : base("class") { }

        public override bool IsValue(object value) => value is NonComputedClass;
        public override Type ToNativeType() => typeof(NonComputedClass);
    }

    /// <summary>
    /// Type for instances of defined types
    /// </summary>
    public sealed class PolyTypeInstance : PolyType
    {
        public override bool CanBeNull { get; } = true;
        public override bool CanBeCreated { get; } = true;

        public PolyTypeInstance() : base("instance") { }

        public override bool IsValue(object value) => value is NonComputedInstance;
        public override Type ToNativeType() => typeof(NonComputedInstance);
    }

    /// <summary>
    /// Base object type for other types
    /// </summary>
    public class PolyTypeObject : PolyType
    {
        public override bool CanBeNull { get; } = true;
        public override bool CanBeCreated { get; } = true;

        public PolyTypeObject() : base("object") { }
        public PolyTypeObject(string name) : base(name) { }

        public override bool IsValue(object value) => value is object;
        public override Type ToNativeType() => typeof(object);

        //TODO: base methods
    }

    /// <summary>
    /// Type for not identified values
    /// </summary>
    public sealed class PolyTypeUnknown : PolyType
    {
        public override object DefaultValue { get; } = null;
        public override bool CanBeNull { get; } = true;
        public override bool CanBeCreated { get; } = false;

        public PolyTypeUnknown() : base("unknown") { }

        public override bool IsValue(object value) => false;
        public override Type ToNativeType() => typeof(void);
    }
    /// <summary>
    /// Null literal type, means 'nothing'
    /// </summary>
    public sealed class PolyTypeNull : PolyType
    {
        public override object DefaultValue { get; } = null;
        public override bool CanBeNull { get; } = true;
        public override bool CanBeCreated { get; } = false;

        public PolyTypeNull() : base("null") { }

        public override bool IsValue(object value) => false;
        public override Type ToNativeType() => typeof(void);
    }
    /// <summary>
    /// Void type for void methods
    /// </summary>
    public sealed class PolyTypeVoid : PolyType
    {
        public override object DefaultValue { get; } = null;
        public override bool CanBeNull { get; } = true;
        public override bool CanBeCreated { get; } = false;

        public PolyTypeVoid() : base("void") { }

        public override bool IsValue(object value) => false;
        public override Type ToNativeType() => typeof(void);
    }

    /// <summary>
    /// Type for integer numbers (Example: 123)
    /// </summary>
    public sealed class PolyTypeInt : PolyTypeObject
    {
        public override object DefaultValue { get; } = 0;
        public override bool CanBeNull { get; } = false;

        public PolyTypeInt() : base("int") { }

        public override bool IsValue(object value) => value is int;
        public override Type ToNativeType() => typeof(int);
    }
    /// <summary>
    /// Type for double/float numbers (Example: 1.23)
    /// </summary>
    public sealed class PolyTypeReal : PolyTypeObject
    {
        public override object DefaultValue { get; } = double.NaN;
        public override bool CanBeNull { get; } = false;

        public PolyTypeReal() : base("real") { }

        public override bool IsValue(object value) => value is double;
        public override Type ToNativeType() => typeof(double);
    }
    /// <summary>
    /// Type for booleans (Example: true/false)
    /// </summary>
    public sealed class PolyTypeBool : PolyTypeObject
    {
        public override object DefaultValue { get; } = false;
        public override bool CanBeNull { get; } = false;

        public PolyTypeBool() : base("bool") { }

        public override bool IsValue(object value) => value is bool;
        public override Type ToNativeType() => typeof(bool);
    }
    /// <summary>
    /// Type for texts (Example: 'hello')
    /// </summary>
    public sealed class PolyTypeString : PolyTypeObject
    {
        public override object DefaultValue { get; } = string.Empty;

        public PolyTypeString() : base("string") { }

        public override bool IsValue(object value) => value is string;
        public override Type ToNativeType() => typeof(string);
    }
    /// <summary>
    /// Type for functions
    /// </summary>
    public sealed class PolyTypeMethod : PolyTypeObject
    {
        public override object DefaultValue { get; } = null;

        public PolyTypeMethod() : base("method") { }

        public override bool IsValue(object value) => value is NonComputedMethod;
        public override Type ToNativeType() => typeof(NonComputedMethod);
    }

    /// <summary>
    /// Type for arrays
    /// </summary>
    public sealed class PolyTypeArray : PolyTypeObject
    {
        public override object DefaultValue { get; } = new ArrayList();
        public override bool CanBeTemplated { get; } = true;
        public override bool IsEnumerable { get; } = true;

        public PolyTypeArray() : base("array")
        {
            T = PolyTypes.Unknown;
        }

        public override bool IsValue(object value) => value.GetType().IsArray;
        public override Type ToNativeType() => typeof(ArrayList);
    }

    //TODO: type for lists, dictionaries, stacks, hashmaps

    public static class PolyTypes
    {
        //default none types
        public static PolyType Unknown = new PolyTypeUnknown();
        public static PolyType Null = new PolyTypeNull();
        public static PolyType Void = new PolyTypeVoid();

        //default basic types
        public static PolyType Int = new PolyTypeInt();
        public static PolyType Real = new PolyTypeReal();
        public static PolyType Bool = new PolyTypeBool();
        public static PolyType String = new PolyTypeString();

        //default types with T
        public static PolyType Array = new PolyTypeArray();

        //default other types
        public static PolyType Object = new PolyTypeObject();
        public static PolyType Class = new PolyTypeClass();
        public static PolyType Instance = new PolyTypeInstance();
        public static PolyType Method = new PolyTypeMethod();

        public static Dictionary<string, PolyType> SystemTypes { get; } = new Dictionary<string, PolyType>();
        static PolyTypes()
        {
            //default none types
            SystemTypes[Unknown.Name] = Unknown;
            SystemTypes[Null.Name] = Null;
            SystemTypes[Void.Name] = Void;

            //default basic types
            SystemTypes[Int.Name] = Int;
            SystemTypes[Real.Name] = Real;
            SystemTypes[Bool.Name] = Bool;
            SystemTypes[String.Name] = String;

            //default types with T
            SystemTypes[Array.Name] = Array;

            //default other types
            SystemTypes[Object.Name] = Object;
            SystemTypes[Method.Name] = Method;
        }

        /// <summary>
        /// Converts type name from string to type
        /// </summary>
        /// <param name="name">type name</param>
        /// <returns></returns>
        public static PolyType FromName(string name)
        {
            foreach(string typeName in SystemTypes.Keys)
            {
                if (typeName == name)
                    return SystemTypes[typeName];
            }

            return PolyTypes.Unknown;
        }

        /// <summary>
        /// Identifies type from AST node
        /// </summary>
        /// <param name="node">AST node</param>
        /// <returns></returns>
        public static PolyType FromNode(AstNode node)
        {
            if (node is ExpressionNode)
                return ((ExpressionNode)node).Type;
            else if (node is MethodNode)
                return PolyTypes.Method;
            else
                return PolyTypes.Unknown;
        }

        public static bool IsItLiteral(string value)
        {
            if (value == "true" || value == "false" || value == "null")
                return true;
            else
                return false;
        }
        public static bool IsItTypeName(string value)
        {
            if (SystemTypes.ContainsKey(value))
                return true;
            else
                return false;
        }
        public static object LiteralToNative(string literalStr)
        {
            switch(literalStr)
            {
                case "true":
                    return true;
                case "false":
                    return false;
                case "null":
                    return null;
                default:
                    return literalStr;
            }
        }
    }
}