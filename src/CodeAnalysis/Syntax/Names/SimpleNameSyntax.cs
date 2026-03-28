namespace CodeAnalysis.Syntax.Names;

public record class SimpleNameSyntax(SyntaxToken IdentifierToken)
    : NameSyntax(SyntaxKind.SimpleName)
{
    public override string FullName => field ??= IdentifierToken.SourceSpan.ToString();

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}

internal record class SyntheticSimpleNameSyntax(string FullName)
    : SimpleNameSyntax(SyntaxToken.CreateSynthetic(SyntaxKind.IdentifierToken))
{
    public override string FullName { get; } = FullName;

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
