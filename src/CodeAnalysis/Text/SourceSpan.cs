using System.Diagnostics;

namespace CodeAnalysis.Text;

public readonly record struct SourceSpan(SourceText SourceText, Range Range)
{
    public char this[Index index] => SourceText[Range][index];
    public char this[int index] => SourceText[Range][index];
    public ReadOnlySpan<char> this[Range range] => SourceText[Range][range];

    public int Length => Range.End.Value - Range.Start.Value;

    public string FileName { get => SourceText.FileName; }
    public string FilePath { get => SourceText.FilePath; }

    public int StartLine => SourceText.GetLineIndex(Range.Start);
    public int StartCharacter => Range.Start.Value - SourceText.Lines[StartLine].SourceSpan.Range.Start.Value;

    public int EndLine => SourceText.GetLineIndex(Range.End);
    public int EndCharacter => Range.End.Value - SourceText.Lines[StartLine].SourceSpan.Range.Start.Value;

    public override string ToString() => SourceText[Range].ToString();

    public static SourceSpan Union(SourceSpan left, SourceSpan right)
    {
        Debug.Assert(left.SourceText == right.SourceText, "Cannot combine spans from different source texts.");

        var min = Math.Min(left.Range.Start.Value, right.Range.Start.Value);
        var max = Math.Max(left.Range.End.Value, right.Range.End.Value);

        return new SourceSpan(left.SourceText, min..max);
    }
}
