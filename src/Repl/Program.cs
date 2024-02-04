using CodeAnalysis.Syntax;
using Spectre.Console;

var syntaxTree = SyntaxTree.ParseScript("""
    sum: (a: i32, b: i32) -> i32 = a + b;
    """);

if (syntaxTree.Diagnostics.Count > 0)
{
    foreach (var diagnostic in syntaxTree.Diagnostics)
        AnsiConsole.Write(diagnostic.ToRenderable());
}
else
{
    AnsiConsole.Write(syntaxTree.ToRenderable());

    var compilation = new Compilation([syntaxTree]);

    var result = compilation.Evaluate();
    if (result.Diagnostics.Count > 0)
    {
        foreach (var diagnostic in result.Diagnostics)
            AnsiConsole.Write(diagnostic.ToRenderable());
    }
    else
    {
        AnsiConsole.WriteLine(result.Value?.ToString() ?? "null");
    }
}
