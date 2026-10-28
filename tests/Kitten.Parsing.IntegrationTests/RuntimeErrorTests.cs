namespace Kitten.Parsing.IntegrationTests;

/// <summary>
/// Проверяют выброс исключений после выполнения
/// </summary>
public class RuntimeErrorTests
{
    [Theory]
    [InlineData("func main() -> void { print(10 / 0); }", "", "Division by zero")]
    [InlineData("func main() -> void { print(2147483647  + 1); }", "", "Integer overflow")]
    [InlineData("func main() -> void { print(-2147483647 - 2); }", "", "Integer overflow")]
    [InlineData("func main() -> void { print(readInt()); }", "", "Unexpected end of input")]
    [InlineData("func main() -> void { print(readInt()); }", "abc\n", "Invalid integer input")]
    [InlineData("func main() -> void { print(readInt()); }", "1.4\n", "Invalid integer input")]
    public void Stops_on_runtime_error(string source, string input, string expectedMessagePart)
    {
        // Arrange
        using StringReader reader = new(input);
        using StringWriter writer = new();
        Parser parser = new(source, reader, writer);

        // Act & Assert
        ParseException exception = Assert.Throws<ParseException>(() => parser.ParseProgram());
        Assert.Contains(expectedMessagePart, exception.Message);
    }

    [Fact]
    public void Keeps_output_before_runtime_error()
    {
        // Arrange
        using StringReader reader = new("");
        using StringWriter writer = new();
        Parser parser = new("func main() -> void { print(7); print(10 / 0); print(9); }", reader, writer);

        // Act
        Assert.Throws<ParseException>(() => parser.ParseProgram());

        // Assert
        Assert.Equal("7\n", NormalizeLineEndings(writer.ToString()));
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}