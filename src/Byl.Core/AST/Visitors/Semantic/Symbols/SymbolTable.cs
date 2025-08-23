namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class SymbolTable
{
    private readonly Stack<Dictionary<string, Symbol>> _scopes = [];
    private readonly Dictionary<string, NamespaceSymbol> _namespaces = [];
    public Dictionary<string, Symbol> CurrentScope => _scopes.Peek();

    public void EnterScope() => _scopes.Push([]);
    public void ExitScope() => _scopes.Pop();
    public bool TryAddSymbol(Symbol symbol)
    {
        if (_scopes.Count is 0) return false;
        return _scopes.Peek().TryAdd(symbol.Name, symbol);
    }
    public Symbol? Lookup(string name)
    {
        foreach (var scope in _scopes)
        {
            if (scope.TryGetValue(name, out var symbol))
                return symbol;
        }
        return null;
    }
    public T? Lookup<T>(string name) where T : Symbol => Lookup(name) as T;
    public void RegisterNamespace(NamespaceSymbol ns) => _namespaces[ns.Name] = ns;
    public NamespaceSymbol? LookupNamespace(string name) => _namespaces.GetValueOrDefault(name);
}