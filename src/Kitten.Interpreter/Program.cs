using Kitten.Parsing;

namespace Kitten.Interpreter;

/// <summary>
/// Консольный интерпретатор языка Kitten.
/// Читает исходный код из файла, выполняет его и выводит результат в стандартный поток вывода.
/// </summary>
public static class Program
{
    /// <summary>
    /// Точка входа консольного интерпретатора.
    /// Ожидает один аргумент — путь к исходному файлу.
    /// </summary>
    public static int Main(string[] args) => Run(args, Console.In, Console.Out, Console.Error);

    public static int Run(string[] args, TextReader input, TextWriter output, TextWriter error)
    {
        if (args.Length != 1)
        {
            error.WriteLine("Usage: Kitten.Interpreter <path-to-file>");
            return 1;
        }

        string path = args[0];

        if (!File.Exists(path))
        {
            error.WriteLine($"File not found: {path}");
            return 1;
        }

        try
        {
            string code = File.ReadAllText(path);
            Parser parser = new(code, input, output);
            parser.ParseProgram();
            return 0;
        }
        catch (ParseException e)
        {
            error.WriteLine(e.Message);
            return 1;
        }
    }
}