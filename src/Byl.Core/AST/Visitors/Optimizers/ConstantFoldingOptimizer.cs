using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors.Optimizers;

internal class ConstantFoldingOptimizer : BaseOptimizer
{
    public override string Name => "Constant Folding";

    public override bool CanOptimize(Node node) => node is ExpressionNode;

    protected override Node VisitBinary(BinaryExpression node)
    {
        var left = (ExpressionNode)Visit(node.Left);
        var right = (ExpressionNode)Visit(node.Right);

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
    protected override Node VisitUnary(UnaryExpression node)
    {
        var right = (ExpressionNode)Visit(node.Right);

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

}
