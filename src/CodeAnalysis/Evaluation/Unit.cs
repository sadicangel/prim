namespace CodeAnalysis.Evaluation;

internal sealed class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }

    public override string ToString() => "null";
}