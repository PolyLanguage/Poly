using System;

namespace PolyToolkit.Interpreter
{
    /// <summary>
    /// Represents entrypoint of the program
    /// </summary>
    public struct Entrypoint
    {
        public string Class { get; }
        public string Method { get; }

        public Entrypoint(string entrypointClass,string entrypointMethod)
        {
            this.Class = entrypointClass;
            this.Method = entrypointMethod;
        }
    }
}
