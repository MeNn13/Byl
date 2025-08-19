using Xunit;

namespace Byl.Tests.Parser;

public class ParserTests
{
    [Fact]
    public void Parses_FunctionWithEmptyBody()
    {
        // Arrange
        var code = """главная() => {}""";

        var lexer = new Core.Lexer.Lexer();
        var parser = new Core.Parser.Parser(lexer.Tokenize(code));

        // Act
        var program = parser.ParseProgram();

        // Assert
        Assert.Single(program.Functions);
        Assert.Equal("главная", program.Functions[0].Name);
        Assert.Empty(program.Functions[0].Parameters);
        Assert.Empty(program.Functions[0].Body.Statements);
    }

    [Fact]
    public void Parses_FunctionWithParameters()
    {
        // Arrange
        var code = """главная(x, y) => {}""";

        var lexer = new Core.Lexer.Lexer();
        var parser = new Core.Parser.Parser(lexer.Tokenize(code));

        // Act
        var program = parser.ParseProgram();

        // Assert
        Assert.Equal(2, program.Functions[0].Parameters.Count);
        Assert.Equal("x", program.Functions[0].Parameters[0].Name);
        Assert.Equal("y", program.Functions[0].Parameters[1].Name);
    }
}
