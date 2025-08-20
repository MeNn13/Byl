using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors.Optimize;

public class ConstantFolder : IAstVisitor<ExpressionNode>
{
    public ExpressionNode Visit(BinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        // Оптимизация: 0 + x → x
        if (node.Operator.Type == TokenType.Plus && left is LiteralExpression { Value: 0 })
            return right;

        // Оптимизация: x + 0 → x  
        if (node.Operator.Type == TokenType.Plus && right is LiteralExpression { Value: 0 })
            return left;

        // Оптимизация: 1 * x → x
        if (node.Operator.Type == TokenType.Multiply && left is LiteralExpression { Value: 1 })
            return right;

        // Оптимизация: x * 1 → x
        if (node.Operator.Type == TokenType.Multiply && right is LiteralExpression { Value: 1 })
            return left;

        // Оптимизация: 0 * x → 0
        if (node.Operator.Type == TokenType.Multiply &&
            (left is LiteralExpression { Value: 0 } || right is LiteralExpression { Value: 0 }))
            return new LiteralExpression(0, node.Line);

        // Сворачиваем константы для целых чисел
        if (left is LiteralExpression lLit && right is LiteralExpression rLit)
        {
            if (lLit.Value is int lInt && rLit.Value is int rInt)
            {
                try
                {
                    object? result = node.Operator.Type switch
                    {
                        TokenType.Plus => lInt + rInt,
                        TokenType.Minus => lInt - rInt,
                        TokenType.Multiply => lInt * rInt,
                        TokenType.Divide => rInt != 0 ? lInt / rInt : null,
                        TokenType.Equal => lInt == rInt ? 1 : 0,
                        TokenType.NotEqual => lInt != rInt ? 1 : 0,
                        TokenType.GreaterThan => lInt > rInt ? 1 : 0,
                        TokenType.LessThan => lInt < rInt ? 1 : 0,
                        _ => null
                    };

                    if (result != null)
                        return new LiteralExpression(result, node.Line);
                }
                catch
                {
                    // В случае ошибки возвращаем оригинальное выражение
                }
            }

            // Сворачиваем булевы операции
            if (lLit.Value is bool lBool && rLit.Value is bool rBool)
            {
                object? result = node.Operator.Type switch
                {
                    TokenType.And => lBool && rBool,
                    TokenType.Or => lBool || rBool,
                    TokenType.Equal => lBool == rBool,
                    TokenType.NotEqual => lBool != rBool,
                    _ => null
                };

                if (result != null)
                    return new LiteralExpression(result, node.Line);
            }
        }

        // Если не удалось свернуть, возвращаем оптимизированные подвыражения
        return new BinaryExpression(left, node.Operator, right, node.Line);
    }
    public ExpressionNode Visit(LiteralExpression node) => node;
    public ExpressionNode Visit(VariableExpression node) => node;
    public ExpressionNode Visit(UnaryExpression node)
    {
        var right = node.Right.Accept(this);

        // Сворачиваем унарные операции с литералами
        if (right is LiteralExpression rightLit)
        {
            if (rightLit.Value is int intValue)
            {
                object? result = node.Operator.Type switch
                {
                    TokenType.Minus => -intValue,
                    TokenType.Not => intValue == 0 ? 1 : 0,
                    _ => null
                };

                if (result != null)
                    return new LiteralExpression(result, node.Line);
            }

            if (rightLit.Value is bool boolValue)
            {
                object? result = node.Operator.Type switch
                {
                    TokenType.Not => !boolValue,
                    _ => null
                };

                if (result != null)
                    return new LiteralExpression(result, node.Line);
            }
        }

        return new UnaryExpression(node.Operator, right, node.Line);
    }

    public ExpressionNode Visit(TypeNode node) => throw new InvalidOperationException();
    public ExpressionNode Visit(ProgramNode node) => throw new InvalidOperationException();
    public ExpressionNode Visit(FunctionDeclaration node) => throw new InvalidOperationException();
    public ExpressionNode Visit(ParameterNode node) => throw new InvalidOperationException();
    public ExpressionNode Visit(BlockStatement node) => throw new InvalidOperationException();
    public ExpressionNode Visit(PrintStatement node) => throw new InvalidOperationException();
    public ExpressionNode Visit(AssignStatement node) => throw new InvalidOperationException();
    public ExpressionNode Visit(IfStatement node) => throw new InvalidOperationException();
    public ExpressionNode Visit(WhileStatement node) => throw new InvalidOperationException();
    public ExpressionNode Visit(ReturnStatement node) => throw new InvalidOperationException();
    public ExpressionNode Visit(VariableDeclarationStatement node) => throw new InvalidOperationException();
}
