using CodeAnalysis.Binding;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Lowering;

internal sealed class Lowerer : BoundTreeRewriter
{
    private int _labelCount;

    private Lowerer() { }

    public static BoundProgram Lower(BoundProgram program) => new(Lower(program.Statement), program.Diagnostics);

    public static BoundBlockStatement Lower(BoundStatement statement)
    {
        var lowerer = new Lowerer();
        var result = lowerer.Rewrite(statement);
        return Flatten(result);

        static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var statements = new List<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current is BoundBlockStatement block)
                {
                    foreach (var child in block.Statements.Reverse())
                        stack.Push(child);
                }
                else
                {
                    statements.Add(current);
                }
            }

            return new BoundBlockStatement(statements);
        }
    }

    private LabelSymbol NewLabel() => new($"Label{++_labelCount}");

    protected override BoundStatement Rewrite(BoundIfStatement node)
    {
        if (!node.HasElseClause)
        {
            //  if (<condition>)
            //      <then>
            //
            //  >>>
            //
            //  goTo if-false <condition> end
            //  <then>
            //  end:
            //
            var endLabel = NewLabel();
            var gotoEndIfFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, JumpIfTrue: false);
            var endLabelStatement = new BoundLabelStatement(endLabel);
            var result = new BoundBlockStatement(new List<BoundStatement>
            {
                gotoEndIfFalse,
                node.Then,
                endLabelStatement
            });
            return Rewrite(result);
        }
        else
        {
            //
            //  if (<condition>)
            //      <then>
            //  else
            //      <else>
            //
            //  >>>
            //
            //  goTo if-false <condition> else
            //  <then>
            //  goto end
            //  else:
            //  <else>
            //  end:

            var elseLabel = NewLabel();
            var endLabel = NewLabel();

            var gotoElseIfFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, JumpIfTrue: false);
            var gotoEnd = new BoundGotoStatement(endLabel);

            var elseLabelStatement = new BoundLabelStatement(elseLabel);
            var endLabelStatement = new BoundLabelStatement(endLabel);
            var result = new BoundBlockStatement(new List<BoundStatement>
            {
                gotoElseIfFalse,
                node.Then,
                gotoEnd,
                elseLabelStatement,
                node.Else,
                endLabelStatement
            });
            return Rewrite(result);
        }
    }

    protected override BoundStatement Rewrite(BoundWhileStatement node)
    {
        //  while (<condition>)
        //      <body>
        //
        //  >>>
        //
        //  goto check:
        //  continue:
        //  <body>
        //  check:
        //  goto if-true <condition> continue
        //  end:
        //

        var continueLabel = NewLabel();
        var checkLabel = NewLabel();
        var endLabel = NewLabel();

        var gotoCheck = new BoundGotoStatement(checkLabel);
        var continueLabelStatement = new BoundLabelStatement(continueLabel);
        var checkLabelStatement = new BoundLabelStatement(checkLabel);
        var gotoContinueIfTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition);
        var endLabelStatement = new BoundLabelStatement(endLabel);

        var result = new BoundBlockStatement(new List<BoundStatement>
        {
            gotoCheck,
            continueLabelStatement,
            node.Body,
            checkLabelStatement,
            gotoContinueIfTrue,
            endLabelStatement
        });
        return Rewrite(result);
    }

    protected override BoundStatement Rewrite(BoundForStatement node)
    {
        //  for (let <var> in <lower>..<upper>)
        //      <body>
        //  >>>
        //
        //  {
        //      var <var> = <lower>;
        //      let <upperBound> = <upper>
        //      while (<var> < <upperBound>) {
        //          <body>
        //          <var> = <var> + 1;
        //  }

        var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
        var variableExpression = new BoundSymbolExpression(node.Variable);
        var upperBoundSymbol = new VariableSymbol("<>upperBound", true, node.Variable.Type);
        var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
        var condition = new BoundBinaryExpression(
            variableExpression,
            BoundBinaryOperator.Bind(TokenKind.Less, BuiltinTypes.I32, BuiltinTypes.I32, BuiltinTypes.Bool)!,
            new BoundSymbolExpression(upperBoundSymbol));
        var increment = new BoundExpressionStatement(
            new BoundAssignmentExpression(
                node.Variable,
                new BoundBinaryExpression(
                    variableExpression,
                    BoundBinaryOperator.Bind(TokenKind.Plus, BuiltinTypes.I32, BuiltinTypes.I32, BuiltinTypes.I32)!,
                    new BoundLiteralExpression(1)
                )));

        var body = new BoundBlockStatement(new List<BoundStatement> { node.Body, increment });
        var @while = new BoundWhileStatement(condition, body);

        var result = new BoundBlockStatement(new List<BoundStatement> { variableDeclaration, upperBoundDeclaration, @while });

        return Rewrite(result);
    }
}
