namespace Byl.Core.AST.Visitors.Semantic.Symbols;

public class SymbolTable
{
    private readonly Stack<Dictionary<string, Symbol>> _scopes = new();

    public void EnterScope() => _scopes.Push([]);
    public void ExitScope() => _scopes.Pop();

    public bool TryAddSymbol(Symbol symbol)
    {
        if (_scopes.Peek().ContainsKey(symbol.Name))
            return false;

        _scopes.Peek()[symbol.Name] = symbol;
        return true;
    }

    public Symbol? Lookup(string name)
    {
        // Ищем от внутреннего scope к внешнему
        foreach (var scope in _scopes.Reverse())
        {
            if (scope.TryGetValue(name, out var symbol))
                return symbol;
        }
        return null;
    }
    public T? Lookup<T>(string name) where T : Symbol
    {
        var symbol = Lookup(name);
        return symbol as T;
    }

    public bool TryLookup<T>(string name, out T? symbol) where T : Symbol
    {
        symbol = Lookup<T>(name);
        return symbol != null;
    }
}
