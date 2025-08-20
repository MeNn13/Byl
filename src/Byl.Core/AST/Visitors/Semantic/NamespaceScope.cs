using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic;

public class NamespaceScope
{
    public string Name { get; }
    private readonly Dictionary<string, FunctionDeclaration> _functions = new();
    private readonly Dictionary<string, VariableSymbol> _variables = new();

    public NamespaceScope(string name)
    {
        Name = name;
    }

    public void AddFunction(FunctionDeclaration func) => _functions[func.Name] = func;
    public bool TryGetFunction(string name, out FunctionDeclaration func) => _functions.TryGetValue(name, out func);

    public void AddVariable(VariableSymbol var) => _variables[var.Name] = var;
    public VariableSymbol? GetVariable(string name) => _variables.GetValueOrDefault(name);

    public T? Lookup<T>(string name) where T : class
    {
        if (typeof(T) == typeof(FunctionDeclaration))
            return _functions.GetValueOrDefault(name) as T;

        if (typeof(T) == typeof(VariableSymbol))
            return _variables.GetValueOrDefault(name) as T;

        return null;
    }
}

