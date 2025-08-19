using Byl.Core.AST.Nodes.Expression;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers;

public class ExpressionParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public ExpressionNode Parse() =>
        ParseAssignment();

    // Присваивание (низший приоритет)
    private ExpressionNode ParseAssignment()
    {
        var expr = ParseLogicalOr();

        if (_parser.Match(TokenType.Assign))
        {
            var op = _parser.Advance();
            var right = ParseAssignment();
            return new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    // Логическое ИЛИ
    private ExpressionNode ParseLogicalOr()
    {
        var expr = ParseLogicalAnd();

        while (_parser.Match(TokenType.Or)) // ||
        {
            var op = _parser.Advance();
            var right = ParseLogicalAnd();
            return new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    // Логическое И
    private ExpressionNode ParseLogicalAnd()
    {
        var expr = ParseEquality();

        while (_parser.Match(TokenType.And)) // &&
        {
            var op = _parser.Advance();
            var right = ParseEquality();
            return new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    // Равенство (!=, ==)
    private ExpressionNode ParseEquality()
    {
        var expr = ParseAdditive();

        while (_parser.Match(TokenType.NotEqual, TokenType.Equal))
        {
            var op = _parser.Advance();
            var right = ParseAdditive();
            return new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    //// Сравнения (<, >, <=, >=)
    //private ExpressionNode ParseComparison()
    //{
    //    var expr = ParseAdditive();

    //    while (_parser.Match(TokenType.Less, TokenType.Greater,
    //                       TokenType.LessEqual, TokenType.GreaterEqual))
    //    {
    //        var op = _parser.Current;
    //        var right = ParseAdditive();
    //        return new BinaryExpression(expr, op, right, op.Line);
    //    }

    //    return expr;
    //}

    // Сложение/вычитание
    private ExpressionNode ParseAdditive()
    {
        var expr = ParseMultiplicative();

        while (_parser.Match(TokenType.Plus, TokenType.Minus))
        {
            var op = _parser.Advance();
            var right = ParseMultiplicative();
            return new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    // Умножение/деление
    private ExpressionNode ParseMultiplicative()
    {
        var expr = ParsePrimary();

        while (_parser.Match(TokenType.Multiply, TokenType.Divide))
        {
            var op = _parser.Advance();
            var right = ParsePrimary();
            return new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    // Унарные операции (-, !)
    //private ExpressionNode ParseUnary()
    //{
    //    if (_parser.Match(TokenType.Minus, TokenType.Not))
    //    {
    //        var op = _parser.Current;
    //        var right = ParseUnary();
    //        return new UnaryExpression
    //        {
    //            Operator = op,
    //            Right = right,
    //            Line = op.Line
    //        };
    //    }

    //    return ParsePrimary();
    //}

    // Базовые элементы
    private ExpressionNode ParsePrimary()
    {
        if (_parser.Match(TokenType.Number))
        {
            var token = _parser.Advance();
            return new LiteralExpression(int.Parse(token.Value), token.Line);
        }

        if (_parser.Match(TokenType.Identifier))
        {
            var token = _parser.Advance();
            return new VariableExpression(token.Value, token.Line);
        }

        if (_parser.Match(TokenType.LParen))
        {
            var expr = Parse();
            _parser.Consume(TokenType.RParen, "Ожидалось ')'");
            return expr;
        }

        throw _parser.UnexpectedToken(_parser.Current);
    }
}
