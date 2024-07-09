using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(SyntaxNode Syntax, StructType StructType)
    : Symbol(BoundKind.StructSymbol, Syntax, StructType.Name, StructType, IsReadOnly: true, IsStatic: true);
