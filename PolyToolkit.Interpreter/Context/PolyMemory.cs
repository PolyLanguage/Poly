using System;
using System.Collections.Generic;

namespace PolyToolkit.Interpreter
{
    /// <summary>
    /// Interpretation memory stack handler
    /// </summary>
    public sealed class PolyMemory
    {
        public Stack<PolyScope> Stack { get; }

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
        public void NowClass(string className) => Now(className, PolyScope.Container.Class);
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
        public void NowLoop(string loopName) => Now(loopName, PolyScope.Container.Block);

        /// <summary>
        /// Pop scope from the stack
        /// </summary>
        /// <returns></returns>
        public PolyScope Pop() => Stack.Pop();
        /// <summary>
        /// Push scope to the stack
        /// </summary>
        /// <param name="scope"></param>
        public void Push(PolyScope scope) => Stack.Push(scope);
        #endregion

        #region Symbols
        /// <summary>
        /// Get value of symbol based on current scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PolySymbol GetValue(string name)
        {
            // loop all scopes
            int i = 0;
            foreach (PolyScope scope in Stack)
            {
                //not (method && next method)
                if (!(Current.ContainerType == PolyScope.Container.Method && i == 0 && scope.ContainerType == PolyScope.Container.Method))
                    if (scope.IsDefined(name))
                        return scope.Get(name);
                i++;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Define symbol in current scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="symbol"></param>
        public void Define(string name, PolySymbol symbol)
        {
            Current.Define(name, symbol);
        }
        #endregion
    }
}