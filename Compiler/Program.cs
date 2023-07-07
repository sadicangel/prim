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
    var (pathsFound, pathsNotFound) = FlattenPaths(args.SourcePaths);

    if (pathsNotFound.Count > 0)
    {
        foreach (var file in pathsNotFound)
            Console.WriteLine($"Could not find file '{file}'");
        return;
    }

    var syntaxTrees = pathsFound
        .Select(SyntaxTree.Load)
        .ToArray();

    var compilation = new Compilation(syntaxTrees);
    var result = compilation.Evaluate(new Dictionary<Symbol, object?>());

    if (!result.Diagnostics.Any())
    {
        if (result.Value is not null)
            Console.WriteLine(result.Value);
    }
    else
    {
        result.Diagnostics.WriteTo(Console.Out);
    }

    static (SortedSet<string> PathsFound, SortedSet<string> PathsNotFound) FlattenPaths(IEnumerable<string> paths)
    {
        var pathsFound = new SortedSet<string>();
        var pathsNotFound = new SortedSet<string>();
        foreach (var path in paths.Select(Path.GetFullPath))
        {
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.EnumerateFiles(path, "*.pr", new EnumerationOptions { RecurseSubdirectories = true }))
                    SortFile(file);
            }
            else
            {
                SortFile(path);
            }
        }
        return (pathsFound, pathsNotFound);



        void SortFile(string path)
        {
            if (File.Exists(path))
                pathsFound.Add(path);
            else
                pathsNotFound.Add(path);
        }
    }
}
