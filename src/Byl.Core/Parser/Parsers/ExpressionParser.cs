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
        var expr = ParseComparison();

        while (_parser.Match(TokenType.NotEqual, TokenType.Equal))
        {
            var op = _parser.Advance();
            var right = ParseComparison();
            expr = new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

    // Сравнения (<, >, <=, >=)
    private ExpressionNode ParseComparison()
    {
        var expr = ParseAdditive();

        while (_parser.Match(TokenType.LessThan, TokenType.GreaterThan))
        {
            var op = _parser.Advance();
            var right = ParseAdditive();
            expr = new BinaryExpression(expr, op, right, op.Line);
        }

        return expr;
    }

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

            // Если после идентификатора идет '(', то это вызов функции
            if (_parser.Match(TokenType.LParen))
            {
                return ParseFunctionCall(token.Value);
            }

            // Иначе это переменная
            return new VariableExpression(token.Value, token.Line);
        }

        if (_parser.Match(TokenType.StringLiteral))
        {
            var token = _parser.Advance();
            return new LiteralExpression(token.Value, token.Line);
        }

        if (_parser.Match(TokenType.LParen))
        {
            _parser.Advance(); // Пропускаем '('
            var expr = Parse();
            _parser.Consume(TokenType.RParen, "Ожидалось ')'");
            return expr;
        }

        if (_parser.Match(TokenType.InterpolatedString))
        {
            return ParseInterpolatedString();
        }

        throw _parser.UnexpectedToken(_parser.Current);
    }

    private ExpressionNode ParseInterpolatedString()
    {
        var token = _parser.Advance();

        // Просто возвращаем литерал с сырым текстом
        // Кодогенератор будет обрабатывать интерполяцию
        return new LiteralExpression(token.Value, token.Line);
    }

    private FunctionCallExpression ParseFunctionCall(string functionName)
    {
        _parser.Consume(TokenType.LParen, "Ожидалось '(' после имени функции");

        var arguments = new List<ExpressionNode>();

        // Парсим аргументы, если они есть
        if (!_parser.Match(TokenType.RParen))
        {
            do
            {
                arguments.Add(Parse());
            }
            while (_parser.Match(TokenType.Comma) && _parser.Advance() != null);
        }

        _parser.Consume(TokenType.RParen, "Ожидалось ')' после аргументов");
        return new FunctionCallExpression(functionName, arguments, _parser.Current.Line);
    }
}
