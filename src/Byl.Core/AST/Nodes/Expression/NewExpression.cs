namespace Byl.Core.AST.Nodes.Expression;

public class NewExpression : ExpressionNode
{
    public string ClassName { get; }
    public List<ExpressionNode> Arguments { get; }

    public NewExpression(int line, string className, List<ExpressionNode> arguments) : base(line)
    {
        ClassName = className;
        Arguments = arguments;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
