using System.Collections;

namespace PolyToolkit.Interpreter
{
    public static class PolyTypeExtensions
    {
        /// <summary>
        /// Perform operation
        /// </summary>
        /// <param name="type"></param>
        /// <param name="op"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static PolySymbol Perform(this PolySymbol a, MathOperation op, PolySymbol b)
        {
            PolyType type = a.Type;

            //can be perfomed?
            if (type.IsPerformable(op, b.Type))
                switch (op)
                {
                    case MathOperation.Plus:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Int, (int)a.Value + (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Real, (double)a.Value + (double)b.Value);
                        else if (type is PolyTypeString)
                            return new PolySymbol(PolyTypes.String, (string)a.Value + (string)b.Value);
                        else if (type is PolyTypeArray)
                        {
                            //add B array to array A
                            ArrayList arrA = (ArrayList)a.Value;
                            arrA.AddRange((ArrayList)b.Value);

                            return new PolySymbol(a.Type, arrA);
                        }
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Minus:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Int, (int)a.Value - (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Real, (double)a.Value - (double)b.Value);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Multiply:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Int, (int)a.Value * (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Real, (double)a.Value * (double)b.Value);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Divide:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Int, (int)a.Value / (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Real, (double)a.Value / (double)b.Value);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Modulus:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Int, (int)a.Value % (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Real, (double)a.Value % (double)b.Value);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Less:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Bool, (int)a.Value < (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Bool, (double)a.Value < (double)b.Value);
                        else if (type is PolyTypeString)
                            return new PolySymbol(PolyTypes.Bool, ((string)a.Value).Length < ((string)b.Value).Length);
                        else if (type is PolyTypeArray)
                            return new PolySymbol(PolyTypes.Bool, ((ArrayList)a.Value).Count < ((ArrayList)b.Value).Count);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.LessOrEquals:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Bool, (int)a.Value <= (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Bool, (double)a.Value <= (double)b.Value);
                        else if (type is PolyTypeString)
                            return new PolySymbol(PolyTypes.Bool, ((string)a.Value).Length <= ((string)b.Value).Length);
                        else if (type is PolyTypeArray)
                            return new PolySymbol(PolyTypes.Bool, ((ArrayList)a.Value).Count <= ((ArrayList)b.Value).Count);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.More:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Bool, (int)a.Value > (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Bool, (double)a.Value > (double)b.Value);
                        else if (type is PolyTypeString)
                            return new PolySymbol(PolyTypes.Bool, ((string)a.Value).Length > ((string)b.Value).Length);
                        else if (type is PolyTypeArray)
                            return new PolySymbol(PolyTypes.Bool, ((ArrayList)a.Value).Count > ((ArrayList)b.Value).Count);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.MoreOrEquals:
                        if (type is PolyTypeInt)
                            return new PolySymbol(PolyTypes.Bool, (int)a.Value >= (int)b.Value);
                        else if (type is PolyTypeReal)
                            return new PolySymbol(PolyTypes.Bool, (double)a.Value >= (double)b.Value);
                        else if (type is PolyTypeString)
                            return new PolySymbol(PolyTypes.Bool, ((string)a.Value).Length >= ((string)b.Value).Length);
                        else if (type is PolyTypeArray)
                            return new PolySymbol(PolyTypes.Bool, ((ArrayList)a.Value).Count >= ((ArrayList)b.Value).Count);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Equals:
                        return new PolySymbol(PolyTypes.Bool, a.Type == b.Type && a.Value == b.Value);
                    case MathOperation.NotEquals:
                        return new PolySymbol(PolyTypes.Bool, a.Type != b.Type && a.Value != b.Value);
                    case MathOperation.And:
                        if (type is PolyTypeBool)
                            return new PolySymbol(PolyTypes.Int, (bool)a.Value && (bool)b.Value);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    case MathOperation.Or:
                        if (type is PolyTypeBool)
                            return new PolySymbol(PolyTypes.Int, (bool)a.Value || (bool)b.Value);
                        else
                            return new PolySymbol(PolyTypes.Unknown, null);
                    default:
                        return new PolySymbol(PolyTypes.Unknown, null);
                }
            //error...
            else
                return new PolySymbol(PolyTypes.Unknown, null);
        }
    }
}
