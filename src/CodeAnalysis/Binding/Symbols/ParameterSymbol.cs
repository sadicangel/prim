using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ParameterSymbol(SyntaxNode Syntax, Parameter Parameter)
    : MemberSymbol(BoundKind.Parameter, Syntax, Parameter.Name)
{
    public override PrimType Type { get; } = Parameter.Type;
}
