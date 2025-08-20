using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;
using Byl.Core.Parser.Parsers.Statement.Statements;

namespace Byl.Core.Parser.Parsers.Statement;

public class StatementParser(Parser parser)
{
    private readonly Parser _parser = parser;
    private readonly List<IStatementParser> _statementParsers =
       [
        new PrintStatementParser(parser),
        new AssignmentStatementParser(parser),
        new IfStatementParser(parser),
        new VariableDeclarationParser(parser),
        new WhileStatementParser(parser),
        new ReturnStatementParser(parser)
       ];

    public BlockStatement Parse()
    {
        _parser.Consume(TokenType.LBrace, "Ожидалось '{'");
        var statements = new List<StatementNode>();

        while (!_parser.Match(TokenType.RBrace) && !_parser.IsAtEnd)
        {
            statements.Add(ParseStatement());
        }

        var lastToken = _parser.Consume(TokenType.RBrace, "Ожидалось '}'");
        return new BlockStatement(statements, lastToken.Line);
    }

    public StatementNode ParseStatement()
    {
        foreach (var parser in _statementParsers)
        {
            if (parser.CanParse())
            {
                return parser.Parse();
            }
        }

        throw _parser.UnexpectedToken(_parser.Current);
    }
}
