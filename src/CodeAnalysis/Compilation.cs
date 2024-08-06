using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis;
public sealed class Compilation
{
    private Compilation(IEnumerable<SourceText> sourceTexts, bool isScript, Compilation? previous = null)
    {
        Func<SourceText, SyntaxTree> parseFunc = isScript ? SyntaxTree.ParseScript : SyntaxTree.Parse;
        SyntaxTrees = new(sourceTexts.Select(parseFunc));
        Previous = previous;
        BoundScope = Previous?.BoundScope ?? ModuleSymbol.CreateGlobalModule();
        BoundTrees = new ReadOnlyList<BoundTree>(SyntaxTrees
            .Select(tree => BoundTree.Bind(tree, BoundScope)));
        Diagnostics = new DiagnosticBag(SyntaxTrees
            .SelectMany(tree => tree.Diagnostics)
            .Concat(BoundTrees
                .SelectMany(tree => tree.Diagnostics)));
    }

    public ReadOnlyList<SyntaxTree> SyntaxTrees { get; }
    public Compilation? Previous { get; }
    internal ScopeSymbol BoundScope { get; }
    internal ReadOnlyList<BoundTree> BoundTrees { get; }
    public DiagnosticBag Diagnostics { get; }

    public static Compilation Compile(SourceText sourceText, Compilation? previous = null) =>
        Compile([sourceText], previous);
    public static Compilation Compile(IEnumerable<SourceText> sourceTexts, Compilation? previous = null) =>
        new(sourceTexts, isScript: false, previous);

    public static Compilation CompileScript(SourceText sourceText, Compilation? previous = null) =>
        CompileScript([sourceText], previous);
    public static Compilation CompileScript(IEnumerable<SourceText> sourceTexts, Compilation? previous = null) =>
        new(sourceTexts, isScript: true, previous);
}
