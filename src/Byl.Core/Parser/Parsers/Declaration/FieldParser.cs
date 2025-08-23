using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Declaration;

public class FieldParser
{
    private readonly Parser _parser;
    private readonly AccessModifierParser _modifierParser;

    public FieldParser(Parser parser)
    {
        _parser = parser;
        _modifierParser = new AccessModifierParser(parser);
    }

    public FieldDeclaration Parse()
    {
        var accessModifier = _modifierParser.Parse();

        var type = new TypeParser(_parser).Parse();

        var nameToken = _parser.Consume(TokenType.Identifier, "Ожидалось имя поля");

        ExpressionNode? initializer = InitializerParse();

        _parser.Consume(TokenType.Semicolon, "Ожидалось ';'");

        return new FieldDeclaration(
            accessModifier,
            type,
            nameToken.Value,
            initializer,
            _parser.Current.Line);
    }

    private ExpressionNode? InitializerParse()
    {
        if (_parser.Match(TokenType.Assign))
        {
            _parser.Advance(); // Пропускаем '='
            return new ExpressionParser(_parser).Parse();
        }

        return null;
    }
}
