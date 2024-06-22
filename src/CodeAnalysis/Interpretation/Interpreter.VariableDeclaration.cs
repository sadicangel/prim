using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static LiteralValue EvaluateVariableDeclaration(BoundVariableDeclaration node, InterpreterContext context)
    {
        var value = EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(node.VariableSymbol, value);
        return LiteralValue.Unit;
    }
}
