using CodeAnalysis;
using CodeAnalysis.Syntax;
using Spectre.Console;
using System.Text;

Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

var syntaxTree = SyntaxTree.ParseScript("""
    sum: (a: i32, b: i32) -> i32 = a + b;
    c: i32 = sum(2, 5);
    """);

if (syntaxTree.Diagnostics.Count > 0)
{
    foreach (var diagnostic in syntaxTree.Diagnostics)
        AnsiConsole.Write(diagnostic.ToRenderable());
}
else
{
    AnsiConsole.Write(new Markup(syntaxTree.Source.Text, "grey66 i"));
    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine();

    AnsiConsole.Write(syntaxTree.ToRenderable());
    AnsiConsole.WriteLine();

    var compilation = new Compilation([syntaxTree]);

    var result = compilation.Evaluate();
    if (result.Diagnostics.Count > 0)
    {
        foreach (var diagnostic in result.Diagnostics)
            AnsiConsole.Write(diagnostic.ToRenderable());
    }
    else
    {
        AnsiConsole.Write(new Markup(result.Value.ToString() ?? string.Empty, "plum4 i"));
        AnsiConsole.WriteLine();
    }
}
