namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class VariableSymbol : Symbol
{
    public string Type { get; }

    public VariableSymbol(string name, string type, int line)
        : base(name, "переменная", line) // Добавляем kind
    {
        Type = type;
    }
}
