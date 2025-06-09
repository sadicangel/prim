using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal abstract record class TypeSymbol(
    SymbolKind SymbolKind,
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : ContainerSymbol(SymbolKind, Syntax, Name, ContainingModule.RuntimeType, ContainingModule, ContainingModule, Modifiers.Static | Modifiers.ReadOnly)
{
    public bool MapsToAny => SwitchAndCheck(x => x.Name is "any");
    public bool MapsToUnit => SwitchAndCheck(x => x.Name is "unit");
    public bool MapsToNever => SwitchAndCheck(x => x.Name is "never");
    public bool MapsToUnknown => SwitchAndCheck(x => x.Name is "unknown");

    public virtual bool Equals(TypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    private bool SwitchAndCheck(Func<TypeSymbol, bool> predicate)
    {
        return this switch
        {
            ArraySymbol t => predicate(t.ElementType),
            LambdaSymbol t => predicate(t.ReturnType),
            PointerSymbol t => predicate(t.ElementType),
            StructSymbol t => predicate(t),
            UnionSymbol t => t.Types.Any(predicate),
            _ => throw new UnreachableException($"Unexpected type '{this}'"),
        };
    }
}
