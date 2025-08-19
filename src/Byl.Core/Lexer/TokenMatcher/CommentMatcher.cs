using Byl.Core.Lexer.Extensions;

namespace Byl.Core.Lexer.TokenMatcher;

public class CommentMatcher : ITokenMatcher
{
    public int Priority => 10;

    public Token? Match(LexerContext ctx)
    {
        var singleComment = SingleComment(ctx);
        if (singleComment is not null) return singleComment;

        var multiComment = MultiComment(ctx);
        if (multiComment is not null) return multiComment;

        return null;
    }

    private Token? SingleComment(LexerContext ctx)
    {
        if (ctx.Current == '/' && ctx.Peek() == '/')
        {
            int start = ctx.Position;
            ctx.Position += 2; // Пропускаем "//"

            // Пропускаем всё до конца строки
            while (ctx.Position < ctx.Code.Length && ctx.Current != '\n')
            {
                ctx.Position++;
            }

            return new Token(TokenType.Comment, ctx.Code[start..ctx.Position], ctx.Line);
        }

        return null;
    }

    private Token? MultiComment(LexerContext ctx)
    {
        if (ctx.Current == '/' && ctx.Peek() == '*')
        {
            int start = ctx.Position;
            int startLine = ctx.Line;
            ctx.Position += 2; // Пропускаем "/*"

            while (ctx.Position < ctx.Code.Length - 1)
            {
                if (ctx.Current == '\n')
                    ctx.Line++;

                if (ctx.Current == '*' && ctx.Peek() == '/')
                {
                    ctx.Position += 2;
                    return new Token(TokenType.Comment, ctx.Code[start..ctx.Position], startLine);
                }

                ctx.Position++;
            }

            throw new LexerException("Незакрытый комментарий", start, startLine);
        }

        return null;
    }
}
