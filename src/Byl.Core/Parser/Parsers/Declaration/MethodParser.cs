using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;
using Byl.Core.Parser.Parsers.Statement;

namespace Byl.Core.Parser.Parsers.Declaration;

public class MethodParser
{
    private readonly Parser _parser;
    private readonly AccessModifierParser _modifierParser;
    private readonly StaticParser _staticParser;
    private readonly TypeParser _typeParser;
    private readonly ParameterParser _parameterParser;

    public MethodParser(Parser parser)
    {
        _parser = parser;
        _staticParser = new StaticParser(parser);
        _modifierParser = new AccessModifierParser(parser);
        _typeParser = new TypeParser(parser);
        _parameterParser = new ParameterParser(parser);
    }

    public MethodDeclaration Parse()
    {
        var accessModifier = _modifierParser.Parse();

        var staticModifier = _staticParser.Parse();

        TypeNode? returnType = _typeParser.Parse();

        var name = NameParse();

        var parameters = _parameterParser.Parse();

        var body = new StatementParser(_parser).Parse();

        return new MethodDeclaration(
            accessModifier,
            staticModifier,
            returnType: returnType,
            name,
            parameters,
            body: body,
            line: _parser.LastToken.Line);
    }

    private string NameParse()
    {
        string methodName;
        if (_parser.Match(TokenType.Identifier))
        {
            methodName = _parser.Consume(TokenType.Identifier, "Ожидалось имя метода").Value;
        }
        else
        {
            throw _parser.UnexpectedToken(_parser.Current);
        }

        return methodName;
    }
}
