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

    private bool SwitchAndCheck(Func<TypeSymbol, bool> predicate)
    {
        return this switch
        {
            ArrayTypeSymbol t => predicate(t.ElementType),
            LambdaTypeSymbol t => predicate(t.ReturnType), // TODO: This is wrong!
            PointerTypeSymbol t => predicate(t.ElementType),
            StructTypeSymbol t => predicate(t),
            UnionTypeSymbol t => t.Types.Any(predicate),
            _ => throw new UnreachableException($"Unexpected type '{this}'"),
        };
    }
}
