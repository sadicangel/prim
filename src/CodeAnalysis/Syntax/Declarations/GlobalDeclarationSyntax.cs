using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class GlobalDeclarationSyntax(SyntaxTree SyntaxTree, DeclarationSyntax Declaration)
    : ExpressionSyntax(SyntaxKind.GlobalDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }
}
