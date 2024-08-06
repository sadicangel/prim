using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class AnonymousScopeSymbol(
    ModuleSymbol ContainingModule,
    ScopeSymbol ContainingScope)
    : ScopeSymbol(
        BoundKind.AnonymousScopeSymbol,
        SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
        GetName(ContainingScope),
        ContainingModule.Never,
        ContainingModule,
        ContainingScope,
        IsStatic: true,
        IsReadOnly: true)
{
    public bool IsGlobal => Name == GlobalModule.Name;

    public override bool IsAnonymous => true;

    public override ModuleSymbol Module => ContainingModule;

    private static string GetName(ScopeSymbol scopeSymbol) =>
        $"{scopeSymbol.Name}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}<AnonymousScope>{Guid.NewGuid()}";
}
