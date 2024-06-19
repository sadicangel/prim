using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class MethodSymbol(SyntaxNode Syntax, Method Method, StructSymbol? ContainingSymbol = null)
    : MemberSymbol(BoundKind.MethodSymbol, Syntax, Method.Name, ContainingSymbol)
{
    public override FunctionType Type { get; } = Method.Type;
}
