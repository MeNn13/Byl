using System.Text;
using Byl.Core.Lexer.Extensions;

namespace Byl.Core.Lexer.TokenMatcher;

public class StringMatcher : ITokenMatcher
{
    public int Priority => 20;

    public Token? Match(LexerContext ctx)
    {
        if (ctx.Current != '"')
            return null;

        int startPos = ctx.Position;
        int startLine = ctx.Line;
        ctx.Position++;

        var sb = new StringBuilder();
        while (ctx.Position < ctx.Code.Length && ctx.Current != '"')
        {
            if (ctx.Current == '\\')
            {
                ctx.Position++;
                if (ctx.Position >= ctx.Code.Length)
                    break;

                ProcessEscapeSequence(ctx, sb);
            }
            else
            {
                if (ctx.Current == '\n')
                    ctx.Line++;

                sb.Append(ctx.Current);
                ctx.Position++;
            }
        }

        if (ctx.Position >= ctx.Code.Length || ctx.Current != '"')
            throw new LexerException("Незакрытая строка", startPos, startLine);

        ctx.Position++;
        return new Token(TokenType.StringLiteral, sb.ToString(), startLine);
    }

    private void ProcessEscapeSequence(LexerContext ctx, StringBuilder sb)
    {
        if (ctx.Position >= ctx.Code.Length)
            throw new LexerException("Незавершённая escape-последовательность", ctx.Position - 1, ctx.Line);

        char escapedChar = ctx.Current switch
        {
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            '"' => '"',
            '\\' => '\\',
            _ => throw new LexerException(
                $"Неизвестная escape-последовательность \\{ctx.Current}",
                ctx.Position - 1,
                ctx.Line)
        };

        sb.Append(escapedChar);
        ctx.Position++;
    }
}
