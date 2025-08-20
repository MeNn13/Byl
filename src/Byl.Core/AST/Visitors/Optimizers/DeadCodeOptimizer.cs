using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Visitors.Optimizers;

internal class DeadCodeOptimizer : BaseOptimizer
{
    public override string Name => "Dead Code Elimination";

    public override bool CanOptimize(Node node) => node is StatementNode;

    protected override Node VisitIf(IfStatement node)
    {
        var condition = (ExpressionNode)Visit(node.Condition);
        var thenBranch = (StatementNode)Visit(node.ThenBranch);
        var elseBranch = node.ElseBranch is not null ? (StatementNode)Visit(node.ElseBranch) : null;

        if (condition is LiteralExpression { Value: true })
            return thenBranch;

        if (condition is LiteralExpression { Value: false })
            return elseBranch ?? new BlockStatement([], node.Line);

        return new IfStatement(condition, thenBranch, elseBranch, node.Line);
    }

    protected override Node VisitWhile(WhileStatement node)
    {
        var condition = (ExpressionNode)Visit(node.Condition);
        var body = (StatementNode)Visit(node.Body);

        if (condition is LiteralExpression { Value: false })
            return new BlockStatement([], node.Line);

        return new WhileStatement(condition, body, node.Line);
    }
    protected override Node VisitFunctionCall(FunctionCallExpression node)
    {
        var optimizedArgs = new List<ExpressionNode>();
        foreach (var arg in node.Arguments)
        {
            optimizedArgs.Add((ExpressionNode)Visit(arg));
        }
        return new FunctionCallExpression(node.FunctionName, optimizedArgs, node.Line);
    }
}
