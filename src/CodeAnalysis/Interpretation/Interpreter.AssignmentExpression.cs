using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static PrimValue EvaluateAssignmentExpression(BoundAssignmentExpression node, InterpreterContext context)
    {
        // TODO: Allow non identifier expressions.
        if (node.Left is not BoundIdentifierNameExpression identifierName)
        {
            throw new NotSupportedException($"Unexpected left hand side of {nameof(BoundAssignmentExpression)} '{node.Left.BoundKind}'");
        }

        var value = EvaluateExpression(node.Right, context);
        context.EvaluatedScope.Declare(identifierName.NameSymbol, value);
        return value;
    }
}
