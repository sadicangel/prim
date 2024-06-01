namespace CodeAnalysis.Binding.Types.Metadata;
public abstract record class Method(string Name, PrimType Type) : Member(Name);
