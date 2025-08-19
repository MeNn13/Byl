using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement.Statements;

public class ReturnStatementParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse() => _parser.Match(TokenType.Return);

    public StatementNode Parse()
    {
        var returnToken = _parser.Advance();
        ExpressionNode? value = null;

        if (!_parser.Match(TokenType.Semicolon))
        {
            value = new ExpressionParser(_parser).Parse();
        }

        _parser.Consume(TokenType.Semicolon, "Ожидалось ';' после return");

        return new ReturnStatement(value, returnToken.Line);
    }
}