using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Tests;
internal static class Extensions
{
    public static PrimValue Evaluate(this string sourceText)
    {
        var compilation = Compilation.CompileScript(new SourceText(sourceText));
        Assert.Empty(compilation.Diagnostics);
        var evaluation = Evaluation.Evaluate(compilation);
        return evaluation.Values[0];
    }
}
