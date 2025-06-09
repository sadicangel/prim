using System.Diagnostics;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal static class Mangler
{
    public static string Mangle(SyntaxKind syntaxKind, params ICollection<TypeSymbol> types)
    {
        var name = SyntaxFacts.GetText(syntaxKind) ?? throw new UnreachableException($"Syntax kind '{syntaxKind}' has no text");

        if (types.Count == 0)
            return name;

        return $"{name}<{string.Join(',', types.Select(x => x.Name))}>";
    }
}
