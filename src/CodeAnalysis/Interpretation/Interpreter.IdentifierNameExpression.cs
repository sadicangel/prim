using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateIdentifierNameExpression(BoundIdentifierNameExpression node, InterpreterContext context)
    {
        return context.EvaluatedScope.Lookup(node.NameSymbol);
    }
}
