namespace CodeAnalysis.Evaluation;

internal sealed class Never
{
    public static readonly Never Value = new();

    private Never() { }

    public override string ToString() => "{}";
}