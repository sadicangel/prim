using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundLiteralExpression(SyntaxNode Syntax, TypeSymbol Type, object Value)
    : BoundExpression(BoundKind.LiteralExpression, Syntax, Type)
{
    public static BoundLiteralExpression Unit(TypeSymbol unitType) =>
        new(SyntaxFactory.SyntheticToken(SyntaxKind.NullKeyword), unitType, CodeAnalysis.Unit.Value);

    public override IEnumerable<BoundNode> Children() => [];
}
