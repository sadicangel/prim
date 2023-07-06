using CodeAnalysis;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using CommandLine;
using Compiler;

Parser.Default
    .ParseArguments<ProgramArguments>(args)
    .WithParsed(HandleArguments);

static void HandleArguments(ProgramArguments args)
{
    var sourceCode = File.ReadAllText(args.SourcePaths[0]);
    var syntaxTree = SyntaxTree.Parse(sourceCode.AsMemory());
    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(new Dictionary<Symbol, object?>());

    if (result.Diagnostics.Count == 0)
    {
        if (result.Value is not null)
            Console.WriteLine(result.Value);
    }
    else
    {
        result.Diagnostics.WriteTo(Console.Out, syntaxTree);
    }
}
