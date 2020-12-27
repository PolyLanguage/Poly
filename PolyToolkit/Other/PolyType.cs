using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit
{
    public class PolyType
    {
        //default types
        public static PolyType StringType = new PolyType("string", true);
        public static PolyType IntType = new PolyType("int", false);
        public static PolyType RealType = new PolyType("real", false);
        public static PolyType BooleanType = new PolyType("bool", false);
        public static PolyType ObjectType = new PolyType("object", true);

        /// <summary>
        /// None type, used by methods that not returns anything
        /// </summary>
        public static PolyType NoneType = new PolyType("none", false);
        /// <summary>
        /// Unknown type, used when type is unknown or failed in conversation
        /// </summary>
        public static PolyType UnknownType = new PolyType("unknown", true);

        public static Dictionary<string, PolyType> SystemTypes = new Dictionary<string, PolyType>();

        //default literals
        public static string NullLiteral = "null";
        public static string TrueLiteral = "true";
        public static string FalseLiteral = "false";
        public static Dictionary<string, PolyType> SystemLiterals = new Dictionary<string, PolyType>();

        static PolyType()
        {
            SystemTypes[StringType.Name] = StringType;
            SystemTypes[IntType.Name] = IntType;
            SystemTypes[RealType.Name] = RealType;
            SystemTypes[BooleanType.Name] = BooleanType;
            SystemTypes[ObjectType.Name] = ObjectType;

            SystemLiterals[NullLiteral] = ObjectType;
            SystemLiterals[TrueLiteral] = BooleanType;
            SystemLiterals[FalseLiteral] = BooleanType;
        }

        public string Name { get; }
        public bool CanBeNull { get; }
        public Func<PolyType,bool> IsEquatable { get; }

        public PolyType(string name,bool canBeNull)
        {
            Name = name;
            CanBeNull = canBeNull;
        }

        /// <summary>
        /// Converts type name from string to type
        /// </summary>
        /// <param name="name">type name</param>
        /// <returns></returns>
        public static PolyType FromName(string name)
        {
            switch(name)
            {
                case "string":
                    return StringType;
                case "int":
                    return IntType;
                case "real":
                    return RealType;
                case "bool":
                    return BooleanType;
                case "var":
                    return ObjectType;
                case "object":
                    return ObjectType;
                default:
                    return PolyType.UnknownType;
            }
        }
        /// <summary>
        /// Identifies type from native value (string/int/etc)
        /// </summary>
        /// <param name="value">native value</param>
        /// <returns></returns>
        public static PolyType FromNativeValue(object value)
        {
            if (value is string)
                return StringType;
            else if (value is int)
                return IntType;
            else if (value is double)
                return RealType;
            else if (value is bool)
                return BooleanType;
            else if (value == null)
                return ObjectType;
            else
                return PolyType.UnknownType;
        }
        
        public static bool IsItLiteral(string value)
        {
            if (SystemLiterals.ContainsKey(value))
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

        public Type ToNativeType()
        {
            switch (Name)
            {
                case "string":
                    return typeof(string);
                case "int":
                    return typeof(int);
                case "real":
                    return typeof(double);
                case "bool":
                    return typeof(bool);
                case "var":
                    return typeof(object);
                case "object":
                    return typeof(object);
                default:
                    return null;
            }
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
        //extensions
        public override string ToString()
        {
            return this.Name;
        }
        public override bool Equals(object obj)
        {
            return obj is PolyType type &&
                   Name == type.Name &&
                   CanBeNull == type.CanBeNull;
        }

        public static bool operator ==(PolyType a, PolyType b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;

            return a.Name == b.Name;
        }
        public static bool operator !=(PolyType a, PolyType b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return true;

            return a.Name != b.Name;
        }
    }
}
