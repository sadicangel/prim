namespace CodeAnalysis.Syntax;

public readonly record struct TokenTrivia(List<Trivia> Leading, List<Trivia> Trailing)
{
    public static readonly TokenTrivia Empty = new([], []);
}
