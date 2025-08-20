using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers.Declaration;
public class ImportParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public ImportDeclaration Parse()
    {
        var importToken = _parser.Advance(); // Пропускаем 'импорт'
        var namespaceName = ParseNamespaceName();
        _parser.Consume(TokenType.Semicolon, "Ожидалось ';' после импорта");

        return new ImportDeclaration(namespaceName, importToken.Line);
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
}
