using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ConversionSymbol(SyntaxNode Syntax, Conversion Conversion)
    : MemberSymbol(BoundKind.ConversionSymbol, Syntax, Conversion.Name, IsReadOnly: true)
{
    public override FunctionType Type { get; } = Conversion.Type;
}
