using System.Collections.Immutable;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;

public sealed class Compilation(ImmutableArray<SourceText> sourceTexts)
{
    public ImmutableArray<SourceText> SourceTexts { get; } = sourceTexts;

    public ImmutableArray<SyntaxTree> SyntaxTrees => !field.IsDefault ? field : field = [.. SourceTexts.Select(st => new SyntaxTree(st))];

    public Compilation(SourceText sourceText) : this([sourceText]) { }

    public IEnumerable<Diagnostic> GetDiagnostics() => SyntaxTrees.SelectMany(x => x.Diagnostics);
}
