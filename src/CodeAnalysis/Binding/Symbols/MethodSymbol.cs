using CodeAnalysis.Syntax;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class MethodSymbol(
    SyntaxNode Syntax,
    Method Method)
    : MemberSymbol(BoundKind.Method, Syntax, $"{Method.Name}<{Method.Type.Name}>")
{
    public override FunctionType Type { get; } = Method.Type;
}
