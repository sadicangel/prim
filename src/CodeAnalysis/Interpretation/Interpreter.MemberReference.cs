using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    private static ReferenceValue EvaluateMemberReference(BoundMemberReference node, InterpreterContext context)
    {
        var expression = EvaluateExpression(node.Expression, context);
        var memberReference = new ReferenceValue(
            node.NameSymbol.Type,
            () => expression.GetMember(node.NameSymbol),
            pv => expression.SetMember(node.NameSymbol, pv));
        return memberReference;
    }
}
