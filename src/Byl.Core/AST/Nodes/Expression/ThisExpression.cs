namespace Byl.Core.AST.Nodes.Expression;

public class ThisExpression : ExpressionNode
{
    public ThisExpression(int line) : base(line)
    {
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
