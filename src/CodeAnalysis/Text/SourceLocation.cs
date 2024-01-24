namespace CodeAnalysis.Text;

public readonly record struct SourceLocation(Source Source, Range Range)
{
    public char this[Index index] => Text[index];
    public ReadOnlySpan<char> this[Range range] => Text[Range];

    public ReadOnlySpan<char> Text { get => Source[Range]; }

    public override string ToString() => Text.ToString();

    public string FileName { get => Source.FileName; }
    public string FilePath { get => Source.FilePath; }
    public int StartLine => Source.GetLineIndex(Range.Start);
    public int StartCharacter => Range.Start.Value - Source.Lines[StartLine].Range.Start.Value;
    public int EndLine => Source.GetLineIndex(Range.End);
    public int EndCharacter => Range.End.Value - Source.Lines[StartLine].Range.Start.Value;
}
