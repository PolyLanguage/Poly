using System;
using System.Collections.Generic;
using PolyToolkit.Parsing.Ast;

namespace PolyToolkit.Interpreter
{
    public sealed class PolyScope
    {
        public enum Container
        {
            Class,
            Method,
            Loop
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
        /// Check if value is defined in scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsDefined(string name)
        {
            return this.ScopeMemory.ContainsKey(name);
        }

        /// <summary>
        /// Check if value is defined in scope and has type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PolyType IsDefinedOfType(string name)
        {
            PolySymbol value = this.Get(name);
            return value == null ? PolyType.UnknownType : value.Type;
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
