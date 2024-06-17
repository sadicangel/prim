using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ConversionSymbol(
    SyntaxNode Syntax,
    Conversion Conversion)
    : MemberSymbol(BoundKind.Conversion, Syntax, Conversion.Name)
{
    public override FunctionType Type { get; } = Conversion.Type;
}
