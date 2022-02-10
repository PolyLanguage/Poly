namespace PolyToolkit.Interpreter
{
    /// <summary>
    /// Context exitpoint handler
    /// </summary>
    public struct Exitpoint
    {
        /// <summary>
        /// Type of value expected on exit
        /// </summary>
        public PolyType OutValueType { get; }
        /// <summary>
        /// Value passed on exit
        /// </summary>
        public PolySymbol OutValue { get; private set; }

        public bool IsBreaked { get; private set; }
        public bool IsExited { get; private set; }

        public Exitpoint(PolyType outValType)
        {
            OutValueType = outValType;
            OutValue = null;

            IsBreaked = false;
            IsExited = false;
        }

        /// <summary>
        /// Exit current context
        /// </summary>
        /// <param name="value"></param>
        public void Exit(PolySymbol value)
        {
            OutValue = value;
            IsExited = true;
        }

        /// <summary>
        /// Break current loop
        /// </summary>
        public void Break()
        {
            IsBreaked = true;
        }
    }
}
