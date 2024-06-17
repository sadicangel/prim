using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ConversionSymbol(
    SyntaxNode Syntax,
    Conversion Conversion)
    : MemberSymbol(BoundKind.Conversion, Syntax, Conversion.Name)
{
    public override FunctionType Type { get; } = Conversion.Type;
}
