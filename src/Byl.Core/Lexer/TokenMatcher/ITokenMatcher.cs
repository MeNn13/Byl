namespace Byl.Core.Lexer.TokenMatcher;

public interface ITokenMatcher
{
    int Priority { get; }
    Token? Match(LexerContext ctx);
}
