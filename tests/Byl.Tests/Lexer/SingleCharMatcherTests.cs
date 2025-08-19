using Byl.Core.Lexer;
using Byl.Core.Lexer.TokenMatcher;
using System.Diagnostics;
using Xunit;

namespace Byl.Tests.Lexer;

public class SingleCharMatcherTests
{
    [Theory]
    [InlineData('+', TokenType.Plus)]
    [InlineData('-', TokenType.Minus)]
    [InlineData('(', TokenType.LParen)]
    [InlineData(')', TokenType.RParen)]
    [InlineData('{', TokenType.LBrace)]
    [InlineData('}', TokenType.RBrace)]
    [InlineData(';', TokenType.Semicolon)]
    public void Matches_SingleCharacter(char inputChar, TokenType expectedType)
    {
        // Arrange
        var ctx = new LexerContext(inputChar.ToString());
        var matcher = new SingleCharMatcher(inputChar, expectedType);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(expectedType, token.Type);
        Assert.Equal(inputChar.ToString(), token.Value);
        Assert.Equal(1, token.Line);
        Assert.Equal(1, ctx.Position); // Позиция должна увеличиться на 1
    }

    [Fact]
    public void DoesNotMatch_DifferentCharacter()
    {
        // Arrange
        var ctx = new LexerContext("x");
        var matcher = new SingleCharMatcher('+', TokenType.Plus);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.Null(token);
        Assert.Equal(0, ctx.Position); // Позиция не должна измениться
    }

    [Fact]
    public void DoesNotMatch_EmptyInput()
    {
        // Arrange
        var ctx = new LexerContext("");
        var matcher = new SingleCharMatcher('+', TokenType.Plus);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.Null(token);
        Assert.Equal(0, ctx.Position);
    }

    [Fact]
    public void Matches_AtSpecificPosition()
    {
        // Arrange
        var ctx = new LexerContext("abc + def");
        ctx.Position = 4; // Устанавливаем позицию на '+'
        var matcher = new SingleCharMatcher('+', TokenType.Plus);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(5, ctx.Position); // Позиция должна увеличиться на 1
    }

    [Fact]
    public void DoesNotMatch_AtWrongPosition()
    {
        // Arrange
        var ctx = new LexerContext("abc + def");
        ctx.Position = 2; // Устанавливаем позицию на 'c'
        var matcher = new SingleCharMatcher('+', TokenType.Plus);

        // Act
        var token = matcher.Match(ctx);

        // Assert
        Assert.Null(token);
        Assert.Equal(2, ctx.Position); // Позиция не должна измениться
    }

    [Fact]
    public void Matches_Quickly()
    {
        var ctx = new LexerContext("+");
        var matcher = new SingleCharMatcher('+', TokenType.Plus);

        var watch = Stopwatch.StartNew();
        for (int i = 0; i < 100000; i++)
        {
            ctx.Position = 0;
            matcher.Match(ctx);
        }
        watch.Stop();

        Assert.True(watch.ElapsedMilliseconds < 100);
    }

    [Fact]
    public void Matches_UnicodeCharacters()
    {
        var ctx = new LexerContext("π");
        var matcher = new SingleCharMatcher('π', TokenType.Special);
        var token = matcher.Match(ctx);
        Assert.NotNull(token);
    }
}
