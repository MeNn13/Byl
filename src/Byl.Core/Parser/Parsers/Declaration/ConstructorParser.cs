using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;
using Byl.Core.Parser.Parsers.Statement;

namespace Byl.Core.Parser.Parsers.Declaration;

public class ConstructorParser
{
    private readonly Parser _parser;
    private readonly ParameterParser _parameterParser;
    private readonly AccessModifierParser _modifierParser;

    public ConstructorParser(Parser parser)
    {
        _parser = parser;
        _parameterParser = new ParameterParser(parser);
        _modifierParser = new AccessModifierParser(parser);
    }

    public ConstructorDeclaration Parse()
    {
        var accessModifier = _modifierParser.Parse();

        var constructorToken = _parser.Consume(TokenType.Constructor, "Ожидалось 'Конструктор'");

        var parameters = _parameterParser.Parse();

        var body = new StatementParser(_parser).Parse();

        return new ConstructorDeclaration(
            accessModifier,
            parameters,
            body: body,
            constructorToken.Line);
    }
}
