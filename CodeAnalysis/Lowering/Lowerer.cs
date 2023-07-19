using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Statements;
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
        return RemoveDeadCode(Flatten(result));

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

    private static BoundBlockStatement RemoveDeadCode(BoundBlockStatement block)
    {
        var controlFlow = ControlFlowGraph.Create(block);

        var reachableNodes = new HashSet<BoundStatement>(controlFlow.Blocks.SelectMany(b => b.Statements));

        // We should also remove jumps and labels that just fall through
        // the next statement. Maybe inspect the graph instead?

        var statements = block.Statements.ToList();
        for (var i = statements.Count - 1; i >= 0; --i)
        {
            if (!reachableNodes.Contains(statements[i]))
                statements.RemoveAt(i);
        }

        if (statements.Count != block.Statements.Count)
            return new BoundBlockStatement(statements);

        return block;
    }

    private LabelSymbol NewLabel(string prefix = "Label") => new($"{prefix}{++_labelCount}");

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
            var result = new BoundBlockBuilder()
                .GotoIf(false, node.Condition, endLabel)
                .Statement(node.Then)
                .Label(endLabel)
                .Build();
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

            var result = new BoundBlockBuilder()
                .GotoIf(false, node.Condition, elseLabel)
                .Statement(node.Then)
                .Goto(endLabel)
                .Label(elseLabel)
                .Statement(node.Else)
                .Label(endLabel)
                .Build();
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

        var bodyLabel = NewLabel();

        var result = new BoundBlockBuilder()
            .Goto(node.Continue)
            .Label(bodyLabel)
            .Statement(node.Body)
            .Label(node.Continue)
            .GotoIf(true, node.Condition, bodyLabel)
            .Label(node.Break)
            .Build();
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
        //          continue:
        //          <var> = <var> + 1;
        //  }

        var variableExpression = new BoundSymbolExpression(node.Variable);
        var upperBoundSymbol = new VariableSymbol("#upperBound", IsReadOnly: true, node.Variable.Type);

        var result = new BoundBlockBuilder()
            .Declare(node.Variable, node.LowerBound)
            .Declare(upperBoundSymbol, node.UpperBound)
            .While(
                Expressions.LessThan(BuiltinTypes.I32, variableExpression, new BoundSymbolExpression(upperBoundSymbol)),
                node.Break,
                new LabelSymbol("#continue"),
                body => body
                    .Statement(node.Body)
                    .Label(node.Continue)
                    .Assign(node.Variable, Expressions.Increment(variableExpression)))
            .Build();

        return Rewrite(result);
    }

    protected override BoundStatement Rewrite(BoundConditionalGotoStatement node)
    {
        if (node.Condition.ConstantValue is null)
            return base.Rewrite(node);

        var condition = (bool)node.Condition.ConstantValue.Value!;
        condition = node.JumpIfTrue ? condition : !condition;
        BoundStatement result = condition ? new BoundGotoStatement(node.Label) : new BoundNopStatement();

        return Rewrite(result);
    }
}

file static class Expressions
{
    public static BoundExpression LessThan(TypeSymbol type, BoundExpression left, BoundExpression right)
    {
        var @operator = BoundBinaryOperator.Bind(TokenKind.Less, type, type, BuiltinTypes.Bool) ?? throw new InvalidOperationException("Unexpected operator");
        return new BoundBinaryExpression(left, @operator, right);
    }

    public static BoundExpression Increment(BoundExpression left)
    {
        var @operator = BoundBinaryOperator.Bind(TokenKind.Plus, BuiltinTypes.I32, BuiltinTypes.I32, BuiltinTypes.I32) ?? throw new InvalidOperationException("Unexpected operator");
        return new BoundBinaryExpression(left, @operator, new BoundLiteralExpression(1));
    }
}

file readonly struct BoundBlockBuilder
{
    private readonly List<BoundStatement> _statements;

    public BoundBlockBuilder() => _statements = new List<BoundStatement>();

    public BoundBlockStatement Build()
    {
        var block = new BoundBlockStatement(_statements.ToArray());
        _statements.Clear();
        return block;
    }

    public BoundBlockBuilder Statement(BoundStatement statement)
    {
        _statements.Add(statement);
        return this;
    }

    public BoundBlockBuilder Declare(VariableSymbol variable, BoundExpression expression)
    {
        _statements.Add(new BoundVariableDeclaration(variable, expression));
        return this;
    }

    public BoundBlockBuilder Label(LabelSymbol label)
    {
        _statements.Add(new BoundLabelDeclaration(label));
        return this;
    }

    public BoundBlockBuilder Goto(LabelSymbol label)
    {
        _statements.Add(new BoundGotoStatement(label));
        return this;
    }

    public BoundBlockBuilder GotoIf(bool jumpIfTrue, BoundExpression condition, LabelSymbol label)
    {
        _statements.Add(new BoundConditionalGotoStatement(label, condition, jumpIfTrue));
        return this;
    }

    public BoundBlockBuilder Assign(VariableSymbol variable, BoundExpression expression)
    {
        _statements.Add(new BoundExpressionStatement(new BoundAssignmentExpression(variable, expression)));
        return this;
    }

    public BoundBlockBuilder While(BoundExpression condition, LabelSymbol @break, LabelSymbol @continue, Action<BoundBlockBuilder> buildBody)
    {
        var bodyBuilder = new BoundBlockBuilder();
        buildBody.Invoke(bodyBuilder);
        _statements.Add(new BoundWhileStatement(condition, bodyBuilder.Build(), @break, @continue));
        return this;
    }
}