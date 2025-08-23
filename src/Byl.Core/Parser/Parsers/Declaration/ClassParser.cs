using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;
using Byl.Core.Parser.Utils;

namespace Byl.Core.Parser.Parsers.Declaration;

public class ClassParser
{
    private readonly Parser _parser;
    private readonly AccessModifierParser _modifierParser;
    private readonly FieldParser _fieldParser;
    private readonly MethodParser _methodParser;
    private readonly ConstructorParser _constructorParser;

    public ClassParser(Parser parser)
    {
        _parser = parser;
        _modifierParser = new AccessModifierParser(parser);
        _fieldParser = new FieldParser(parser);
        _methodParser = new MethodParser(parser);
        _constructorParser = new ConstructorParser(parser);
    }

    public ClassDeclaration Parse()
    {
        string accessModifier = _modifierParser.Parse();

        var classToken = _parser.Consume(TokenType.Class, "Ожидалось 'класс'");

        var className = _parser.Consume(TokenType.Identifier, "Ожидалось имя класса").Value;

        string? baseClass = InheritanceParse();

        _parser.Consume(TokenType.LBrace, "Ожидалось '{'");

        var fields = new List<FieldDeclaration>();
        var methods = new List<MethodDeclaration>();
        ConstructorDeclaration? constructor = null;

        while (!_parser.Match(TokenType.RBrace) && !_parser.IsAtEnd)
        {
            var member = ParseClassMember();

            switch (member)
            {
                case FieldDeclaration field:
                    fields.Add(field);
                    break;
                case MethodDeclaration method:
                    methods.Add(method);
                    break;
                case ConstructorDeclaration constr:
                    if (constructor is not null)
                        throw new ParserException("Класс может иметь только один конструктор", _parser.Current.Line);
                    constructor = constr;
                    break;
            }
        }

        _parser.Consume(TokenType.RBrace, "Ожидалось '}'");

        return new ClassDeclaration(
            accessModifier,
            className,
            baseClass,
            fields,
            constructor,
            methods,
            classToken.Line);
    }

    private string? InheritanceParse()
    {
        if (_parser.Match(TokenType.Colon))
        {
            _parser.Advance(); // Пропускаем ':'
            return _parser.Consume(TokenType.Identifier, "Ожидалось имя родительского класса").Value;
        }

        return null;
    }
    private DeclarationNode ParseClassMember()
    {
        // Определяем тип члена класса
        if (_parser.Match(TokenType.Constructor))
        {
            return _constructorParser.Parse();
        }
        else if (_parser.IsTypeToken(_parser.Current.Type))
        {
            // Проверяем, это поле или метод
            var peek1 = _parser.Peek();
            var peek2 = _parser.Peek(2);

            if (peek1 is not null && peek1.Type is TokenType.Identifier
                && peek2 is not null && peek2.Type is TokenType.LParen)
            {
                return _methodParser.Parse();
            }
            else
            {
                return _fieldParser.Parse();
            }
        }

        throw _parser.UnexpectedToken(_parser.Current);
    }
}
