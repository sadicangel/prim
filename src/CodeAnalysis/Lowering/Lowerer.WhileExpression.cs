using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundExpression LowerWhileExpression(BoundWhileExpression node, Context context)
    {
        // source: while-expression
        //  while (<condition>)
        //      <body>
        //
        // target: block-expression
        //  goto check<$>
        //  continue<$>:
        //  <body>
        //  check<$>:
        //  goto continue<$> when <condition> is true
        //  break<$>:

        var checkLabel = context.CreateLabel("check");

        var expression = new BoundBlockExpression(
            node.Syntax,
            node.Type,
            [
                new BoundGotoExpression(checkLabel.Syntax, checkLabel, new BoundNopExpression(checkLabel.Syntax)),
                new BoundLabelDeclaration(node.ContinueLabel.Syntax, node.ContinueLabel),
                node.Body,
                new BoundLabelDeclaration(checkLabel.Syntax, checkLabel),
                new BoundConditionalGotoExpression(node.ContinueLabel.Syntax, node.ContinueLabel, node.Condition, new BoundNopExpression(node.ContinueLabel.Syntax), JumpTrue: true),
                new BoundLabelDeclaration(node.BreakLabel.Syntax, node.BreakLabel)
            ]);

        return LowerExpression(expression, context);
    }
}
