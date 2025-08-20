using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement;

internal class ExpressionStatementParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse()
    {
        // Выражения могут начинаться с идентификатора, числа, строки, унарных операторов
        return _parser.Match(TokenType.Identifier, TokenType.Number,
                           TokenType.StringLiteral, TokenType.Plus,
                           TokenType.Minus, TokenType.Not, TokenType.LParen);
    }

    public StatementNode Parse()
    {
        var expression = new ExpressionParser(_parser).Parse();
        _parser.Consume(TokenType.Semicolon, "Ожидалось ';' после выражения");
        return new ExpressionStatement(expression, expression.Line);
    }
}
