namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class NamespaceSymbol : Symbol
{
    public Dictionary<string, Symbol> Members { get; } = new();

    public NamespaceSymbol(string name, int line) : base(name, "пространство", line) { }

    public bool TryAddSymbol(Symbol symbol) => Members.TryAdd(symbol.Name, symbol);
    public Symbol? Lookup(string name) => Members.GetValueOrDefault(name);
    public T? Lookup<T>(string name) where T : Symbol => Lookup(name) as T;
}
