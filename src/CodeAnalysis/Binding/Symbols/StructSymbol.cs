using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(
    SyntaxNode Syntax,
    StructType StructType,
    Symbol? ContainingSymbol)
    : Symbol(
        BoundKind.StructSymbol,
        Syntax,
        StructType.Name,
        StructType,
        ContainingSymbol,
        IsReadOnly: true,
        IsStatic: true);
