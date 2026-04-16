using System.Collections.Immutable;
using CodeAnalysis.Binding;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed class Compilation(ImmutableArray<SourceText> sourceTexts, ParseOptions parseOptions = default, Compilation? previous = null)
{
    private readonly Dictionary<Symbol, Result<BoundNode>> _bindings = [];

    public ImmutableArray<SourceText> SourceTexts { get; } = sourceTexts;

    public ParseOptions ParseOptions { get; } = parseOptions;

    public Compilation? Previous { get; } = previous;

    public ImmutableArray<SyntaxTree> SyntaxTrees => !field.IsDefault ? field : field = [.. SourceTexts.Select(st => new SyntaxTree(st, ParseOptions))];

    internal ModuleSymbol GlobalModule => field ??= CreateDeclaredGlobalModule();

    internal VariableSymbol? EntryPoint => GlobalModule.FindEntryPoint();

    public Compilation(SourceText sourceText, ParseOptions parseOptions = default, Compilation? previous = null)
        : this([sourceText], parseOptions, previous) { }

    public IEnumerable<Diagnostic> GetDiagnostics() => SyntaxTrees.SelectMany(x => x.Diagnostics);

    internal ModuleSymbol CreateDeclaredGlobalModule()
    {
        var global = Previous?.GlobalModule ?? SymbolFactory.CreateGlobalModule();
        var binder = new GlobalSymbolBinder(global);
        binder.DeclareSymbols(SyntaxTrees);
        return global;
    }

    internal void EnsureGlobalSymbolsDeclared() => _ = GlobalModule;

    internal Result<BoundNode> Bind(Symbol symbol)
    {
        var current = this;
        EnsureGlobalSymbolsDeclared();
        //do
        //{
        if (current._bindings.TryGetValue(symbol, out var binding))
        {
            return new Result<BoundNode>(binding.Value.LinkParents(), binding.Diagnostics);
        }

        //    current = current.Previous;
        //} while (current is not null);

        var result = symbol.BindDeclared();
        var boundNode = result.Value.LinkParents();
        return _bindings[symbol] = result with { Value = boundNode };
    }
}
