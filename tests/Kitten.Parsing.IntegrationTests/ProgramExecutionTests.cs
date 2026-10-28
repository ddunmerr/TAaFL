namespace Kitten.Parsing.IntegrationTests;

/// <summary>
/// Проверяют правильность выполнения
/// </summary>
public class ProgramExecutionTests
{
    [Theory]
    [InlineData("func main() -> void { }", "", "")]
    [InlineData("func main() -> /* коммент */ void { }", "", "")]
    [InlineData("func main() -> void { } /* коммент */", "", "")]
    [InlineData("func main() -> void { print(5); }", "", "5\n")]
    [InlineData("func main() -> void { print(10 + 2 * 5); }", "", "20\n")]
    [InlineData("func main() -> void { print((10 + 2) * 5); }", "", "60\n")]
    [InlineData("func main() -> void { print(10 - 3 - 2); }", "", "5\n")]
    [InlineData("func main() -> void { print(-5); }", "", "-5\n")]
    [InlineData("func main() -> void { print(-(2 + 3)); }", "", "-5\n")]
    [InlineData("func main() -> void { print(--5); }", "", "5\n")]
    [InlineData("func main() -> void { print(---5); }", "", "-5\n")]
    [InlineData("func main() -> void { print(20 / 4); }", "", "5\n")]
    [InlineData("func main() -> void { print(1); print(2); }", "", "1\n2\n")]
    [InlineData("func main() -> void { 2 + 3; print(6); }", "", "6\n")]
    [InlineData("func main() -> void { print(readInt() + 1); }", "7\n", "8\n")]
    [InlineData("func main() -> void { print(min(3, 7)); }", "", "3\n")]
    [InlineData("func main() -> void { print(max(3, 7)); }", "", "7\n")]
    [InlineData("func main() -> void { print(sqr(5)); }", "", "25\n")]
    [InlineData("func main() -> void { print(sqr(-5)); }", "", "25\n")]
    [InlineData("func main() -> void { print(min(2, 3) + max(4, 5) * sqr(2)); }", "", "22\n")]
    [InlineData("func main() -> void { print(10 + 7 * (2 * 3 + 4) + 5 + -5); }", "", "80\n")]

    public void Runs_valid_program(string source, string input, string expectedOutput)
    {
        // Arrange
        using StringReader reader = new(input);
        using StringWriter writer = new();
        Parser parser = new(source, reader, writer);

        // Act
        parser.ParseProgram();

        // Assert
        Assert.Equal(expectedOutput, NormalizeLineEndings(writer.ToString()));
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}