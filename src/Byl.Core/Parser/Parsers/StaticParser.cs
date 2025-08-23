using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers;

public class StaticParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public bool Parse()
    {
        bool isStatic = false;
        if (_parser.Match(TokenType.Static))
        {
            isStatic = true;
            _parser.Advance(); // Пропускаем 'static'
        }

        return isStatic;
    }
}
