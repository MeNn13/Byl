using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic.Visitors;

internal class MethodSemantic : BaseSemanticVisitor
{
    private readonly SymbolTable _symbols;
    private readonly Dictionary<string, NamespaceScope> _namespaces;
    private readonly List<string> _imports;
    private readonly ISemanticVisitor _expressionVisitor;
    private MethodDeclaration? _currentFunction;

    public MethodSemantic(
        SymbolTable symbols,
        Dictionary<string, NamespaceScope> namespaces, 
        List<string> imports)
    {
        _symbols = symbols;
        _namespaces = namespaces;
        _imports = imports;
        _expressionVisitor = new ExpressionSemantic(symbols, namespaces, imports);
    }

    public override SemanticResult Visit(MethodDeclaration node)
    {
        var funcSymbol = new MethodSymbol(node);
        if (!_symbols.TryAddSymbol(funcSymbol))
            return SemanticResult.Error($"Метод '{node.Name}' уже определена");

        _currentFunction = node;
        _symbols.EnterScope();

        foreach (var param in node.Parameters)
        {
            var result = param.Accept(this);
            if (!result.IsValid) return result;
        }

        var bodyResult = node.Body.Accept(_expressionVisitor);
        _symbols.ExitScope();
        _currentFunction = null;

        return bodyResult;
    }
    public override SemanticResult Visit(ParameterNode node)
    {
        var varSymbol = new VariableSymbol(node.Name, node.Type.TypeName, node.Line);
        if (!_symbols.TryAddSymbol(varSymbol))
            return SemanticResult.Error($"Параметр '{node.Name}' уже существует");

        return SemanticResult.Success();
    }
    public override SemanticResult Visit(ReturnStatement node)
    {
        if (node.Value != null && _currentFunction?.ReturnType == null)
            return SemanticResult.Error("Функция не должна возвращать значение");

        if (node.Value == null && _currentFunction?.ReturnType != null)
            return SemanticResult.Error("Функция должна возвращать значение");

        if (node.Value != null && _currentFunction?.ReturnType != null)
        {
            var valueResult = node.Value.Accept(_expressionVisitor);
            if (!valueResult.IsValid) return valueResult;

            var valueType = GetExpressionType(node.Value);
            var returnType = _currentFunction.ReturnType.TypeName;

            if (!TypeSystem.AreTypesCompatible(returnType, valueType))
                return SemanticResult.Error($"Нельзя вернуть {valueType} из функции типа {returnType}");
        }

        return SemanticResult.Success();
    }

    private string? GetExpressionType(ExpressionNode expr) => expr switch
    {
        LiteralExpression lit => lit.Value switch
        {
            int => "цел",
            bool => "лог",
            string => "стр",
            _ => "общ"
        },
        _ => null
    };
}
