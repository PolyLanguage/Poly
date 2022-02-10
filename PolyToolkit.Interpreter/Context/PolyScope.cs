using System;
using System.Collections.Generic;

namespace PolyToolkit.Interpreter
{
    public sealed class PolyScope
    {
        public enum Container
        {
            Global,
            Class,
            Instance,
            Method,
            Block
        }

        public string Name { get; }
        public Container ContainerType { get; }
        public Dictionary<string, PolySymbol> ScopeMemory { get; private set; }

        public PolyScope(string name, Container container)
        {
            this.Name = name;
            this.ContainerType = container;
            this.ScopeMemory = new Dictionary<string, PolySymbol>();
        }

        /// <summary>
        /// Define value in the memory of scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Define(string name, PolySymbol value)
        {
            this.ScopeMemory.Add(name, value);
        }

        /// <summary>
        /// Assign value to existing symbol if possible
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newValue"></param>
        public void Assign(string name, PolySymbol newValue)
        {
            //if not constant
            if (IsConstant(name) == false)
                this.ScopeMemory[name] = newValue;
            else
                throw new Exception("Cannot assign value to constant or not defined value");
        }

        /// <summary>
        /// Check if value is defined in scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsDefined(string name)
        {
            return this.ScopeMemory.ContainsKey(name);
        }

        /// <summary>
        /// Check if value is defined and constant
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsConstant(string name)
        {
            return this.ScopeMemory.ContainsKey(name) && this.ScopeMemory[name].IsConstant;
        }

        /// <summary>
        /// Check if value is defined in scope and has type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PolyType IsDefinedOfType(string name)
        {
            PolySymbol value = this.Get(name);
            return value == null ? PolyTypes.Unknown : value.Type;
        }

        /// <summary>
        /// Get value of defined in scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PolySymbol Get(string name)
        {
            PolySymbol val = null;
            this.ScopeMemory.TryGetValue(name, out val);
            return val;
        }
    }
}
