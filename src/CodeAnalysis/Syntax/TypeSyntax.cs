namespace CodeAnalysis.Syntax;

public abstract record class TypeSyntax(SyntaxKind Kind) : SyntaxNode(Kind);

public sealed record class ArrayTypeSyntax(
    TypeSyntax ElementType,
    SyntaxToken BracketOpenToken,
    ExpressionSyntax? Length,
    SyntaxToken BracketCloseToken)
    : TypeSyntax(SyntaxKind.ArrayType)
{
    public ArrayTypeSyntax(TypeSyntax elementType, SyntaxToken bracketOpenToken, SyntaxToken bracketCloseToken)
        : this(elementType, bracketOpenToken, null, bracketCloseToken) { }

    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElementType;
        yield return BracketOpenToken;
        if (Length is not null) yield return Length;
        yield return BracketCloseToken;
    }
}

public sealed record class LambdaTypeSyntax(
    SyntaxToken ParenthesisOpenToken,
    SeparatedSyntaxList<TypeSyntax> ParameterTypes,
    SyntaxToken ParenthesisCloseToken,
    SyntaxToken ArrowReturnToken,
    TypeSyntax ReturnType)
    : TypeSyntax(SyntaxKind.LambdaType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpenToken;
        foreach (var parameter in ParameterTypes.SyntaxNodes) yield return parameter;
        yield return ParenthesisCloseToken;
        yield return ArrowReturnToken;
        yield return ReturnType;
    }
}

public sealed record class MaybeTypeSyntax(TypeSyntax UnderlyingType, SyntaxToken HookToken)
    : TypeSyntax(SyntaxKind.MaybeType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return UnderlyingType;
        yield return HookToken;
    }
}

public sealed record class NamedTypeSyntax(NameSyntax Name) : TypeSyntax(SyntaxKind.NamedType)
{
    public override IEnumerable<SyntaxNode> Children() { yield return Name; }
}

public sealed record class PointerTypeSyntax(TypeSyntax ElementType, SyntaxToken StarToken)
    : TypeSyntax(SyntaxKind.PointerType)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ElementType;
        yield return StarToken;
    }
}

public sealed record class PredefinedTypeSyntax(SyntaxKind Kind, SyntaxToken PredefinedTypeToken) : TypeSyntax(Kind)
{
    public override IEnumerable<SyntaxNode> Children() { yield return PredefinedTypeToken; }
}

public sealed record class UnionTypeSyntax(SeparatedSyntaxList<TypeSyntax> Types) : TypeSyntax(SyntaxKind.UnionType)
{
    public override IEnumerable<SyntaxNode> Children() => Types.SyntaxNodes;
}

public sealed record class ErrorTypeSyntax(SyntaxToken ErrorToken) : TypeSyntax(SyntaxKind.ErrType)
{
    public override IEnumerable<SyntaxNode> Children() { yield return ErrorToken; }
}
