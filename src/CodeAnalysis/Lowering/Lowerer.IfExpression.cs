﻿using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundExpression LowerIfExpression(BoundIfExpression node, Context context)
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

        var elseLabel = context.CreateLabel("else", node.Type.ContainingModule);
        var endLabel = context.CreateLabel("end", node.Type.ContainingModule);

        var expression = new BoundBlockExpression(
            node.Syntax,
            node.Type,
            [
                new BoundConditionalGotoExpression(node.Condition.Syntax, elseLabel, node.Condition, new BoundNopExpression(node.Condition.Syntax, context.BoundScope.Unknown), JumpTrue: false),
                node.Then,
                new BoundGotoExpression(endLabel.Syntax, endLabel, new BoundNopExpression(endLabel.Syntax, context.BoundScope.Unknown)),
                new BoundLabelDeclaration(elseLabel.Syntax, elseLabel),
                node.Else,
                new BoundLabelDeclaration(endLabel.Syntax, endLabel),
            ]);

        return LowerExpression(expression, context);
    }
}
