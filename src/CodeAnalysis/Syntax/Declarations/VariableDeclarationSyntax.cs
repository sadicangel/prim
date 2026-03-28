using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class VariableDeclarationSyntax(
    SyntaxToken BindingKeyword,
    SimpleNameSyntax Name,
    TypeClauseSyntax? TypeClause,
    InitClauseSyntax? InitClause,
    SyntaxToken? SemicolonToken)
    : DeclarationSyntax(SyntaxKind.VariableDeclaration)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BindingKeyword;
        yield return Name;
        if (TypeClause is not null)
            yield return TypeClause;
        if (InitClause is not null)
            yield return InitClause;
        if (SemicolonToken is not null)
            yield return SemicolonToken;
    }
}
