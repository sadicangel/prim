using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ConversionSymbol(SyntaxNode Syntax, Conversion Conversion, StructSymbol? ContainingSymbol = null)
    : MemberSymbol(BoundKind.ConversionSymbol, Syntax, Conversion.Name, IsReadOnly: true, ContainingSymbol)
{
    public override FunctionType Type { get; } = Conversion.Type;
}
