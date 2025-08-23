using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic.Visitors;

internal class ProgramSemantic : BaseSemanticVisitor
{
    private readonly SymbolTable _symbols = new();
    private readonly Dictionary<string, NamespaceScope> _namespaces = [];
    private readonly List<string> _imports = [];
    private readonly ISemanticVisitor _methodVisitor;
    private readonly ISemanticVisitor _expressionVisitor;
    private readonly ISemanticVisitor _classVisitor;

    public ProgramSemantic()
    {
        _methodVisitor = new MethodSemantic(_symbols, _namespaces, _imports);
        _expressionVisitor = new ExpressionSemantic(_symbols, _namespaces, _imports);
        _classVisitor = new ClassSemantic(_symbols, _namespaces, _imports);
    }

    public override SemanticResult Visit(ProgramNode node)
    {
        _symbols.EnterScope();

        // Собираем импорты и пространства имен
        foreach (var declaration in node.Declarations)
        {
            var result = declaration.Accept(this);
            if (!result.IsValid) return result;
        }

        // Анализируем все объявления
        foreach (var declaration in node.Declarations)
        {
            SemanticResult result;

            if (declaration is MethodDeclaration || declaration is NamespaceDeclaration)
                result = declaration.Accept(this);
            else
                result = declaration.Accept(_expressionVisitor);

            if (!result.IsValid) return result;
        }

        if (FindFunction("главный") is null)
            return SemanticResult.Error("Не найдена функция 'главный'");

        _symbols.ExitScope();
        return SemanticResult.Success();
    }
    public override SemanticResult Visit(ImportDeclaration node)
    {
        _imports.Add(node.Namespace);
        return SemanticResult.Success();
    }
    public override SemanticResult Visit(NamespaceDeclaration node)
    {
        if (!_namespaces.ContainsKey(node.Name))
            _namespaces[node.Name] = new NamespaceScope(node.Name);

        var scope = _namespaces[node.Name];
        foreach (var member in node.Members.OfType<MethodDeclaration>())
            scope.AddMethod(member);

        // Анализируем содержимое namespace
        foreach (var member in node.Members)
        {
            var result = member.Accept(this);
            if (!result.IsValid) return result;
        }

        return SemanticResult.Success();
    }
    public override SemanticResult Visit(MethodDeclaration node)
    {
        return _methodVisitor.Visit(node);
    }
    public override SemanticResult Visit(ClassDeclaration node)
    {
        return _classVisitor.Visit(node);
    }

    private MethodDeclaration? FindFunction(string name)
    {
        foreach (var ns in _namespaces.Values)
        {
            if (ns.TryGetFunction(name, out var func))
                return func;
        }

        var globalFunc = _symbols.Lookup<MethodSymbol>(name);
        return globalFunc?.Declaration;
    }
}
