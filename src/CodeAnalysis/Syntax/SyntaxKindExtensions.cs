using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Syntax;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
internal static class SyntaxKindExtensions
{
    extension(SyntaxKind kind)
    {
        public bool IsEndingKind(params ReadOnlySpan<SyntaxKind> kinds) => kind is SyntaxKind.EofToken || kinds.Contains(kind);
    }
}
