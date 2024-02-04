using CodeAnalysis.Syntax;
using CodeAnalysis.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace CodeAnalysis.Binding;

internal record class BoundProgram(
    SyntaxNode SyntaxNode,
    IReadOnlyList<BoundNode> Nodes,
    DiagnosticBag Diagnostics
)
    : BoundNode(BoundNodeKind.Program, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children() => Nodes;

    public IRenderable ToRenderable()
    {
        var tree = new Tree($"[aqua]{NodeKind}[/]")
            .Style("dim white");

        WriteTo(this, tree);

        return tree;

        static void WriteTo(BoundNode boundNode, IHasTreeNodes treeNode)
        {
            foreach (var child in boundNode.Children())
            {
                switch (child)
                {
                    //case Token token:
                    //    foreach (var trivia in token.Trivia.Leading)
                    //        treeNode.AddNode($"[grey66 i]{trivia.TokenKind}[/]");
                    //    if (token.Value is not null)
                    //        treeNode.AddNode($"[green3]{token.TokenKind}Token[/] {FormatLiteral(token)}");
                    //    else
                    //        treeNode.AddNode($"[green3]{token.TokenKind}Token[/] [darkseagreen2 i]{token.Text.ToString()}[/]");
                    //    foreach (var trivia in token.Trivia.Trailing)
                    //        treeNode.AddNode($"[grey66 i]{trivia.TokenKind}[/]");
                    //    break;

                    //case TypeSyntax type:
                    //    WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/] [darkseagreen2 i]{type.Text}[/]"));
                    //    // $"{base.Name}: {Type.Name}"
                    //    break;

                    default:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/] [darkseagreen2 i]{child.SyntaxNode.Text}[/]"));
                        break;
                }
            }
        }

        //static string FormatLiteral(Token token)
        //{
        //    return token.TokenKind switch
        //    {
        //        TokenKind.I32 => $"[gold3]{token.Value}[/]",
        //        TokenKind.F32 => $"[gold3]{token.Value}[/]",
        //        TokenKind.Str => $"[darkorange3]\"{token.Value}\"[/]",
        //        TokenKind.True => $"[blue3_1]{token.Value}[/]",
        //        TokenKind.False => $"[blue3_1]{token.Value}[/]",
        //        TokenKind.Null => $"[blue3_1]{token.Value}[/]",
        //        _ => throw new UnreachableException($"Unexpected {nameof(TokenKind)} '{token.TokenKind}'"),
        //    };
        //}
    }
}
