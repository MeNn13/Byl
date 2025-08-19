namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public abstract class Symbol
{
    public string Name { get; }
    public int Line { get; } // Для ошибок

    protected Symbol(string name, int line)
    {
        Name = name;
        Line = line;
    }
}
