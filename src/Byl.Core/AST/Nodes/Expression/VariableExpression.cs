namespace Byl.Core.AST.Nodes.Expression;

public class VariableExpression : ExpressionNode
{
    public string Name { get; }

    public VariableExpression(string name, int line) : base(line)
    {
        Name = name;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
