namespace CodeAnalysis.Tests.Parsing;

#if !DEBUG
public partial class ScannerTests
{
    public static TheoryData<TokenData, TokenData> GetTokenPairs()
    {
        var data = new TheoryData<TokenData, TokenData>();
        foreach (var x in s_all_tokens)
            foreach (var y in s_all_tokens)
                if (!RequireSeparator(x.SyntaxKind, y.SyntaxKind))
                    data.Add(x, y);
        return data;
    }

    [Theory]
    [MemberData(nameof(GetTokenPairs))]
    public void Scan_scans_all_pairs(TokenData x, TokenData y)
    {
        if (SyntaxTree.Scan(new SourceText(x.Text + y.Text)) is [var a, var b, _])
        {
            Assert.Equal(x.SyntaxKind, a.SyntaxKind);
            Assert.Equal(x.Text, a.Text);
            Assert.Equal(y.SyntaxKind, b.SyntaxKind);
            Assert.Equal(y.Text, b.Text);
        }
        else
        {
            Assert.Fail($"{nameof(Scan_scans_all_pairs)} must scan 3 tokens");
        }
    }

    private static bool RequireSeparator(SyntaxKind x, SyntaxKind y)
    {
        if ((x is SyntaxKind.IdentifierToken || SyntaxFacts.IsKeyword(x) || SyntaxFacts.IsNumberLiteralToken(x))
            && (y is SyntaxKind.IdentifierToken || SyntaxFacts.IsKeyword(y) || SyntaxFacts.IsNumberLiteralToken(y)))
            return true;

        if (SyntaxFacts.IsNumberLiteralToken(x))
        {
            if (y is SyntaxKind.DotToken or SyntaxKind.DotDotToken)
                return true;
        }

        switch ((x, y))
        {
            case (SyntaxKind.AmpersandToken, SyntaxKind.AmpersandToken):
            case (SyntaxKind.AmpersandToken, SyntaxKind.AmpersandAmpersandToken):
            case (SyntaxKind.AmpersandToken, SyntaxKind.AmpersandEqualsToken):
            case (SyntaxKind.AmpersandToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.AmpersandToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.AmpersandToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.ArrowToken, SyntaxKind.ArrowToken):
                return true;

            case (SyntaxKind.BangToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.BangToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.BangToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.DotToken, SyntaxKind.DotToken):
            case (SyntaxKind.DotToken, SyntaxKind.DotDotToken):
            case (SyntaxKind.DotToken, var kind) when SyntaxFacts.IsNumberLiteralToken(kind):
            case (SyntaxKind.DotDotToken, SyntaxKind.DotToken):
                return true;

            case (SyntaxKind.EqualsToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.EqualsToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.EqualsToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.EqualsToken, SyntaxKind.GreaterToken):
            case (SyntaxKind.EqualsToken, SyntaxKind.GreaterEqualsToken):
            case (SyntaxKind.EqualsToken, SyntaxKind.GreaterGreaterToken):
            case (SyntaxKind.EqualsToken, SyntaxKind.GreaterGreaterEqualsToken):
                return true;

            case (SyntaxKind.LessToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.LessToken, SyntaxKind.LessToken):
            case (SyntaxKind.LessToken, SyntaxKind.LessEqualsToken):
            case (SyntaxKind.LessToken, SyntaxKind.LessLessToken):
            case (SyntaxKind.LessToken, SyntaxKind.LessLessEqualsToken):
            case (SyntaxKind.LessToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.LessToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.LessLessToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.LessLessToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.LessLessToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.GreaterToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.GreaterToken, SyntaxKind.GreaterToken):
            case (SyntaxKind.GreaterToken, SyntaxKind.GreaterEqualsToken):
            case (SyntaxKind.GreaterToken, SyntaxKind.GreaterGreaterToken):
            case (SyntaxKind.GreaterToken, SyntaxKind.GreaterGreaterEqualsToken):
            case (SyntaxKind.GreaterToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.GreaterToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.GreaterGreaterToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.GreaterGreaterToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.GreaterGreaterToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.HatToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.HatToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.HatToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.HookToken, SyntaxKind.HookToken):
            case (SyntaxKind.HookToken, SyntaxKind.HookHookToken):
            case (SyntaxKind.HookToken, SyntaxKind.HookHookEqualsToken):
            case (SyntaxKind.HookHookToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.HookHookToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.HookHookToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.MinusToken, SyntaxKind.ArrowToken):
            case (SyntaxKind.MinusToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.MinusToken, SyntaxKind.MinusToken):
            case (SyntaxKind.MinusToken, SyntaxKind.MinusEqualsToken):
            case (SyntaxKind.MinusToken, SyntaxKind.MinusMinusToken):
            case (SyntaxKind.MinusToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.MinusToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.MinusToken, SyntaxKind.GreaterToken):
            case (SyntaxKind.MinusToken, SyntaxKind.GreaterEqualsToken):
            case (SyntaxKind.MinusToken, SyntaxKind.GreaterGreaterToken):
            case (SyntaxKind.MinusToken, SyntaxKind.GreaterGreaterEqualsToken):
                return true;

            case (SyntaxKind.PercentToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.PercentToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.PercentToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.PipeToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.PipeToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.PipeToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.PipeToken, SyntaxKind.PipeToken):
            case (SyntaxKind.PipeToken, SyntaxKind.PipeEqualsToken):
            case (SyntaxKind.PipeToken, SyntaxKind.PipePipeToken):
                return true;

            case (SyntaxKind.PlusToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.PlusToken, SyntaxKind.PlusToken):
            case (SyntaxKind.PlusToken, SyntaxKind.PlusEqualsToken):
            case (SyntaxKind.PlusToken, SyntaxKind.PlusPlusToken):
            case (SyntaxKind.PlusToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.PlusToken, SyntaxKind.EqualsEqualsToken):
                return true;

            case (SyntaxKind.StrLiteralToken, SyntaxKind.StrLiteralToken):
                return true;

            case (SyntaxKind.SlashToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.SlashToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.SlashToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.SlashToken, SyntaxKind.SlashToken):
            case (SyntaxKind.SlashToken, SyntaxKind.SlashEqualsToken):
            case (SyntaxKind.SlashToken, SyntaxKind.StarToken):
            case (SyntaxKind.SlashToken, SyntaxKind.StarEqualsToken):
            case (SyntaxKind.SlashToken, SyntaxKind.StarStarToken):
            case (SyntaxKind.SlashToken, SyntaxKind.StarStarEqualsToken):
            case (SyntaxKind.SlashToken, SyntaxKind.SingleLineCommentTrivia):
            case (SyntaxKind.SlashToken, SyntaxKind.MultiLineCommentTrivia):
                return true;

            case (SyntaxKind.StarToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.StarToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.StarToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.StarToken, SyntaxKind.StarToken):
            case (SyntaxKind.StarToken, SyntaxKind.StarEqualsToken):
            case (SyntaxKind.StarToken, SyntaxKind.StarStarToken):
            case (SyntaxKind.StarToken, SyntaxKind.StarStarEqualsToken):
                return true;

            case (SyntaxKind.StarStarToken, SyntaxKind.LambdaToken):
            case (SyntaxKind.StarStarToken, SyntaxKind.EqualsToken):
            case (SyntaxKind.StarStarToken, SyntaxKind.EqualsEqualsToken):
            case (SyntaxKind.StarStarToken, SyntaxKind.StarToken):
            case (SyntaxKind.StarStarToken, SyntaxKind.StarEqualsToken):
            case (SyntaxKind.StarStarToken, SyntaxKind.StarStarToken):
            case (SyntaxKind.StarStarToken, SyntaxKind.StarStarEqualsToken):
                return true;
        }


        return false;
    }
}
#endif
