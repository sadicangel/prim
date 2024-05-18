namespace CodeAnalysis.Text;

public readonly record struct SourceLocation(SourceText SourceText, Range Range)
{
    public char this[Index index] => Text[index];
    public ReadOnlySpan<char> this[Range range] => Text[Range];

    public ReadOnlySpan<char> Text { get => SourceText[Range]; }

    public override string ToString() => Text.ToString();

    public string FileName { get => SourceText.FileName; }
    public string FilePath { get => SourceText.FilePath; }
    public int StartLine => SourceText.GetLineIndex(Range.Start);
    public int StartCharacter => Range.Start.Value - SourceText.Lines[StartLine].Range.Start.Value;
    public int EndLine => SourceText.GetLineIndex(Range.End);
    public int EndCharacter => Range.End.Value - SourceText.Lines[StartLine].Range.Start.Value;
}