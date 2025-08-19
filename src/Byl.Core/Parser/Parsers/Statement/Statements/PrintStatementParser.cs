using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement.Statements;

internal class PrintStatementParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse() => _parser.Match(TokenType.Print);

    public StatementNode Parse()
    {
        var printToken = _parser.Advance();
        _parser.Consume(TokenType.LParen, "Ожидалось '(' после 'печать'");

        var expression = new ExpressionParser(_parser).Parse();

        _parser.Consume(TokenType.RParen, "Ожидалось ')' после выражения");
        _parser.Consume(TokenType.Semicolon, "Ожидалось ';'");

        return new PrintStatement(expression, printToken.Line);
    }
}
