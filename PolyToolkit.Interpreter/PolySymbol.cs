namespace PolyToolkit.Interpreter
{
    public record PolySymbol
    {
        public PolyType Type { get; }
        public object Value { get; }

        public PolySymbol(PolyType type, object value)
        {
            this.Type = type;
            this.Value = value;
        }

        public static PolySymbol operator +(PolySymbol a, PolySymbol b)
        {
            //int
            if (a.Type == PolyType.IntType && a.Type == PolyType.IntType)
                return new PolySymbol(PolyType.IntType, (int)a.Value + (int)b.Value);
            //real
            else if (a.Type == PolyType.RealType && a.Type == PolyType.RealType)
                return new PolySymbol(PolyType.RealType, (double)a.Value + (double)b.Value);
            //string
            else if (a.Type == PolyType.StringType && a.Type == PolyType.StringType)
                return new PolySymbol(PolyType.StringType, (string)a.Value + (string)b.Value);
            //unknown
            else
            {
                return new PolySymbol(PolyType.NullType, null);
                //TODO: throw error
            }
        }
        public static PolySymbol operator -(PolySymbol a, PolySymbol b)
        {
            //int
            if (a.Type == PolyType.IntType && a.Type == PolyType.IntType)
                return new PolySymbol(PolyType.IntType, (int)a.Value - (int)b.Value);
            //real
            else if (a.Type == PolyType.RealType && a.Type == PolyType.RealType)
                return new PolySymbol(PolyType.RealType, (double)a.Value - (double)b.Value);
            //unknown
            else
            {
                return new PolySymbol(PolyType.NullType, null);
                //TODO: throw error
            }
        }

        //TODO: multiplication, dividing, modulus, etc
    }
}
