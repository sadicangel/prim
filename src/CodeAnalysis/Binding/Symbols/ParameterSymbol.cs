using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ParameterSymbol(SyntaxNode Syntax, Parameter Parameter)
    : MemberSymbol(BoundKind.Parameter, Syntax, Parameter.Name)
{
    public override PrimType Type { get; } = Parameter.Type;
}
