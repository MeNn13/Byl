using Byl.Core.Lexer.Extensions;
using System.Globalization;

namespace Byl.Core.Lexer.TokenMatcher.Number;

public class NumberMatcher : ITokenMatcher
{
    public int Priority => 20;

    public Token? Match(LexerContext ctx)
    {
        if (!IsDigit(ctx.Current) && ctx.Current != '.')
            return null;

        int start = ctx.Position;
        int line = ctx.Line;
        bool hasDot = false;
        bool hasExponent = false;
        int exponentStart = -1;

        // Целая часть
        ConsumeDigits(ctx);

        // Дробная часть
        if (ctx.Current == '.' && IsDigit(ctx.Peek()))
        {
            hasDot = true;
            ctx.Position++;
            ConsumeDigits(ctx);
        }

        // Экспонента
        if (ctx.Current is 'e' or 'E' && IsDigit(ctx.Peek()))
        {
            hasExponent = true;
            exponentStart = ctx.Position;
            ctx.Position++; // Пропускаем 'e'/'E'

            // Знак экспоненты (если есть)
            if (ctx.Current is '+' or '-')
                ctx.Position++;

            // Обязательно должна быть хотя бы одна цифра
            if (!IsDigit(ctx.Current))
            {
                // Откатываемся к началу экспоненты
                ctx.Position = exponentStart;
                hasExponent = false;
            }
            else
            {
                ConsumeDigits(ctx);
            }
        }

        string originalValue = ctx.Code[start..ctx.Position];
        string normalizedValue = NormalizeNumber(originalValue);

        ValidateNumber(normalizedValue, hasDot, hasExponent, start, line);

        return new Token(TokenType.Number, normalizedValue, line);
    }

    private string NormalizeNumber(string value)
    {
        var normalized = value.Replace("_", "");

        // Нормализуем .5 в 0.5
        if (normalized.StartsWith("."))
            normalized = "0" + normalized;
        else if (normalized.StartsWith("-."))
            normalized = "-0" + normalized[1..];

        return normalized;
    }

    private void ConsumeDigits(LexerContext ctx)
    {
        while (IsDigit(ctx.Current))
        {
            ctx.Position++;
        }
    }

    private bool IsDigit(char c) => char.IsDigit(c) || c == '_';

    private void ValidateNumber(string value, bool hasDot, bool hasExponent, int pos, int line)
    {
        try
        {
            var numStr = value.Replace("_", "");
            if (hasExponent)
            {
                if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                    throw NewLexerError($"Некорректное число: {value}", pos, line);
            }
            else if (hasDot)
            {
                if (!double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                    throw NewLexerError($"Некорректное дробное число: {value}", pos, line);
            }
            else
            {
                if (!long.TryParse(numStr, out _))
                    throw NewLexerError($"Некорректное целое число: {value}", pos, line);
            }
        }
        catch (Exception ex)
        {
            throw NewLexerError(ex.Message, pos, line);
        }
    }

    private LexerException NewLexerError(string msg, int pos, int line) =>
        new(msg, pos, line);
}
