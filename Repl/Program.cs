using CodeAnalysis;
using CodeAnalysis.Binding;
using CodeAnalysis.Syntax;

var showTree = false;

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (String.IsNullOrWhiteSpace(line))
        return;

    if (line.StartsWith('\\'))
    {
        switch (line)
        {
            case "\\showtree":
                showTree = true;
                Console.WriteLine($"<{(showTree ? "showing" : "hiding")} parse trees>");
                continue;

            case "\\cls":
                Console.Clear();
                continue;
        }
    }

    var syntaxTree = SyntaxTree.Parse(line);
    var binder = new Binder();
    var boundExpression = binder.BindExpression(syntaxTree.Root);

    var diagnosists = syntaxTree.Diagnostics.Concat(binder.Diagnostics);

    if (showTree)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        syntaxTree.PrettyPrint(Console.Out);
    }

    Console.ResetColor();

    if (diagnosists.Any())
    {
        foreach (var diagnostic in diagnosists)
        {
            Console.ForegroundColor = diagnostic.IsError ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;
            Console.WriteLine(diagnostic.Message);
        }

        Console.ResetColor();
    }
    else
    {
        var evaluator = new Evaluator(boundExpression);
        Console.WriteLine(evaluator.Evaluate());
    }
}