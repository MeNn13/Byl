namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public abstract class Symbol
{
    public string Name { get; }
    public string Kind { get; } // "переменная", "функция", "пространство"
    public int Line { get; }

    protected Symbol(string name, string kind, int line)
    {
        Name = name;
        Kind = kind;
        Line = line;
    }

    // Добавляем конструктор без kind для обратной совместимости
    protected Symbol(string name, int line) : this(name, "неизвестно", line) { }
}
