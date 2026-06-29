namespace CodeAnalysis.Syntax;

public abstract record class NameSyntax(SyntaxKind Kind) : ExpressionSyntax(Kind)
{
    public abstract NameString Name { get; }

    public string FullName => Name.FullName;
}

public sealed record class SimpleNameSyntax(SyntaxToken IdentifierToken) : NameSyntax(SyntaxKind.SimpleName)
{
    public override NameString Name => field.IsEmpty ? field = new NameString(IdentifierToken.ValueText.ToString()) : field;

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}

public sealed record class QualifiedNameSyntax(SeparatedSyntaxList<SyntaxToken> IdentifierTokens) : NameSyntax(SyntaxKind.QualifiedName)
{
    public override NameString Name => field.IsEmpty ? field = new NameString(IdentifierTokens.Select(x => x.ValueText.ToString())) : field;

    public override IEnumerable<SyntaxNode> Children() => IdentifierTokens.SyntaxNodes;
}
