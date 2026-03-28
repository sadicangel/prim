using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class GlobalDeclarationSyntax(DeclarationSyntax Declaration)
    : ExpressionSyntax(SyntaxKind.GlobalDeclaration)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }
}
