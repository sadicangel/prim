using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundExpression LowerIfExpression(BoundIfExpression node, LowererContext context)
    {
        // source: if-expression
        //  if (<condition>)
        //      <then>
        //  else
        //      <else>
        //
        // target: block-expression
        //  goto else<$> when <condition> is false
        //  <then>
        //  goto end<$>
        //  else<$>:
        //  <else>
        //  end<$>:

        var elseLabel = context.CreateLabel("else");
        var endLabel = context.CreateLabel("end");

        var expression = new BoundBlockExpression(
            node.Syntax,
            node.Type,
            [
                new BoundConditionalGotoExpression(node.Condition.Syntax, elseLabel, node.Condition, new BoundNopExpression(node.Condition.Syntax), JumpTrue: false),
                node.Then,
                new BoundGotoExpression(endLabel.Syntax, endLabel, new BoundNopExpression(endLabel.Syntax)),
                new BoundLabelDeclaration(elseLabel.Syntax, elseLabel),
                node.Else,
                new BoundLabelDeclaration(endLabel.Syntax, endLabel),
            ]);

        return LowerExpression(expression, context);
    }
}
