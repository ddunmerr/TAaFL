using Kitten.Interpreter;

namespace Kitten.Parsing.IntegrationTests;

/// <summary>
/// Проверяют успешное выполнение программы через CLI.
/// </summary>
public class ProgramExecutionTests
{
    [Theory]
    [MemberData(nameof(GetExecutionData))]
    public void Runs_valid_program(string source, string input, string expectedOutput)
    {
        // Arrange
        string path = Path.GetTempFileName();
        File.WriteAllText(path, source);
        using StringReader reader = new(input);
        using StringWriter output = new();
        using StringWriter error = new();

        try
        {
            // Act
            int exitCode = Program.Run([path], reader, output, error);

            // Assert
            Assert.Equal(0, exitCode);
            Assert.Equal(expectedOutput, NormalizeLineEndings(output.ToString()));
            Assert.Equal("", error.ToString());
        }
        finally
        {
            File.Delete(path);
        }
    }

    public static TheoryData<string, string, string> GetExecutionData()
    {
        return new TheoryData<string, string, string>
        {
            { "func main() -> void { }", "", "" },
            { "func main() -> /* коммент */ void { }", "", "" },
            { "func main() -> void { } /* коммент */", "", "" },
            { "func main() -> void { print(5); }", "", "5\n" },
            { "func main() -> void { print(10 + 2 * 5); }", "", "20\n" },
            { "func main() -> void { print((10 + 2) * 5); }", "", "60\n" },
            { "func main() -> void { print(10 - 3 - 2); }", "", "5\n" },
            { "func main() -> void { print(-5); }", "", "-5\n" },
            { "func main() -> void { print(-(2 + 3)); }", "", "-5\n" },
            { "func main() -> void { print(--5); }", "", "5\n" },
            { "func main() -> void { print(---5); }", "", "-5\n" },
            { "func main() -> void { print(20 / 4); }", "", "5\n" },
            { "func main() -> void { print(1); print(2); }", "", "1\n2\n" },
            { "func main() -> void { 2 + 3; print(6); }", "", "6\n" },
            { "func main() -> void { print(readInt() + 1); }", "7\n", "8\n" },
            { "func main() -> void { print(min(3, 7)); }", "", "3\n" },
            { "func main() -> void { print(max(3, 7)); }", "", "7\n" },
            { "func main() -> void { print(sqr(5)); }", "", "25\n" },
            { "func main() -> void { print(sqr(-5)); }", "", "25\n" },
            { "func main() -> void { print(min(2, 3) + max(4, 5) * sqr(2)); }", "", "22\n" },
            { "func main() -> void { print(10 + 7 * (2 * 3 + 4) + 5 + -5); }", "", "80\n" },
            { "func main() -> void { print(10 * -2); }", "", "-20\n" },
            { "func main() -> void { print(min(min(1, 2), 3)); }", "", "1\n" },
            { "func main() -> void { print(sqr(sqr(2))); }", "", "16\n" },
        };
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}