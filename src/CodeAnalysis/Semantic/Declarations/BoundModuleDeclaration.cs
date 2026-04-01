using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic.Declarations;

internal sealed record class BoundModuleDeclaration(ModuleSymbol Module, ImmutableArray<BoundNode> Members)
    : BoundDeclaration(BoundKind.ModuleDeclaration, Module.Syntax, Module.Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => Members;
}
