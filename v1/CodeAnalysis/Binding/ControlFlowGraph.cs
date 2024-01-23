using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Statements;
using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace CodeAnalysis.Binding;

internal sealed class ControlFlowGraph : INode
{
    private ControlFlowGraph(List<BasicBlock> blocks, List<BasicBranch> branches)
    {
        Blocks = blocks;
        Branches = branches;
    }

    internal List<BasicBlock> Blocks { get; }
    internal List<BasicBranch> Branches { get; }

    internal sealed record class BasicBlock(bool IsStart, bool IsEnd)
    {
        public BasicBlock() : this(IsStart: false, IsEnd: false) { }

        public List<BoundStatement> Statements { get; } = new();
        public List<BasicBranch> Incoming { get; } = new();
        public List<BasicBranch> Outgoing { get; } = new();

        public override string ToString()
        {
            if (IsStart)
                return "<Start>";

            if (IsEnd)
                return "<End>";

            using var writer = new StringWriter();
            using var indented = new IndentedTextWriter(writer);
            foreach (var statement in Statements)
                statement.WriteTo(indented);

            return writer.ToString();
        }
    }

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal sealed record class BasicBranch(BasicBlock From, BasicBlock To, BoundExpression? Condition)
    {
        public override string ToString()
        {
            if (Condition is null)
                return String.Empty;

            return Condition.ToString();
        }

        private string GetDebuggerDisplay() => $"{From} -> {To} {ToString()}";
    }

    private sealed record class GraphBuilder(BoundBlockStatement Block)
    {
        public ControlFlowGraph Build()
        {
            var blockFromStatement = new Dictionary<BoundStatement, BasicBlock>();
            var blockFromLabel = new Dictionary<LabelSymbol, BasicBlock>();
            var start = new BasicBlock(IsStart: true, IsEnd: false);
            var end = new BasicBlock(IsStart: false, IsEnd: true);

            var blocks = EnumerateBlocks(Block).ToList();

            var branches = new List<BasicBranch>();

            if (!blocks.Any())
            {
                Connect(start, end);
            }
            else
            {
                Connect(start, blocks[0]);
            }
            foreach (var block in blocks)
            {
                foreach (var statement in block.Statements)
                {
                    blockFromStatement[statement] = block;
                    if (statement is BoundLabelDeclaration labelDeclaration)
                        blockFromLabel[labelDeclaration.Label] = block;
                }
            }

            for (int i = 0; i < blocks.Count; ++i)
            {
                var current = blocks[i];
                var next = i < blocks.Count - 1 ? blocks[i + 1] : end;

                foreach (var statement in current.Statements)
                {
                    var isLastStatementInBlock = statement == current.Statements[^1];

                    switch (statement.NodeKind)
                    {
                        case BoundNodeKind.GotoStatement:
                            var @goto = (BoundGotoStatement)statement;
                            var to = blockFromLabel[@goto.Label];
                            Connect(current, to);
                            break;
                        case BoundNodeKind.ConditionalGotoStatement:
                            var gotoIf = (BoundConditionalGotoStatement)statement;
                            var thenBlock = blockFromLabel[@gotoIf.Label];
                            var elseBlock = next;
                            var negatedCondition = Negate(gotoIf.Condition);
                            var thenCondition = gotoIf.JumpIfTrue ? gotoIf.Condition : negatedCondition;
                            var elseCondition = gotoIf.JumpIfTrue ? negatedCondition : gotoIf.Condition;
                            Connect(current, thenBlock, thenCondition);
                            Connect(current, elseBlock, elseCondition);
                            break;
                        case BoundNodeKind.ReturnStatement:
                            Connect(current, end);
                            break;
                        case BoundNodeKind.FunctionDeclaration or BoundNodeKind.VariableDeclaration or BoundNodeKind.LabelDeclaration or BoundNodeKind.ExpressionStatement or BoundNodeKind.NopStatement when isLastStatementInBlock:
                            Connect(current, next);
                            break;
                        case BoundNodeKind.FunctionDeclaration or BoundNodeKind.VariableDeclaration or BoundNodeKind.LabelDeclaration or BoundNodeKind.ExpressionStatement or BoundNodeKind.NopStatement:
                            break;
                        default:
                            throw new InvalidOperationException($"Unexpected statement {statement.NodeKind}");
                    }
                }
            }

        ScanAgain:
            foreach (var block in blocks)
            {
                if (block.Incoming.Count == 0)
                {
                    blocks.Remove(block);
                    foreach (var branch in block.Incoming)
                    {
                        branch.From.Outgoing.Remove(branch);
                        branches.Remove(branch);
                    }
                    foreach (var branch in block.Outgoing)
                    {
                        branch.To.Incoming.Remove(branch);
                        branches.Remove(branch);
                    }
                    goto ScanAgain;
                }
            }

            blocks.Insert(0, start);
            blocks.Add(end);

            return new ControlFlowGraph(blocks, branches);

            void Connect(BasicBlock from, BasicBlock to, BoundExpression? condition = null)
            {
                if (condition is BoundLiteralExpression literal)
                {
                    var value = (bool)literal.Value!;
                    // If true, connect unconditionally; else, don't connect.
                    if (value)
                        condition = null;
                    else
                        return;
                }
                var branch = new BasicBranch(from, to, condition);
                from.Outgoing.Add(branch);
                to.Incoming.Add(branch);
                branches.Add(branch);
            }

            BoundExpression Negate(BoundExpression condition)
            {
                if (condition is BoundLiteralExpression literal)
                    return new BoundLiteralExpression(condition.Syntax, !((bool)literal.Value!));

                var @operator = BoundUnaryOperator.Bind(TokenKind.Bang, PredefinedTypes.Bool).Single();
                return new BoundUnaryExpression(condition.Syntax, @operator, condition);
            }

            static IEnumerable<BasicBlock> EnumerateBlocks(BoundBlockStatement boundBlock)
            {
                var statements = new List<BoundStatement>();
                var blocks = new List<BasicBlock>();
                BasicBlock? block;
                foreach (var statement in boundBlock.Statements)
                {
                    switch (statement.NodeKind)
                    {
                        case BoundNodeKind.NopStatement:
                        case BoundNodeKind.VariableDeclaration:
                        case BoundNodeKind.FunctionDeclaration:
                        case BoundNodeKind.ExpressionStatement:
                            statements.Add(statement);
                            continue;
                        case BoundNodeKind.LabelDeclaration:
                            block = StartBlock();
                            statements.Add(statement);
                            break;
                        case BoundNodeKind.GotoStatement:
                        case BoundNodeKind.ConditionalGotoStatement:
                        case BoundNodeKind.ReturnStatement:
                            statements.Add(statement);
                            block = StartBlock();
                            break;
                        default:
                            throw new InvalidOperationException($"Unexpected statement {statement.NodeKind}");
                    }

                    if (block is not null)
                        yield return block;
                }

                block = EndBlock();

                if (block is not null)
                    yield return block;

                BasicBlock? StartBlock() => EndBlock();

                BasicBlock? EndBlock()
                {
                    if (statements.Count > 0)
                    {
                        var block = new BasicBlock();
                        block.Statements.AddRange(statements);
                        blocks.Add(block);
                        statements.Clear();
                        return block;
                    }

                    return null;
                }
            }
        }
    }

    public static ControlFlowGraph Create(BoundBlockStatement statement)
    {
        var builder = new GraphBuilder(statement);
        var graph = builder.Build();
#if DEBUG
        File.WriteAllText(Path.Combine(PathInfo.SolutionPath, "cfg.dot"), graph.ToString());
#endif
        return graph;
    }

    public static bool AllPathsReturn(BoundBlockStatement body)
    {
        var graph = Create(body);
        if (graph.Blocks.Count == 0 || graph.Blocks[^1].Incoming.Count == 0)
            return false;

        foreach (var branch in graph.Blocks[^1].Incoming)
        {
            if (branch.From.Statements.Count == 0 || branch.From.Statements[^1].NodeKind is not BoundNodeKind.ReturnStatement)
                return false;
        }
        return true;
    }

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }

    public void WriteTo(TextWriter writer, string indent = "", bool isLast = true)
    {
        writer.WriteLine("digraph G {");

        var blockIds = new Dictionary<BasicBlock, string>();
        for (var i = 0; i < Blocks.Count; ++i)
        {
            var id = $"N{i}";
            blockIds[Blocks[i]] = id;
            var label = Quote(Blocks[i].ToString().Replace(Environment.NewLine, "\\l"));
            writer.WriteLine($"    {id} [label = {label}, shape = box, fontname = \"Consolas\"]");
        }
        foreach (var branch in Branches)
        {
            var fromId = blockIds[branch.From];
            var toId = blockIds[branch.To];
            if (branch.Condition is not null)
            {
                var label = Quote(branch.ToString());
                writer.WriteLine($"    {fromId} -> {toId} [label = {label}, fontname = \"Consolas\"]");
            }
            else
            {
                writer.WriteLine($"    {fromId} -> {toId}");
            }
        }
        writer.WriteLine("}");

        static string Quote(string s) => '\"' + s + '\"';
    }
    public IEnumerable<INode> Children() => Enumerable.Empty<INode>();
}
