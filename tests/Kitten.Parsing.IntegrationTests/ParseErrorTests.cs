namespace Kitten.Parsing.IntegrationTests;

/// <summary>
/// Проверяют выброс исключений при разборе.
/// </summary>
public class ParseErrorTests
{
    [Theory]
    [MemberData(nameof(GetParseErrorData))]
    public void Rejects_invalid_program(string source, string expectedMessage)
    {
        // Arrange
        using StringReader reader = new("");
        using StringWriter writer = new();
        Parser parser = new(source, reader, writer);

        // Act
        ParseException exception = Assert.Throws<ParseException>(() => parser.ParseProgram());

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    public static TheoryData<string, string> GetParseErrorData()
    {
        return new TheoryData<string, string>
        {
            { "", "Expected Func, got EndOfFile" },
            { "func foo() -> void { }", "Expected 'main', got Identifier (foo)" },
            { "func main() -> int { }", "Expected Void, got Int" },
            { "func main() -> void { print(1);", "Expected CloseBrace, got EndOfFile" },
            { "func main() -> void { } extra", "Expected EndOfFile, got Identifier (extra)" },
            { "func main() -> void { print(1) }", "Expected Semicolon, got CloseBrace" },
            { "func main() -> void { print(1 + ); }", "Expected expression, got CloseParenthesis" },
            { "func main() -> void { print(foo(1)); }", "Unknown function 'foo'" },
            { "func main() -> void { print(min(1)); }", "Expected Comma, got CloseParenthesis" },
            { "func main() -> void { print(min(1, 2, 3)); }", "Expected CloseParenthesis, got Comma" },
            { "func main() -> void { print(max(1)); }", "Expected Comma, got CloseParenthesis" },
            { "func main() -> void { print(max(1, 2, 3)); }", "Expected CloseParenthesis, got Comma" },
            { "func main() -> void { print(sqr()); }", "Expected expression, got CloseParenthesis" },
            { "func main() -> void { print(sqr(1, 2)); }", "Expected CloseParenthesis, got Comma" },
            { "func main() -> void { print(readInt(1)); }", "Expected CloseParenthesis, got IntLiteral (1)" },
            { "func main() -> void { print(abc); }", "Unknown function 'abc'" },
            { "func main() -> void { print(\"abc\"); }", "Expected expression, got StringLiteral (abc)" },
        };
    }
}