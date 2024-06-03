using CodeAnalysis.Binding.Types;
using CodeAnalysis.Binding.Types.Metadata;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class MethodSymbol(SyntaxNode SyntaxNode, Method Method)
    : Symbol(BoundKind.Method, SyntaxNode, $"{Method.Name}<{Method.Type.Name}>")
{
    public override FunctionType Type { get; } = Method.Type;
}
