using System.Reflection;
using Byl.Core.Lexer.Extensions;
using Byl.Core.Lexer.TokenMatcher;
using Byl.Core.Lexer.TokenMatcher.Number;

namespace Byl.Core.Lexer;

public class Lexer
{
    private readonly List<ITokenMatcher> _matchers = [];

    public Lexer()
    {
        _matchers = InitializeMatchers();
    }

    public List<Token> Tokenize(string code)
    {
        code = code.Replace("\0", "");
        var context = new LexerContext(code);
        var tokens = new List<Token>();

        while (context.Position < context.Code.Length)
        {
            if (char.IsWhiteSpace(context.Current))
            {
                if (context.Current == '\n') context.Line++;
                context.Position++;
                continue;
            }

            bool matched = ProcessToken(context, tokens);

            if (!matched)
                ThrowUnknownCharacterError(context);
        }

        tokens.Add(new Token(TokenType.EOF, "", context.Line));
        return tokens;
    }

    private void ThrowUnknownCharacterError(LexerContext ctx)
    {
        var charDisplay = ctx.Current < 32 ? $"0x{(int)ctx.Current:X2}" : $"'{ctx.Current}'";
        throw new LexerException($"Неизвестный символ {charDisplay}", ctx.Position, ctx.Line);
    }
    private bool ProcessToken(LexerContext ctx, List<Token> tokens)
    {
        foreach (var matcher in _matchers.OrderBy(m => m.Priority))
            if (matcher.Match(ctx) is { } token)
                if (token.Type is not TokenType.Comment)
                {
                    tokens.Add(token);
                    return true;
                }

        return false;
    }
    private static List<ITokenMatcher> InitializeMatchers()
    {
        var matchers = new List<ITokenMatcher>();

        foreach (TokenType type in Enum.GetValues<TokenType>())
        {
            var field = typeof(TokenType).GetField(type.ToString());
            if (field is null) continue;

            var multiAttr = field.GetCustomAttribute<MultiCharAttribute>();
            if (multiAttr is not null)
            {
                matchers.Add(new MultiCharMatcher(multiAttr.Pattern, type));
                continue;
            }

            var charAttr = field.GetCustomAttribute<CharAttribute>();
            if (charAttr is not null)
            {
                matchers.Add(new SingleCharMatcher(type));
                continue;
            }

            var keywordAttr = field.GetCustomAttribute<KeywordAttribute>();
            if (keywordAttr is not null)
            {
                matchers.Add(new KeywordMatcher());
                continue;
            }
        }

        // Специальные матчеры
        matchers.Add(new NumberMatcher());
        matchers.Add(new InterpolatedStringMatcher());
        matchers.Add(new StringMatcher());
        matchers.Add(new CommentMatcher());

        return [.. matchers.OrderBy(m => m.Priority)];
    }
}