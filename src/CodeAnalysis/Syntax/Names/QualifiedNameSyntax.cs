namespace CodeAnalysis.Syntax.Names;

public record class QualifiedNameSyntax(NameSyntax Left, SyntaxToken ColonColonToken, SimpleNameSyntax Right)
    : NameSyntax(SyntaxKind.QualifiedName)
{
    public override string FullName => field ??= string.Join(SyntaxFacts.GetText(SyntaxKind.ColonColonToken), EnumerateNames(this));

    private static IEnumerable<string> EnumerateNames(NameSyntax name)
    {
        if (name is SimpleNameSyntax simpleName)
        {
            yield return simpleName.FullName;
        }
        else
        {
            var qualifiedName = (QualifiedNameSyntax)name;
            foreach (var left in EnumerateNames(qualifiedName.Left))
                yield return left;

            yield return qualifiedName.Right.FullName;
        }
    }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return ColonColonToken;
        yield return Right;
    }
}
