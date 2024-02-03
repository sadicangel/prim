using CodeAnalysis.Syntax;
using Spectre.Console;

var syntaxTree = SyntaxTree.ParseScript("""
    a: i32 = 2;
    b: i32 = a + 2;
    """);
AnsiConsole.Write(syntaxTree.ToRenderable());

var compilation = new Compilation([syntaxTree]);

compilation.Evaluate();
