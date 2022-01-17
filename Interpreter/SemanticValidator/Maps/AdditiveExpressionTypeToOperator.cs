using System.Collections.Generic;
using Interpreter.ParserModule.Structures.Expressions.Types;

namespace Interpreter.SemanticValidator.Maps
{
    public static class AdditiveExpressionTypeToOperator
    {
        public static Dictionary<AdditiveExpressionType, string> Map = new()
        {
            {AdditiveExpressionType.Plus, "+"},
            {AdditiveExpressionType.Minus, "-"},
        };
    }
}