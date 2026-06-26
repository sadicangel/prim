namespace CodeAnalysis.Syntax;

public abstract record StatementSyntax(SyntaxKind Kind) : SyntaxNode(Kind);

public abstract record DeclarationSyntax(SyntaxKind Kind) : StatementSyntax(Kind)
{
    public abstract bool IsReadOnly { get; }
    public abstract bool IsStatic { get; }
}

public sealed record class GlobalDeclarationSyntax(
    NameSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax? Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax Initializer,
    SyntaxToken SemicolonToken)
    : DeclarationSyntax(SyntaxKind.GlobalDeclaration)
{
    public override bool IsReadOnly => OperatorToken.Kind is SyntaxKind.ColonToken;
    public override bool IsStatic => true;

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        if (Type is not null) yield return Type;
        yield return OperatorToken;
        yield return Initializer;
        yield return SemicolonToken;
    }
}

public sealed record class LocalDeclarationSyntax(
    SimpleNameSyntax Name,
    SyntaxToken ColonToken,
    TypeSyntax? Type,
    SyntaxToken OperatorToken,
    ExpressionSyntax? Initializer,
    SyntaxToken SemicolonToken)
    : DeclarationSyntax(SyntaxKind.LocalDeclaration)
{
    public override bool IsReadOnly => OperatorToken.Kind is SyntaxKind.ColonToken;
    public override bool IsStatic => true;

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        if (Type is not null) yield return Type;
        yield return OperatorToken;
        if (Initializer is not null) yield return Initializer;
        yield return SemicolonToken;
    }
}

public sealed record class ExpressionStatementSyntax(ExpressionSyntax Expression, SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.ExpressionStatement)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return SemicolonToken;
    }
}

public sealed record class EmptyStatementSyntax(SyntaxToken SemicolonToken)
    : StatementSyntax(SyntaxKind.EmptyStatement)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return SemicolonToken;
    }
}
