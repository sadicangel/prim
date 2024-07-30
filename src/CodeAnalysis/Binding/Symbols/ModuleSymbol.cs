using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ModuleSymbol(
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : Symbol(
        BoundKind.ModuleSymbol,
        Syntax,
        Name,
        Predefined.Never,
        ContainingModule,
        IsStatic: true,
        IsReadOnly: true);
