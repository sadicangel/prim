namespace CodeAnalysis.Semantic.Symbols;

[Flags]
internal enum Modifiers
{
    None = 0,
    Static = 1 << 0,
    ReadOnly = 1 << 1,
}

