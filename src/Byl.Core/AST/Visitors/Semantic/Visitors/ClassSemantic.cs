using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic.Visitors;

internal class ClassSemantic : BaseSemanticVisitor
{
    private readonly SymbolTable _symbols;
    private readonly Dictionary<string, NamespaceScope> _namespaces;
    private readonly List<string> _imports;
    private readonly ISemanticVisitor _expressionVisitor;

    public ClassSemantic(
        SymbolTable symbols,
        Dictionary<string, NamespaceScope> namespaces,
        List<string> imports)
    {
        _symbols = symbols;
        _namespaces = namespaces;
        _imports = imports;
        _expressionVisitor = new ExpressionSemantic(symbols, namespaces, imports);
    }

    public override SemanticResult Visit(ClassDeclaration node)
    {
        var classSymbol = new ClassSymbol(node.Name, node.AccessModifier, node.Line);
        if (!_symbols.TryAddSymbol(classSymbol))
            return SemanticResult.Error($"Класс '{node.Name}' уже определен");

        _symbols.EnterScope();

        // Анализируем поля
        foreach (var field in node.Fields)
        {
            var result = field.Accept(this);
            if (!result.IsValid) return result;
        }

        // Анализируем конструктор
        if (node.Constructor is not null)
        {
            var result = node.Constructor.Accept(this);
            if (!result.IsValid) return result;
        }

        // Анализируем методы
        foreach (var method in node.Methods)
        {
            var result = method.Accept(this);
            if (!result.IsValid) return result;
        }

        _symbols.ExitScope();
        return SemanticResult.Success();
    }
    public override SemanticResult Visit(FieldDeclaration node)
    {
        var fieldSymbol = new VariableSymbol(node.Name, node.Type.TypeName, node.Line);
        if (!_symbols.TryAddSymbol(fieldSymbol))
            return SemanticResult.Error($"Поле '{node.Name}' уже определено");

        return node.Initializer?.Accept(_expressionVisitor) ?? SemanticResult.Success();
    }
    public override SemanticResult Visit(ConstructorDeclaration node)
    {
        _symbols.EnterScope();

        // Добавляем параметры конструктора
        foreach (var param in node.Parameters)
        {
            var result = param.Accept(this);
            if (!result.IsValid) return result;
        }

        // Анализируем тело конструктора
        var bodyResult = node.Body.Accept(_expressionVisitor);

        _symbols.ExitScope();
        return bodyResult;
    }
}
