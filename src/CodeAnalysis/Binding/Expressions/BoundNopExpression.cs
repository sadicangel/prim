using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNopExpression(
    SyntaxNode Syntax,
    TypeSymbol? Type = null)
    : BoundExpression(BoundKind.NopExpression, Syntax, Type ?? PredefinedSymbols.Unknown)
{
    public override IEnumerable<BoundNode> Children() => [];
}
