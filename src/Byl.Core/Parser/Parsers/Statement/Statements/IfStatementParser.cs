using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement.Statements;

internal class IfStatementParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse() => _parser.Match(TokenType.If);

    public StatementNode Parse()
    {
        var ifToken = _parser.Advance();
        _parser.Consume(TokenType.LParen, "Ожидалось '(' после 'если'");

        var condition = new ExpressionParser(_parser).Parse();
        _parser.Consume(TokenType.RParen, "Ожидалось ')' после условия");

        var thenBranch = new StatementParser(_parser).Parse();
        StatementNode? elseBranch = null;

        if (_parser.Match(TokenType.Else))
        {
            _parser.Advance();
            elseBranch = new StatementParser(_parser).Parse();
        }

        return new IfStatement(condition, thenBranch, elseBranch, ifToken.Line);
    }
}
