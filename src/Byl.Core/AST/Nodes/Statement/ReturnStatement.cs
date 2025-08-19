using Byl.Core.AST.Nodes.Expression;

namespace Byl.Core.AST.Nodes.Statement;

public class ReturnStatement : StatementNode
{
    public ExpressionNode? Value { get; }

    public ReturnStatement(ExpressionNode? value, int line) : base(line)
    {
        Value = value;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}

