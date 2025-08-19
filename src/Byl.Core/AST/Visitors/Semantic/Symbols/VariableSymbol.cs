namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class VariableSymbol : Symbol
{
    public string Type { get; } // "int", "string" и т.д.

    public VariableSymbol(string name, string type, int line)
        : base(name, line)
    {
        Type = type;
    }
}
