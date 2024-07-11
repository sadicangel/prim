using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class NamespaceSymbol(
    SyntaxNode Syntax,
    string Name,
    NamespaceSymbol ContainingNamespace)
    : Symbol(
        BoundKind.NamespaceSymbol,
        Syntax,
        Name,
        PredefinedTypes.Unit,
        ContainingNamespace,
        ContainingNamespace,
        IsReadOnly: true,
        IsStatic: true)
{
    private const string GlobalNamespaceName = "<global>";

    private NamespaceSymbol() : this(
        SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
        GlobalNamespaceName,
        null!)
    {
        ContainingSymbol = this;
        ContainingNamespace = this;
    }

    public static NamespaceSymbol Global { get; } = new();
}
