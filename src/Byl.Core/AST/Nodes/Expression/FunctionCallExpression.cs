namespace Byl.Core.AST.Nodes.Expression;

public class FunctionCallExpression : ExpressionNode
{
    public string FunctionName { get; }
    public List<ExpressionNode> Arguments { get; }

    public FunctionCallExpression(string functionName, List<ExpressionNode> arguments, int line)
        : base(line)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}