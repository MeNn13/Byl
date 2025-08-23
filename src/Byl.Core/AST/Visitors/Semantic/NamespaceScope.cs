using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic;

public class NamespaceScope
{
    public string Name { get; }
    private readonly Dictionary<string, MethodDeclaration> _methods = [];
    private readonly Dictionary<string, ClassDeclaration> _classes = [];

    public NamespaceScope(string name) => Name = name;

    public void AddMethod(MethodDeclaration func) => _methods[func.Name] = func;
    public void AddClass(ClassDeclaration @class) => _classes[@class.Name] = @class;

    public bool TryGetFunction(string name, out MethodDeclaration func) => 
        _methods.TryGetValue(name, out func);
    public bool TryGetClass(string name, out ClassDeclaration @class) =>
        _classes.TryGetValue(name, out @class);

    public T? Lookup<T>(string name) where T : class
    {
        if (typeof(T) == typeof(MethodDeclaration))
            return _methods.GetValueOrDefault(name) as T;

        if (typeof(T) == typeof(ClassDeclaration))
            return _classes.GetValueOrDefault(name) as T;

        return null;
    }
}

