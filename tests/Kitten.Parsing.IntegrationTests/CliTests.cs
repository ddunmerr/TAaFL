using Kitten.Interpreter;

namespace Kitten.Parsing.IntegrationTests;

public class CliTests
{
    [Fact]
    public void No_arguments_returns_error()
    {
        // Arrange
        using StringReader input = new("");
        using StringWriter output = new();
        using StringWriter error = new();

        // Act
        int exitCode = Program.Run([], input, output, error);

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal("", output.ToString());
        Assert.Equal(
            "Usage: Kitten.Interpreter <path-to-file>" + "\n",
            NormalizeLineEndings(error.ToString()));
    }

    [Fact]
    public void Missing_file_returns_error()
    {
        // Arrange
        using StringReader input = new("");
        using StringWriter output = new();
        using StringWriter error = new();

        // Act
        int exitCode = Program.Run(["notexisting.kit"], input, output, error);

        // Assert
        Assert.Equal(1, exitCode);
        Assert.Equal("", output.ToString());
        Assert.Equal(
            "File not found: notexisting.kit" + "\n",
            NormalizeLineEndings(error.ToString()));
    }

    [Fact]
    public void Valid_program_returns_success()
    {
        // Arrange
        string path = Path.GetTempFileName();
        File.WriteAllText(path, "func main() -> void { print(readInt()); }");
        using StringReader input = new("5");
        using StringWriter output = new();
        using StringWriter error = new();

        try
        {
            // Act
            int exitCode = Program.Run([path], input, output, error);

            // Assert
            Assert.Equal(0, exitCode);
            Assert.Equal("5\n", NormalizeLineEndings(output.ToString()));
            Assert.Equal("", error.ToString());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Program_with_parse_error_returns_error()
    {
        // Arrange
        string path = Path.GetTempFileName();
        File.WriteAllText(path, "func main() -> void { print(1 + ); }");
        using StringReader input = new("");
        using StringWriter output = new();
        using StringWriter error = new();

        try
        {
            // Act
            int exitCode = Program.Run([path], input, output, error);

            // Assert
            Assert.Equal(1, exitCode);
            Assert.Equal("", output.ToString());
            Assert.Equal(
                "Expected expression, got CloseParenthesis" + "\n",
                NormalizeLineEndings(error.ToString()));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Program_with_runtime_error_returns_error()
    {
        // Arrange
        string path = Path.GetTempFileName();
        File.WriteAllText(path, "func main() -> void { print(10 / 0); }");
        using StringReader input = new("");
        using StringWriter output = new();
        using StringWriter error = new();

        try
        {
            // Act
            int exitCode = Program.Run([path], input, output, error);

            // Assert
            Assert.Equal(1, exitCode);
            Assert.Equal("", output.ToString());
            Assert.Equal(
                "Division by zero" + "\n",
                NormalizeLineEndings(error.ToString()));
        }
        finally
        {
            File.Delete(path);
        }
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n");
    }
}