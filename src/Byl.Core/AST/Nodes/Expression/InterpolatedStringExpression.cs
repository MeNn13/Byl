namespace Byl.Core.AST.Nodes.Expression;

public class InterpolatedStringExpression : ExpressionNode
{
    public List<ExpressionNode> Parts { get; }

    public InterpolatedStringExpression(List<ExpressionNode> parts, int line)
        : base(line) => Parts = parts;

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
