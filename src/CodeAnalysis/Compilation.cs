using System.Collections.Immutable;
using CodeAnalysis.Binding;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed class Compilation(ImmutableArray<SourceText> sourceTexts, ParseOptions parseOptions = default, Compilation? previous = null)
{
    public ImmutableArray<SourceText> SourceTexts { get; } = sourceTexts;

    public ParseOptions ParseOptions { get; } = parseOptions;

    public Compilation? Previous { get; } = previous;

    public ImmutableArray<SyntaxTree> SyntaxTrees => !field.IsDefault ? field : field = [.. SourceTexts.Select(st => new SyntaxTree(st, ParseOptions))];

    internal BoundProgram Program => field ??= Binder.Bind(SyntaxTrees);

    public Compilation(SourceText sourceText, ParseOptions parseOptions = default, Compilation? previous = null)
        : this([sourceText], parseOptions, previous)
    {
    }
}
