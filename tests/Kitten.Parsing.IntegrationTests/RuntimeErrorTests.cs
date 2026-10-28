using Kitten.Interpreter;

namespace Kitten.Parsing.IntegrationTests;

/// <summary>
/// Проверяют ошибки времени выполнения через CLI.
/// </summary>
public class RuntimeErrorTests
{
    [Theory]
    [MemberData(nameof(GetRuntimeErrorData))]
    public void Stops_on_runtime_error(
        string source,
        string input,
        string expectedOutput,
        string expectedError)
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
            Assert.Equal(1, exitCode);
            Assert.Equal(expectedOutput, NormalizeLineEndings(output.ToString()));
            Assert.Equal(expectedError + "\n", NormalizeLineEndings(error.ToString()));
        }
        finally
        {
            File.Delete(path);
        }
    }

    public static TheoryData<string, string, string, string> GetRuntimeErrorData()
    {
        return new TheoryData<string, string, string, string>
        {
            {
                "func main() -> void { print(10 / 0); }",
                "",
                "",
                "Division by zero"
            },
            {
                "func main() -> void { print(2147483647 + 1); }",
                "",
                "",
                "Integer overflow"
            },
            {
                "func main() -> void { print(-2147483647 - 2); }",
                "",
                "",
                "Integer overflow"
            },
            {
                "func main() -> void { print(2147483647 * 2); }",
                "",
                "",
                "Integer overflow"
            },
            {
                "func main() -> void { print(sqr(2147483647)); }",
                "",
                "",
                "Integer overflow"
            },
            {
                "func main() -> void { print(readInt()); }",
                "",
                "",
                "Unexpected end of input"
            },
            {
                "func main() -> void { print(readInt()); }",
                "abc\n",
                "",
                "Invalid integer input: 'abc'"
            },
            {
                "func main() -> void { print(readInt()); }",
                "1.4\n",
                "",
                "Invalid integer input: '1.4'"
            },
            {
                "func main() -> void { print(7); print(10 / 0); print(9); }",
                "",
                "7\n",
                "Division by zero"
            },
        };
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}