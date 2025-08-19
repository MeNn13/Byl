using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Statement.Statements;

internal class VariableDeclarationParser(Parser parser) : IStatementParser
{
    private readonly Parser _parser = parser;

    public bool CanParse() => _parser.IsTypeToken(_parser.Current.Type);

    public StatementNode Parse()
    {
        var type = ParseType();
        var name = _parser.Consume(TokenType.Identifier, "Ожидалось имя переменной");

        ExpressionNode? initializer = null;
        if (_parser.Match(TokenType.Assign))
        {
            _parser.Advance();
            initializer = new ExpressionParser(_parser).Parse();
        }

        _parser.Consume(TokenType.Semicolon, "Ожидалось ';'");

        return new VariableDeclarationStatement(type, name.Value, initializer, name.Line);
    }

    private TypeNode ParseType()
    {
        if (!_parser.IsTypeToken(_parser.Current.Type))
        {
            throw _parser.UnexpectedToken(_parser.Current);
        }

        var typeToken = _parser.Advance();
        return new TypeNode(typeToken.Value, typeToken.Line);
    }
}