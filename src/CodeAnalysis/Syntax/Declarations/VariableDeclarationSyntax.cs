using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;
public sealed record class VariableDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken LetOrVarKeyword,
    SimpleNameSyntax Name,
    TypeClauseSyntax? TypeClause,
    InitClauseSyntax? InitClause,
    SyntaxToken? SemicolonToken)
    : DeclarationSyntax(SyntaxKind.VariableDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return LetOrVarKeyword;
        yield return Name;
        if (TypeClause is not null)
            yield return TypeClause;
        if (InitClause is not null)
            yield return InitClause;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
