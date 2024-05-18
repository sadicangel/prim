using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Xunit;

namespace CodeAnalysis.Tests.Syntax;
public sealed class SyntaxFactsTests
{
    [Theory]
    [MemberData(nameof(GetSyntaxKindData))]
    public void GetText_result_can_be_scanned_into_a_token(SyntaxKind syntaxKind)
    {
        var text = SyntaxFacts.GetText(syntaxKind);
        if (text is null)
            return;

        var token = Assert.Single(SyntaxTree.Scan(new SourceText(text)).SkipLast(1));
        Assert.Equal(syntaxKind, token.SyntaxKind);
        Assert.Equal(text, token.Text);
    }

    public static TheoryData<SyntaxKind> GetSyntaxKindData() => new(Enum.GetValues<SyntaxKind>());
}
