using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundWhileExpression(
    SyntaxNode Syntax,
    LabelSymbol ContinueLabel,
    BoundExpression Condition,
    BoundExpression Body,
    LabelSymbol BreakLabel)
    : BoundExpression(BoundKind.WhileExpression, Syntax, Body.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return ContinueLabel;
        yield return Condition;
        yield return Body;
        yield return BreakLabel;
    }
}
