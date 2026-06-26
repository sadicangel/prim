namespace CodeAnalysis.Text;

internal static class RangeExtensions
{
    extension(Range)
    {
        public static Range EmptyAt(int start) => new(start, start);
        public static Range StartAt(int start, int length) => new(start, start + length);
    }
}
