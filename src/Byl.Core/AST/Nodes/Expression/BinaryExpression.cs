using Byl.Core.Lexer;

namespace Byl.Core.AST.Nodes.Expression;

//Бинарные операции (2 + 3)
public class BinaryExpression : ExpressionNode
{
    public ExpressionNode Left { get; }
    public Token Operator { get; }
    public ExpressionNode Right { get; }

    public BinaryExpression(
        ExpressionNode left,
        Token @operator,
        ExpressionNode right,
        int line) : base(line)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override TResult Accept<TResult>(IAstVisitor<TResult> visitor) =>
        visitor.Visit(this);
}
