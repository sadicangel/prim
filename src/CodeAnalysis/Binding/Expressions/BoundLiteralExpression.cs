using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(SyntaxNode Syntax, TypeSymbol Type, object Value)
    : BoundExpression(BoundKind.LiteralExpression, Syntax, Type)
{
    public static BoundLiteralExpression Unit =>
        new(SyntaxFactory.SyntheticToken(SyntaxKind.NullKeyword), PredefinedSymbols.Unit, CodeAnalysis.Unit.Value);

    public override IEnumerable<BoundNode> Children() => [];
}
