using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;
using Byl.Core.Parser.Parsers.Statement;

namespace Byl.Core.Parser.Parsers;

public class FunctionParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public FunctionDeclaration Parse()
    {
        // Парсим тип возвращаемого значения (если есть)
        TypeNode? returnType = null;
        if (_parser.IsTypeToken(_parser.Current.Type))
        {
            returnType = ParseType();
        }

        // Определяем тип функции (main или обычная)
        Token nameToken;
        if (_parser.Match(TokenType.Main))
        {
            nameToken = _parser.Advance();
        }
        else if (_parser.Match(TokenType.Function))
        {
            _parser.Advance(); // Пропускаем 'function'
            nameToken = _parser.Consume(TokenType.Identifier, "Ожидалось имя функции");
        }
        else
        {
            throw _parser.UnexpectedToken(_parser.Current);
        }

        _parser.Consume(TokenType.LParen, "Ожидалось '('");

        var parameters = new List<ParameterNode>();
        while (!_parser.Match(TokenType.RParen))
        {
            parameters.Add(ParseParameter());
            if (!_parser.Match(TokenType.RParen))
            {
                _parser.Consume(TokenType.Comma, "Ожидалось ',' между параметрами");
            }
        }

        _parser.Consume(TokenType.RParen, "Ожидалось ')'");
        var body = new StatementParser(_parser).Parse();
        return new FunctionDeclaration(nameToken.Value,
            parameters,
            body,
            returnType,
            _parser.LastToken.Line);
    }

    private ParameterNode ParseParameter()
    {
        // Парсим тип параметра
        var type = ParseType();

        var name = _parser.Consume(TokenType.Identifier, "Ожидалось имя параметра");

        return new ParameterNode(name.Value, type, name.Line);
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
