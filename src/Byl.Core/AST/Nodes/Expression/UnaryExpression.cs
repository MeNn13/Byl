using Byl.Core.Lexer;

namespace Byl.Core.AST.Nodes.Expression;

public class UnaryExpression : ExpressionNode
{
    public Token Operator { get; }
    public ExpressionNode Right { get; }

    public UnaryExpression(Token @operator, ExpressionNode right, int line)
        : base(line)
    {
        Operator = @operator;
        Right = right;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
