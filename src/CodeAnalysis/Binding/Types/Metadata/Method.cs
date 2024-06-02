namespace CodeAnalysis.Binding.Types.Metadata;
internal sealed record class Method(string Name, FunctionType Type) : Member(Name);
