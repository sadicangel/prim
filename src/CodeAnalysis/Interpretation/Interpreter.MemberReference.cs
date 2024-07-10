using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateMemberReference(BoundMemberReference node, InterpreterContext context)
    {
        Debug.Assert(node.Symbol.ContainingSymbol is not null);
        var expression = EvaluateExpression(node.Expression, context);
        context.EvaluatedScope.Declare(VariableSymbol.This(expression.Type, node.Symbol.ContainingSymbol), expression, @throw: false);
        var memberReference = new ReferenceValue(
            node.Symbol.Type,
            () => expression.Get(node.Symbol),
            pv => expression.Set(node.Symbol, pv));
        return memberReference;
    }
}
