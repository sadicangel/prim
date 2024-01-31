using CodeAnalysis.Syntax;
using Spectre.Console;

var syntaxTree = SyntaxTree.ParseScript("""
    for (a : get_range()) {
        print(a + 2);
        print("done!");
    }
    """);



AnsiConsole.Write(syntaxTree.ToRenderable());