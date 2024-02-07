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
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/] [darkseagreen2 i]{expression.Type}[/]"));
                        break;

                    default:
                        WriteTo(child, treeNode.AddNode($"[aqua]{child.NodeKind}[/]"));
                        break;
                }
            }
        }
    }
}
