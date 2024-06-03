﻿namespace CodeAnalysis.Binding.Types.Metadata;
internal sealed record class Method(string Name, FunctionType Type) : Member(Name)
{
    public override FunctionType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
