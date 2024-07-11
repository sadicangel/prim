using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class Symbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    PrimType Type,
    Symbol ContainingSymbol,
    NamespaceSymbol ContainingNamespace,
    bool IsReadOnly,
    bool IsStatic)
    : BoundNode(BoundKind, Syntax)
{
    public string FullName { get; } = GetFullName(ContainingNamespace, Name);

    private static string GetFullName(NamespaceSymbol? @namespace, string name)
    {
        // TODO: Fix this hack?
        if (@namespace is null)
            return name;

        return string.Join('.', EnumerateParts(@namespace).Append(name));

        static IEnumerable<string> EnumerateParts(NamespaceSymbol ns)
        {
            // Make it a do-while if we want to see the global namespace in the full name.
            while (ns.ContainingNamespace != ns)
            {
                yield return ns.Name;
                ns = ns.ContainingNamespace;
            }
        }
    }

    public override IEnumerable<BoundNode> Children() => [];

    public virtual bool Equals(Symbol? other) => other is not null && BoundKind == other.BoundKind && Name == other.Name;

    public override int GetHashCode() => HashCode.Combine(BoundKind, Name);

    public sealed override string ToString() => $"{FullName}: {Type}";
}
