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

    internal ModuleSymbol GlobalModule => field ??= GetGlobalModule();

    internal VariableSymbol? EntryPoint => GlobalModule.FindEntryPoint();

    public Compilation(SourceText sourceText, ParseOptions parseOptions = default, Compilation? previous = null)
        : this([sourceText], parseOptions, previous) { }

    public IEnumerable<Diagnostic> GetDiagnostics() => SyntaxTrees.SelectMany(x => x.Diagnostics);

    private ModuleSymbol GetGlobalModule()
    {
        var global = Previous?.GlobalModule ?? SymbolFactory.CreateGlobalModule();
        var binder = new ModuleBinder(global);
        binder.DeclareSymbols(SyntaxTrees);
        return global;
    }

    internal Result<BoundNode> Bind(Symbol symbol)
    {
        var current = this;
        //do
        //{
        if (current._bindings.TryGetValue(symbol, out var binding))
        {
            return binding;
        }

        //    current = current.Previous;
        //} while (current is not null);

        return _bindings[symbol] = symbol.Bind();
    }
}
