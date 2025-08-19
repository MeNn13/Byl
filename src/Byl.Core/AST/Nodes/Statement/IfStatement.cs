using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Statement;

public class IfStatement : StatementNode
{
    public ExpressionNode Condition { get; }
    public StatementNode ThenBranch { get; }
    public StatementNode? ElseBranch { get; }

    public IfStatement(ExpressionNode condition, StatementNode thenBranch,
                     StatementNode? elseBranch, int line) : base(line)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}

