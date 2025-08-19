using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Statement;

public class WhileStatement : StatementNode
{
    public ExpressionNode Condition { get; }
    public StatementNode Body { get; }

    public WhileStatement(ExpressionNode condition, StatementNode body, int line)
        : base(line)
    {
        Condition = condition;
        Body = body;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}

