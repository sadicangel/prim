namespace CodeAnalysis.Text;

public readonly record struct SourceLine(SourceSpan SourceSpan, SourceSpan SourceSpanWithLineBreak)
{
    public SourceText SourceText => SourceSpan.SourceText;

    public char this[Index index] => SourceSpan[index];
    public ReadOnlySpan<char> this[Range range] => SourceSpan[range];

    public override string ToString() => SourceSpan.ToString();
}
