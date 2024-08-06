namespace CodeAnalysis.Syntax.Expressions.Names;

public record class QualifiedNameSyntax(
    SyntaxTree SyntaxTree,
    NameSyntax Left,
    SyntaxToken ColonColonToken,
    SimpleNameSyntax Right)
    : NameSyntax(SyntaxKind.QualifiedName, SyntaxTree)
{
    private NameValue? _nameValues;

    public override NameValue NameValue { get => _nameValues ??= new NameValue([.. EnumerateNames(this)]); }

    private static IEnumerable<string> EnumerateNames(NameSyntax name)
    {
        if (name is SimpleNameSyntax simpleName)
        {
            yield return simpleName.NameValue.ToString();
        }
        else
        {
            var qualifiedName = (QualifiedNameSyntax)name;
            foreach (var left in EnumerateNames(qualifiedName.Left))
                yield return left;

            yield return qualifiedName.Right.NameValue.ToString();
        }
    }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return ColonColonToken;
        yield return Right;
    }
}
