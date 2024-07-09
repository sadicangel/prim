using CodeAnalysis.Syntax;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(SyntaxNode Syntax, Property Property, bool IsReadOnly, bool IsStatic)
    : Symbol(BoundKind.PropertySymbol, Syntax, Property.Name, Property.Type, IsReadOnly, IsStatic);
