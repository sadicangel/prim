using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ArrayTypeSymbol : TypeSymbol
{
    public ArrayTypeSymbol(SyntaxNode syntax, TypeSymbol elementType, int length)
        : base(
            BoundKind.ArrayTypeSymbol,
            syntax,
            $"[{elementType.Name}: {length}]",
            PredefinedTypes.Type)
    {
        ElementType = elementType;
        Length = length;
        AddOperator(
            SyntaxKind.BracketOpenBracketCloseToken,
            new LambdaTypeSymbol([new("index", PredefinedTypes.I32)], ElementType),
            isReadOnly: false);
    }

    public TypeSymbol ElementType { get; init; }
    public int Length { get; init; }

    public override bool IsNever => ElementType.IsNever;
}
