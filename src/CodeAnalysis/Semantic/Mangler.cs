using System.Diagnostics;
using System.Text;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal static class Mangler
{
    public static string Mangle(SyntaxKind syntaxKind, params ReadOnlySpan<TypeSymbol> types)
    {
        var name = SyntaxFacts.GetText(syntaxKind) ?? throw new UnreachableException($"Syntax kind '{syntaxKind}' has no text");

        if (types.IsEmpty)
            return name;

        var builder = new StringBuilder(name);
        var first = true;

        builder.Append('<');
        foreach (var type in types)
        {
            if (first) first = false;
            else builder.Append(',');
            builder.Append(type.Name);
        }

        builder.Append('>');
        return builder.ToString();
    }
}
