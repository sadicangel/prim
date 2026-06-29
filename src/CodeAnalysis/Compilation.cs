using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed class Compilation
{
    public Compilation(ImmutableArray<SourceText> sourceTexts)
        : this(sourceTexts, globalModule: null) { }

    internal Compilation(ImmutableArray<SourceText> sourceTexts, ModuleSymbol? globalModule)
    {
        SourceTexts = sourceTexts;
        GlobalModule = globalModule ?? SymbolFactory.CreateGlobalModule();
    }

    public Compilation(SourceText sourceText) : this([sourceText]) { }

    internal Compilation(SourceText sourceText, ModuleSymbol? globalModule)
        : this([sourceText], globalModule) { }

    public ImmutableArray<SourceText> SourceTexts { get; }

    internal ModuleSymbol GlobalModule { get; }

    public ImmutableArray<SyntaxTree> SyntaxTrees => !field.IsDefault ? field : field = [.. SourceTexts.Select(st => new SyntaxTree(st))];

    public IEnumerable<Diagnostic> GetDiagnostics() => SyntaxTrees.SelectMany(x => x.Diagnostics);

    internal (ImmutableArray<DeclarationNode> Declarations, ImmutableArray<Diagnostic> Diagnostics) Bind()
    {
        var binder = new Binder(new ModuleScope(GlobalModule));
        var declarations = binder.Bind(SyntaxTrees);
        return (declarations, binder.GetDiagnostics().ToImmutableArray());
    }
}