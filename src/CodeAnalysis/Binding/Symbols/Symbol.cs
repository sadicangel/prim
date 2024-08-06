using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal abstract record class Symbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule,
    bool IsStatic,
    bool IsReadOnly)
    : BoundNode(BoundKind, Syntax)
{
    public QualifiedName QualifiedName { get; } = new QualifiedName(ContainingModule, Name);

    public abstract IEnumerable<Symbol> DeclaredSymbols { get; }

    public sealed override IEnumerable<Symbol> Children() => [];

    public virtual bool Equals(Symbol? other) => other is not null && BoundKind == other.BoundKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(BoundKind, Name);

    public sealed override string ToString() => $"{Name}: {Type.Name}";

    private static NameValue GetQualifiedName(Symbol symbol)
    {
        return EnumerateNames(symbol).ToArray();

        static IEnumerable<string> EnumerateNames(Symbol symbol)
        {
            var module = symbol.ContainingModule;

            while (!module.IsGlobal)
            {
                yield return module.Name;
                module = module.ContainingModule;
            }

            yield return symbol.Name;
        }
    }
}
