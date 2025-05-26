using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;

internal sealed class Null
{
    public static readonly Null Value = new();
    private Null() { }
    public override string ToString() => SyntaxFacts.GetText(SyntaxKind.NullKeyword)
        ?? throw new UnreachableException("Missing ToString method");
}
