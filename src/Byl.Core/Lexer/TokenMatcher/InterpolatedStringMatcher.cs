using System.Text;

namespace Byl.Core.Lexer.TokenMatcher;

internal class InterpolatedStringMatcher : ITokenMatcher
{
    public int Priority => 15;

    public Token? Match(LexerContext ctx)
    {
        if (ctx.Current == '%' && ctx.Peek() == '"')
        {
            return MatchInterpolatedString(ctx);
        }
        return null;
    }

    private Token MatchInterpolatedString(LexerContext context)
    {
        var startPos = context.Position;
        var line = context.Line;

        // Пропускаем %"
        context.Position += 2;

        var resultText = new StringBuilder();
        resultText.Append("%\"");

        var currentText = new StringBuilder();
        var inExpression = false;

        while (context.Position < context.Code.Length)
        {
            if (context.Current == '\\') // Escape sequences
            {
                resultText.Append(context.Current);
                context.Position++;

                if (context.Position < context.Code.Length)
                {
                    resultText.Append(context.Current);
                    context.Position++;
                }
            }
            else if (context.Current == '{' && !inExpression)
            {
                // Начало выражения - добавляем накопленный текст
                resultText.Append(currentText);
                currentText.Clear();

                resultText.Append('{');
                inExpression = true;
                context.Position++;
            }
            else if (context.Current == '}' && inExpression)
            {
                // Конец выражения
                resultText.Append(currentText);
                currentText.Clear();

                resultText.Append('}');
                inExpression = false;
                context.Position++;
            }
            else if (context.Current == '"' && !inExpression)
            {
                // Конец строки
                resultText.Append(currentText);
                resultText.Append('"');
                context.Position++;
                break;
            }
            else
            {
                currentText.Append(context.Current);
                context.Position++;
            }

            if (context.Current == '\n') context.Line++;
        }

        return new Token(TokenType.InterpolatedString, resultText.ToString(), line);
    }

    private char ProcessEscape(char c) => c switch
    {
        'n' => '\n',
        't' => '\t',
        'r' => '\r',
        '"' => '"',
        '\\' => '\\',
        _ => c
    };
}
