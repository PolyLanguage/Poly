using System;
using System.Collections;

namespace PolyToolkit.Interpreter
{
    /// <summary>
    /// Symbol that contains native value
    /// </summary>
    public record PolySymbol
    {
        /// <summary>
        /// Type of value
        /// </summary>
        public PolyType Type { get; }
        /// <summary>
        /// Native value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Is value constant
        /// </summary>
        public bool IsConstant { get; }

        /// <summary>
        /// Create new symbol
        /// </summary>
        /// <param name="type">type of value</param>
        /// <param name="value">native value</param>
        /// <param name="constant">is value constant (cannot be changed)</param>
        public PolySymbol(PolyType type, object value, bool constant = false)
        {
            Type = type;
            Value = value;
            IsConstant = constant;

            if (!IsValid())
                throw new InvalidCastException("Not valid symbol value");
        }

        private bool IsValid() => (this.Value == null && this.Type.CanBeNull) || (this.Value.GetType() == this.Type.ToNativeType());

        public bool IsInt { get => this.Type.Name == PolyTypes.Int.Name; }
        public bool IsReal { get => this.Type.Name == PolyTypes.Real.Name; }
        public bool IsBool { get => this.Type.Name == PolyTypes.Bool.Name; }
        public bool IsString { get => this.Type.Name == PolyTypes.String.Name; }
        public bool IsArray { get => this.Type.Name == PolyTypes.Array.Name; }
        public bool IsClass { get => this.Type.Name == PolyTypes.Class.Name; }
        public bool IsInstance { get => this.Type.Name == PolyTypes.Instance.Name; }
        public bool IsMethod { get => this.Type.Name == PolyTypes.Method.Name; }
    }
}