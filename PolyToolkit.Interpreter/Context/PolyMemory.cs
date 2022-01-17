using System;
using System.Collections.Generic;

namespace PolyToolkit.Interpreter
{
    /// <summary>
    /// Interpretation memory stack handler
    /// </summary>
    public sealed class PolyMemory
    {
        private Stack<PolyScope> Stack;

        /// <summary>
        /// Get scope at the top of the stack
        /// </summary>
        public PolyScope Current { get { return Stack.Peek(); } }

        /// <summary>
        /// Is memory ready (is global scope defined atleast?)
        /// </summary>
        public bool IsReady { get; private set; }

        public PolyMemory()
        {
            Stack = new Stack<PolyScope>();
        }

        #region Scope
        /// <summary>
        /// Push scope to the memory stack
        /// </summary>
        /// <param name="name"></param>
        /// <param name="container"></param>
        private void Now(string name, PolyScope.Container container) => Stack.Push(new PolyScope(name, container));

        /// <summary>
        /// Push global scope to the memory stack
        /// </summary>
        public void NowGlobal()
        {
            Now("global", PolyScope.Container.Global);
            IsReady = true;
        }
        /// <summary>
        /// Push class instance scope to the memory stack
        /// </summary>
        public void NowInstance(string className) => Now(className, PolyScope.Container.Instance);
        /// <summary>
        /// Push method scope to the memory stack
        /// </summary>
        public void NowMethod(string methodName) => Now(methodName, PolyScope.Container.Global);
        /// <summary>
        /// Push loop scope to the memory stack
        /// </summary>
        public void NowLoop(string loopName) => Now(loopName, PolyScope.Container.Loop);

        public PolyScope Back() => this.Stack.Pop();
        #endregion

        #region Symbols
        /// <summary>
        /// Check if symbol defined in current scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsDefined(string name)
        {
            return Stack.Peek().IsDefined(name);
        }
        public void Define(string name, PolySymbol symbol)
        {
            Stack.Peek().Define(name, symbol);
        }
        #endregion
    }
}