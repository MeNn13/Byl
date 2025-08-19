using Byl.Core.Lexer;
using Byl.Core.Lexer.TokenMatcher;
using Xunit;

namespace Byl.Tests.Lexer;

public class MultiCharMatcherTests
{
    [Theory]
    [InlineData("==", "==", TokenType.Equal)]
    [InlineData("=>", "=>", TokenType.Arrow)]
    [InlineData(":=", ":=", TokenType.Declare)]
    public void Matches_ValidMultiCharOperators(string input, string expectedValue, TokenType expectedType)
    {
        // Arrange
        var ctx = new LexerContext(input);
        var matcher = new MultiCharMatcher(expectedValue, expectedType);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(expectedValue, token.Value);
        Assert.Equal(1, token.Line);
        Assert.Equal(expectedValue.Length, ctx.Position); // Проверяем что позиция сдвинулась
    }

    [Fact]
    public void DoesNotMatch_PartialMatch()
    {
        // Arrange
        var ctx = new LexerContext("=x");
        var matcher = new MultiCharMatcher("==", TokenType.Equal);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.Null(token);
        Assert.Equal(0, ctx.Position); // Позиция не должна измениться
    }

    [Fact]
    public void DoesNotMatch_WhenInputTooShort()
    {
        // Arrange
        var ctx = new LexerContext("=");
        var matcher = new MultiCharMatcher("==", TokenType.Equal);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.Null(token);
        Assert.Equal(0, ctx.Position);
    }

    [Fact]
    public void Matches_AtMiddleOfInput()
    {
        // Arrange
        var ctx = new LexerContext("abc == def");
        ctx.Position = 4; // Перемещаемся к "=="
        var matcher = new MultiCharMatcher("==", TokenType.Equal);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(6, ctx.Position); // 4 + 2 символа
    }

    [Fact]
    public void DoesNotMatch_WhenNotAtStart()
    {
        // Arrange
        var ctx = new LexerContext("x == y");
        var matcher = new MultiCharMatcher("==", TokenType.Equal);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.Null(token);
        Assert.Equal(0, ctx.Position);
    }

    [Theory]
    [InlineData("!=", "!=", TokenType.NotEqual)]
    [InlineData("//", "//", TokenType.Comment)]
    public void Matches_CustomOperators(string pattern, string input, TokenType type)
    {
        // Arrange
        var ctx = new LexerContext(input);
        var matcher = new MultiCharMatcher(pattern, type);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(type, token.Type);
        Assert.Equal(pattern, token.Value);
    }
}
