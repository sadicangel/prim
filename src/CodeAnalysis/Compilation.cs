using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Spectre.Console;

namespace CodeAnalysis;
public readonly record struct Result(object Value, DiagnosticBag Diagnostics)
{
    public bool HasDiagnostics => Diagnostics.Count > 0;
    public Result(object value) : this(value, []) { }
    public Result(DiagnosticBag diagnostics) : this(Unit.Value, diagnostics) { }
}

public sealed record class Compilation(IReadOnlyList<SyntaxTree> SyntaxTrees, Compilation? Previous = null)
{
    public Result Evaluate()
    {
        if (SyntaxTrees.Any(st => st.Diagnostics.HasErrors))
            return new Result(new DiagnosticBag(SyntaxTrees.SelectMany(tree => tree.Diagnostics)));

        var program = Binder.Bind(Scope.CreateGlobalScope(), SyntaxTrees[0].Root);
        if (program.Diagnostics.HasErrors)
            return new Result(program.Diagnostics);

        AnsiConsole.Write(program.ToRenderable());
        AnsiConsole.WriteLine();

        return Interpreter.Interpret(Environment.CreateGlobal(), program);
    }
}
