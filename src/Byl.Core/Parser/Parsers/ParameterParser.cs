using Byl.Core.AST.Nodes;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers;

internal class ParameterParser
{
    private readonly Parser _parser;
    private readonly TypeParser _typeParser;

    public ParameterParser(Parser parser)
    {
        _parser = parser;
        _typeParser = new TypeParser(parser);
    }

    public List<ParameterNode> Parse()
    {
        _parser.Consume(TokenType.LParen, "Ожидалось '('");

        var parameters = new List<ParameterNode>();

        while (!_parser.Match(TokenType.RParen))
        {
            var parameter = ParameterParse();
            parameters.Add(parameter);

            if (!_parser.Match(TokenType.RParen))
            {
                _parser.Consume(TokenType.Comma, "Ожидалось ',' между параметрами");
            }
        }

        _parser.Consume(TokenType.RParen, "Ожидалось ')'");

        return parameters;
    }

    private ParameterNode ParameterParse()
    {
        var type = _typeParser.Parse();

        var name = _parser.Consume(TokenType.Identifier, "Ожидалось имя параметра");
        return new ParameterNode(name.Value, type, name.Line);
    }
}
