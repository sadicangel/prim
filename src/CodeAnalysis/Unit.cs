using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

internal sealed class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
    public override string ToString() => SyntaxFacts.GetText(SyntaxKind.NullKeyword)
        ?? throw new UnreachableException("Missing ToString method");
}
