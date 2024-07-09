using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateMemberReference(BoundMemberReference node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(VariableSymbol.This(expression.Type), expression, @throw: false);
        var memberReference = new ReferenceValue(
            node.NameSymbol.Type,
            () => expression.Get(node.NameSymbol),
            pv => expression.Set(node.NameSymbol, pv));
        return memberReference;
    }
}
