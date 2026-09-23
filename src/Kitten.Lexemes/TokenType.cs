namespace Kitten.Lexemes;

/// <summary>
/// Тип лексемы.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Ключевое слово if.
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово else.
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово for.
    /// </summary>
    For,

    /// <summary>
    /// Ключевое слово func.
    /// </summary>
    Func,

    /// <summary>
    /// Ключевое слово var.
    /// </summary>
    Var,

    /// <summary>
    /// Ключевое слово while.
    /// </summary>
    While,

    /// <summary>
    /// Идентификатор.
    /// </summary>
    Identifier,

    /// <summary>
    /// Литерал целого числа.
    /// </summary>
    IntLiteral,

    /// <summary>
    /// Литерал строки.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Оператор сложения "+".
    /// </summary>
    Plus,

    /// <summary>
    /// Оператор вычитания "-".
    /// </summary>
    Minus,

    /// <summary>
    /// Оператор умножения "*".
    /// </summary>
    Multiply,

    /// <summary>
    /// Оператор деления "/".
    /// </summary>
    Divide,

    /// <summary>
    /// Оператор НЕ "!".
    /// </summary>
    Not,

    /// <summary>
    /// Оператор сравнения "равно".
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения "не равно".
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Оператор "и".
    /// </summary>
    And,

    /// <summary>
    /// Оператор "или".
    /// </summary>
    Or,

    /// <summary>
    /// Оператор присваивания "=".
    /// </summary>
    Assign,

    /// <summary>
    /// Точка.
    /// </summary>
    Dot,

    /// <summary>
    /// Запятая.
    /// </summary>
    Comma,

    /// <summary>
    /// Двоеточие.
    /// </summary>
    Colon,

    /// <summary>
    /// Точка с запятой.
    /// </summary>
    Semicolon,

    /// <summary>
    /// Открывающая круглая скобка.
    /// </summary>
    OpenParenthesis,

    /// <summary>
    /// Закрывающая круглая скобка.
    /// </summary>
    CloseParenthesis,

    /// <summary>
    /// Открывающая квадратная скобка.
    /// </summary>
    OpenBracket,

    /// <summary>
    /// Закрывающая квадратная скобка.
    /// </summary>
    CloseBracket,

    /// <summary>
    /// Открывающая фигурная скобка.
    /// </summary>
    OpenBrace,

    /// <summary>
    /// Закрывающая фигурная скобка.
    /// </summary>
    CloseBrace,

    /// <summary>
    /// Недопустимая лексема.
    /// </summary>
    Error,

    /// <summary>
    /// Конец потока токенов.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Оператор "->" в объявлении функции
    /// </summary>
    Arrow,

    /// <summary>
    /// Ключевое слово int.
    /// </summary>
    Int,

    /// <summary>
    /// Ключевое слово bool.
    /// </summary>
    Bool,

    /// <summary>
    /// Ключевое слово true.
    /// </summary>
    True,

    /// <summary>
    /// Ключевое слово false.
    /// </summary>
    False,

    /// <summary>
    /// Ключевое слово string.
    /// </summary>
    String,

    /// <summary>
    /// Ключевое слово void.
    /// </summary>
    Void,

    /// <summary>
    /// Ключевое слово struct.
    /// </summary>
    Struct,

    /// <summary>
    /// Ключевое слово return.
    /// </summary>
    Return,
}