﻿namespace CodeAnalysis.Text;

public sealed record class SourceLine(SourceText Source, Range Range, Range RangeWithLineBreak)
{
    public char this[Index index] => Text[index];
    public ReadOnlySpan<char> this[Range range] => Text[Range];

    public ReadOnlySpan<char> Text { get => Source[Range]; }
    public ReadOnlySpan<char> TextWithLineBreak { get => Source[RangeWithLineBreak]; }

    public override string ToString() => Text.ToString();
}