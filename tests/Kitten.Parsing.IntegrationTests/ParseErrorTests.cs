namespace Kitten.Parsing.IntegrationTests;

using Kitten.Parsing;

/// <summary>
/// Проверяют выброс исключений до выполнения
/// </summary>
public class ParseErrorTests
{
    [Theory]
    [InlineData("", "Expected Func")]
    [InlineData("func foo() -> void { }", "Expected 'main'")]
    [InlineData("func main() -> int { }", "Expected Void")]
    [InlineData("func main() -> void { print(1);", "Expected CloseBrace")]
    [InlineData("func main() -> void { } extra", "Expected EndOfFile")]
    [InlineData("func main() -> void { print(1) }", "Expected Semicolon")]
    [InlineData("func main() -> void { print(1 + ); }", "Expected expression")]
    [InlineData("func main() -> void { print(foo(1)); }", "Unknown function 'foo'")]
    [InlineData("func main() -> void { print(min(1)); }", "Expected Comma")]
    [InlineData("func main() -> void { print(min(1, 2, 3)); }", "Expected CloseParenthesis")]
    [InlineData("func main() -> void { print(max(1)); }", "Expected Comma")]
    [InlineData("func main() -> void { print(max(1, 2, 3)); }", "Expected CloseParenthesis")]
    [InlineData("func main() -> void { print(sqr()); }", "Expected expression")]
    [InlineData("func main() -> void { print(sqr(1, 2)); }", "Expected CloseParenthesis")]
    [InlineData("func main() -> void { print(readInt(1)); }", "Expected CloseParenthesis")]
    [InlineData("func main() -> void { print(abc); }", "Unknown function")]
    [InlineData("func main() -> void { print(\"abc\"); }", "Expected expression")]
    public void Rejects_invalid_program(string source, string expectedMessagePart)
    {
        // Arrange
        using StringReader reader = new("");
        using StringWriter writer = new();
        Parser parser = new(source, reader, writer);

        // Act & Assert
        ParseException exception = Assert.Throws<ParseException>(() => parser.ParseProgram());
        Assert.Contains(expectedMessagePart, exception.Message);
    }
}