using CommandLine;

namespace Compiler;
internal sealed class ProgramArguments
{
    [Value(0, Required = true, Min = 1, Max = 1, HelpText = "Source file paths.", MetaName = "<source-paths>", MetaValue = "string[]")]
    public required IReadOnlyList<string> SourcePaths { get; set; }
}
