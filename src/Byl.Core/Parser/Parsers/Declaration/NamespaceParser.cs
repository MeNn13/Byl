using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Declaration;

public class NamespaceParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public NamespaceDeclaration Parse()
    {
        var namespaceToken = _parser.Advance(); // Пропускаем 'пространство'
        var name = ParseNamespaceName();
        _parser.Consume(TokenType.LBrace, "Ожидалось '{' после имени пространства имен");

        var members = new List<DeclarationNode>();
        while (!_parser.Match(TokenType.RBrace) && !_parser.IsAtEnd)
        {
            members.Add(ParseMember());
        }

        _parser.Consume(TokenType.RBrace, "Ожидалось '}'");
        return new NamespaceDeclaration(name, members, namespaceToken.Line);
    }

    private string ParseNamespaceName()
    {
        var names = new List<string> { _parser.Consume(TokenType.Identifier, "Ожидался идентификатор").Value };

        while (_parser.Match(TokenType.NamespaceSeparator))
        {
            _parser.Advance(); // Пропускаем '::'
            names.Add(_parser.Consume(TokenType.Identifier, "Ожидался идентификатор").Value);
        }

        return string.Join("::", names);
    }

    private DeclarationNode ParseMember()
    {
        if (_parser.Match(TokenType.Static))
        {
            return ParseStaticFunction();
        }

        if (_parser.IsTypeToken(_parser.Current.Type) || _parser.Match(TokenType.Function))
        {
            return new FunctionParser(_parser).Parse();
        }

        throw _parser.UnexpectedToken(_parser.Current);
    }

    private FunctionDeclaration ParseStaticFunction()
    {
        var staticToken = _parser.Advance(); // Пропускаем 'static'

        // Парсим остальную часть функции
        var function = new FunctionParser(_parser).Parse();

        return new FunctionDeclaration(
            function.IsStatic,
            function.ReturnType,
            function.Name,
            function.Parameters,
            function.Body,
            staticToken.Line
        );
    }
}
