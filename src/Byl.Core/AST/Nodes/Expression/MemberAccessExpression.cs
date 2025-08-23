namespace Byl.Core.AST.Nodes.Expression;

public class MemberAccessExpression : ExpressionNode
{
    public ExpressionNode Target { get; }
    public string MemberName { get; }

    public MemberAccessExpression(int line, ExpressionNode target, string memberName) : base(line)
    {
        Target = target;
        MemberName = memberName;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
