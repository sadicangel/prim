namespace CodeAnalysis.Syntax.Expressions.Declarations;
public sealed record class LocalDeclarationSyntax(SyntaxTree SyntaxTree, DeclarationSyntax Declaration)
    : ExpressionSyntax(SyntaxKind.LocalDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
    }
}
