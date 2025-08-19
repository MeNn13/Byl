using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Statement;

public class PrintStatement : StatementNode
{
    public ExpressionNode Expression { get; }

    public PrintStatement(ExpressionNode expression, int line) : base(line)
    {
        Expression = expression;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
