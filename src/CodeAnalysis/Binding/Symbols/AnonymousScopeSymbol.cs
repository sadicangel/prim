using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class AnonymousScopeSymbol(ScopeSymbol ContainingScope)
    : ScopeSymbol(
        BoundKind.AnonymousScopeSymbol,
        SyntaxFactory.SyntheticToken(SyntaxKind.IdentifierToken),
        $"<anonymous-{Guid.NewGuid()}>",
        ContainingScope.Never,
        ContainingScope,
        IsStatic: true,
        IsReadOnly: true)
{
    public bool IsGlobal => Name == GlobalModule.Name;

    public override bool IsAnonymous => true;

    public override ModuleSymbol Module => ContainingModule;
}
