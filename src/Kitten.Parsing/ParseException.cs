using Kitten.Lexemes;

namespace Kitten.Parsing;

#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.

/// <summary>
/// Ошибка разбора или выполнения программы на Kitten.
/// Хранит сообщение и токен, на котором произошла ошибка.
/// </summary>
public class ParseException : Exception
{
    public ParseException(string message, Token token)
        : base(message)
    {
        Token = token;
    }

    /// <summary>
    /// Токен, на котором произошла ошибка.
    /// </summary>
    public Token Token { get; }
}

#pragma warning restore RCS1194