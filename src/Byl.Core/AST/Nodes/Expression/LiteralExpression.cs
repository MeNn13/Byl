namespace Byl.Core.AST.Nodes.Expression;

public class LiteralExpression : ExpressionNode
{
    public object Value { get; }

    public LiteralExpression(object value, int line) : base(line)
    {
        Value = value;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
