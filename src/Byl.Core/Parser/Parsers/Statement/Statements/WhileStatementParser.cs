using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement.Statements;

internal class WhileStatementParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse() => _parser.Match(TokenType.While);

    public StatementNode Parse()
    {
        var whileToken = _parser.Advance();
        _parser.Consume(TokenType.LParen, "Ожидалось '(' после 'пока'");

        var condition = new ExpressionParser(_parser).Parse();
        _parser.Consume(TokenType.RParen, "Ожидалось ')' после условия");

        var body = new StatementParser(_parser).Parse();

        return new WhileStatement(condition, body, whileToken.Line);
    }
}
