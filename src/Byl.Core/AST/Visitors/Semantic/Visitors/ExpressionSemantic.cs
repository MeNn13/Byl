using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic.Visitors;

internal class ExpressionSemantic : BaseSemanticVisitor
{
    private readonly SymbolTable _symbols;
    private readonly Dictionary<string, NamespaceScope> _namespaces;
    private readonly List<string> _imports;

    public ExpressionSemantic(
        SymbolTable symbols,
        Dictionary<string, NamespaceScope> namespaces, 
        List<string> imports)
    {
        _symbols = symbols;
        _namespaces = namespaces;
        _imports = imports;
    }

    public override SemanticResult Visit(BlockStatement node)
    {
        _symbols.EnterScope();

        foreach (var stmt in node.Statements)
        {
            var result = stmt.Accept(this);
            if (!result.IsValid) return result;
        }

        _symbols.ExitScope();
        return SemanticResult.Success();
    }
    public override SemanticResult Visit(VariableDeclarationStatement node)
    {
        if (_symbols.Lookup(node.VariableName) != null)
            return SemanticResult.Error($"Переменная '{node.VariableName}' уже определена");

        var varSymbol = new VariableSymbol(node.VariableName, node.Type.TypeName, node.Line);
        if (!_symbols.TryAddSymbol(varSymbol))
            return SemanticResult.Error($"Не удалось добавить переменную '{node.VariableName}'");

        return node.Initializer?.Accept(this) ?? SemanticResult.Success();
    }
    public override SemanticResult Visit(AssignStatement node)
    {
        var variableSymbol = _symbols.Lookup<VariableSymbol>(node.VariableName);
        if (variableSymbol == null)
            return SemanticResult.Error($"Неизвестная переменная '{node.VariableName}'");

        var valueResult = node.Value.Accept(this);
        if (!valueResult.IsValid) return valueResult;

        var valueType = GetExpressionType(node.Value);
        if (valueType == null)
            return SemanticResult.Error("Не удалось определить тип выражения");

        if (!TypeSystem.AreTypesCompatible(variableSymbol.Type, valueType))
            return SemanticResult.Error($"Нельзя присвоить {valueType} переменной типа {variableSymbol.Type}");

        return SemanticResult.Success();
    }
    public override SemanticResult Visit(VariableExpression node)
    {
        if (_symbols.Lookup<VariableSymbol>(node.Name) != null)
            return SemanticResult.Success();

        // Проверка в namespace и импортах...
        return SemanticResult.Error($"Неизвестная переменная '{node.Name}'");
    }
    public override SemanticResult Visit(BinaryExpression node)
    {
        var leftResult = node.Left.Accept(this);
        if (!leftResult.IsValid) return leftResult;

        var rightResult = node.Right.Accept(this);
        if (!rightResult.IsValid) return rightResult;

        var leftType = GetExpressionType(node.Left);
        var rightType = GetExpressionType(node.Right);

        if (leftType is null || rightType is null)
            return SemanticResult.Error("Не удалось определить тип выражения");

        if (!TypeSystem.AreTypesCompatible(leftType, rightType))
            return SemanticResult.Error($"Несовместимые типы: {leftType} и {rightType}");

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
        VariableExpression varExpr => GetVariableType(varExpr),
        _ => null
    };
    private string? GetVariableType(VariableExpression varExpr)
    {
        var variable = _symbols.Lookup<VariableSymbol>(varExpr.Name);
        return variable?.Type;
    }
}
