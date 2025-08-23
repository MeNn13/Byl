using Byl.Core.Lexer;

namespace Byl.Core.Parser.Parsers;

public class AccessModifierParser(Parser parser)
{
    private readonly Parser _parser = parser;

    public string Parse(string accessModifier = "приватный")
    {
        if (IsAccessModifier(_parser.Current.Type))
        {
            accessModifier = _parser.Advance().Value;
        }

        return accessModifier;
    }

    private bool IsAccessModifier(TokenType type) => type switch
    {
        TokenType.Public or 
        TokenType.Private or 
        TokenType.Protected => true,
        _ => false
    };
}
