using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ModuleDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameExpressionSyntax Name,
    SyntaxToken ColonToken,
    SyntaxToken ModuleKeyword,
    SyntaxToken ColonOrEquals,
    SyntaxToken BraceOpenToken,
    SyntaxList<DeclarationSyntax> Declarations,
    SyntaxToken BraceCloseToken
    )
    : DeclarationSyntax(SyntaxKind.ModuleDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return ModuleKeyword;
        yield return ColonOrEquals;
        yield return BraceOpenToken;
        foreach (var declaration in Declarations)
            yield return declaration;
        yield return BraceCloseToken;
    }
}
