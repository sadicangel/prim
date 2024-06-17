using CodeAnalysis.Binding;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis;
public sealed record class Compilation(ReadOnlyList<SyntaxTree> SyntaxTrees, DiagnosticBag Diagnostics, Compilation? Previous = null)
{
    private readonly BoundScope _scope = Previous?._scope ?? new GlobalBoundScope();

    internal ReadOnlyList<BoundTree> Compile()
    {
        var boundTrees = new List<BoundTree>();
        var diagnostics = new DiagnosticBag();
        foreach (var syntaxTree in SyntaxTrees)
        {
            if (syntaxTree.Diagnostics.Count > 0)
            {
                diagnostics.AddRange(syntaxTree.Diagnostics);
                continue;
            }

            var boundTree = BoundTree.Bind(syntaxTree, _scope);
            diagnostics.AddRange(boundTree.Diagnostics);
            boundTrees.Add(boundTree);
        }
        return [.. boundTrees];
    }
}
