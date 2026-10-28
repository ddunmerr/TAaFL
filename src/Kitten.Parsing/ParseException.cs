using Kitten.Lexemes;

namespace Kitten.Parsing;

#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.

public class ParseException : Exception
{
    public ParseException(string message, Token token)
        : base($"{message} at {token}")
    {
    }
}

#pragma warning restore RCS1194