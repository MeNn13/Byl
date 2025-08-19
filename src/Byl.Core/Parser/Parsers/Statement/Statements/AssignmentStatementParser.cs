using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement.Statements;

internal class AssignmentStatementParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse() =>
        _parser.Match(TokenType.Identifier) &&
        _parser.Peek()?.Type == TokenType.Assign;

    public StatementNode Parse()
    {
        var identifier = _parser.Advance();
        _parser.Consume(TokenType.Assign, "Ожидалось '='");

        var expression = new ExpressionParser(_parser).Parse();
        _parser.Consume(TokenType.Semicolon, "Ожидалось ';'");

        return new AssignStatement(identifier.Value, expression, identifier.Line);
    }
}
