using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Operators;
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
                    case BoundOperator @operator:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/] [darkseagreen2 i]{@operator.Operator.OperatorKind}[/]"));
                        break;

                    case BoundExpression expression:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/] [darkseagreen2 i]{expression.Type.Name}[/]"));
                        break;

                    default:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/]"));
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
