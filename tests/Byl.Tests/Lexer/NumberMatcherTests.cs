using Byl.Core.Lexer;
using Byl.Core.Lexer.TokenMatcher;
using Byl.Core.Lexer.TokenMatcher.Number;
using Xunit;

namespace Byl.Tests.Lexer;

public class NumberMatcherTests
{
    private readonly NumberMatcher _matcher = new();

    [Theory]
    // Целые числа
    [InlineData("0", "0")]
    [InlineData("42", "42")]
    [InlineData("1000000", "1000000")]
    [InlineData("1_000_000", "1000000")]
    // Дробные числа
    [InlineData("0.0", "0.0")]
    [InlineData("3.14", "3.14")]
    [InlineData("0.123_456", "0.123456")]
    [InlineData(".5", "0.5")] // Добавляем нормализацию для .5
    public void CorrectlyParses_ValidNumbers(string input, string expectedNormalized)
    {
        var ctx = new LexerContext(input);
        var token = _matcher.Match(ctx);

        Assert.NotNull(token);
        Assert.Equal(TokenType.Number, token.Type);
        Assert.Equal(expectedNormalized, token.Value); // Проверяем нормализованное значение
        Assert.Equal(1, token.Line);
        Assert.Equal(input.Length, ctx.Position);
    }

    [Theory]
    // Неправильные форматы
    [InlineData("3.14.15", "3.14")]
    [InlineData("1e5e10", "1e5")]
    [InlineData("1.2.3", "1.2")]
    // Частичные числа
    [InlineData("42abc", "42")]
    [InlineData("1.2e", "1.2")]
    [InlineData("1.2e+", "1.2")]
    public void Parses_PartialNumbers(string input, string expected)
    {
        var ctx = new LexerContext(input);
        var token = _matcher.Match(ctx);

        Assert.NotNull(token);
        Assert.Equal(expected, token.Value);
    }

    [Theory]
    [InlineData("1e")]
    [InlineData("1.2e+")]
    [InlineData("1.2e-")]
    public void DoesNotThrow_OnIncompleteExponent(string input)
    {
        var ctx = new LexerContext(input);
        var exception = Record.Exception(() => _matcher.Match(ctx));
        Assert.Null(exception); // Убеждаемся что исключения нет
    }
}
