﻿namespace CodeAnalysis.Binding.Types.Metadata;

internal sealed record class Property(
    string Name,
    PrimType Type,
    bool IsReadOnly
)
    : Member(Name)
{
    public override string ToString() => $"{Name}: {Type.Name}";
}
