using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.AST.Visitors.Semantic.Symbols;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors.Semantic;

public class SemanticAnalyzer : IAstVisitor<SemanticResult>
{
    private readonly SymbolTable _symbols = new();
    private FunctionDeclaration? _currentFunction;

    public SemanticResult Visit(ProgramNode node)
    {
        _symbols.EnterScope(); // Глобальная область

        var mainFunc = node.Functions.FirstOrDefault(f => f.Name == "главный");
        if (mainFunc is null)
            return SemanticResult.Error("Не найдена функция 'главный'");

        // Добавляем все функции в глобальную область
        foreach (var func in node.Functions)
        {
            var funcSymbol = new FunctionSymbol(
                func.Name,
                func.Parameters,
                func.ReturnType,
                func.Line
            );

            if (!_symbols.TryAddSymbol(funcSymbol))
                return SemanticResult.Error($"Функция '{func.Name}' уже определена");

            //Проверка функций
            var result = func.Accept(this);
            if (!result.IsValid) return result;
        }

        _symbols.ExitScope();
        return SemanticResult.Success();
    }
    public SemanticResult Visit(FunctionDeclaration node)
    {
        _currentFunction = node;
        _symbols.EnterScope(); // Новая область видимости для функции

        // Добавляем параметры в scope
        foreach (var param in node.Parameters)
        {
            var paramResult = param.Accept(this);
            if (!paramResult.IsValid) return paramResult;
        }

        // Проверяем тело функции
        var bodyResult = node.Body.Accept(this);

        _symbols.ExitScope();
        _currentFunction = null;

        return bodyResult;
    }
    public SemanticResult Visit(ParameterNode node)
    {
        var varSymbol = new VariableSymbol(node.Name, node.Type.TypeName, node.Line);
        if (!_symbols.TryAddSymbol(varSymbol))
            return SemanticResult.Error($"Параметр '{node.Name}' уже существует");

        return SemanticResult.Success();
    }
    public SemanticResult Visit(BlockStatement node)
    {
        _symbols.EnterScope(); // Новая область видимости для блока

        foreach (var stmt in node.Statements)
        {
            var result = stmt.Accept(this);
            if (!result.IsValid) return result;
        }

        _symbols.ExitScope();
        return SemanticResult.Success();
    }
    public SemanticResult Visit(PrintStatement node)
    {
        return node.Expression.Accept(this);
    }
    public SemanticResult Visit(AssignStatement node)
    {
        // Проверяем, объявлена ли переменная
        var variableSymbol = _symbols.Lookup<VariableSymbol>(node.VariableName);
        if (variableSymbol == null)
            return SemanticResult.Error($"Неизвестная переменная '{node.VariableName}'");

        // Проверяем выражение
        var valueResult = node.Value.Accept(this);
        if (!valueResult.IsValid) return valueResult;

        // Проверяем совместимость типов
        var valueType = GetExpressionType(node.Value);
        if (valueType == null)
            return SemanticResult.Error("Не удалось определить тип выражения");

        if (!TypeSystem.AreTypesCompatible(variableSymbol.Type, valueType))
        {
            return SemanticResult.Error(
                $"Нельзя присвоить {valueType} переменной типа {variableSymbol.Type}"
            );
        }

        return SemanticResult.Success();
    }
    public SemanticResult Visit(BinaryExpression node)
    {
        // Проверяем левую часть
        var leftResult = node.Left.Accept(this);
        if (!leftResult.IsValid) return leftResult;

        // Проверяем правую часть
        var rightResult = node.Right.Accept(this);
        if (!rightResult.IsValid) return rightResult;

        // Проверяем типы
        var leftType = GetExpressionType(node.Left);
        var rightType = GetExpressionType(node.Right);

        if (leftType is null || rightType is null)
            return SemanticResult.Error("Не удалось определить тип выражения");

        // Специальная проверка для конкатенации строк
        if (node.Operator.Type == TokenType.Plus)
        {
            if ((leftType == "стр" || rightType == "стр") &&
                TypeSystem.AreTypesCompatible(leftType, rightType))
            {
                return SemanticResult.Success();
            }
        }

        if (!TypeSystem.AreTypesCompatible(leftType, rightType))
            return SemanticResult.Error($"Несовместимые типы: {leftType} и {rightType}");

        if (!TypeSystem.IsOperationAllowed(node.Operator.Type, leftType) ||
            !TypeSystem.IsOperationAllowed(node.Operator.Type, rightType))
        {
            return SemanticResult.Error(
                $"Операция '{node.Operator.Value}' не поддерживается для типов {leftType} и {rightType}"
            );
        }

        return SemanticResult.Success();
    }
    public SemanticResult Visit(LiteralExpression node)
    {
        // Литералы всегда валидны
        return SemanticResult.Success();
    }
    public SemanticResult Visit(VariableExpression node)
    {
        // Проверяем, объявлена ли переменная
        if (_symbols.Lookup(node.Name) == null)
            return SemanticResult.Error($"Неизвестная переменная '{node.Name}'");

        return SemanticResult.Success();
    }
    public SemanticResult Visit(VariableDeclarationStatement node)
    {
        // Проверяем, не объявлена ли переменная уже
        if (_symbols.Lookup(node.VariableName) != null)
            return SemanticResult.Error($"Переменная '{node.VariableName}' уже определена");

        // Добавляем переменную в таблицу символов
        var varSymbol = new VariableSymbol(node.VariableName, node.Type.TypeName, node.Line);
        if (!_symbols.TryAddSymbol(varSymbol))
            return SemanticResult.Error($"Не удалось добавить переменную '{node.VariableName}'");

        // Проверяем инициализатор, если есть
        if (node.Initializer != null)
        {
            return node.Initializer.Accept(this);
        }

        return SemanticResult.Success();
    }
    public SemanticResult Visit(TypeNode node)
    {
        // Типы всегда валидны (проверяются на этапе парсинга)
        return SemanticResult.Success();
    }
    public SemanticResult Visit(IfStatement node)
    {
        // Проверяем условие
        var conditionResult = node.Condition.Accept(this);
        if (!conditionResult.IsValid) return conditionResult;

        // Проверяем then ветку
        var thenResult = node.ThenBranch.Accept(this);
        if (!thenResult.IsValid) return thenResult;

        // Проверяем else ветку, если есть
        if (node.ElseBranch != null)
        {
            return node.ElseBranch.Accept(this);
        }

        return SemanticResult.Success();
    }
    public SemanticResult Visit(WhileStatement node)
    {
        // Проверяем условие
        var conditionResult = node.Condition.Accept(this);
        if (!conditionResult.IsValid) return conditionResult;

        // Проверяем тело цикла
        return node.Body.Accept(this);
    }
    public SemanticResult Visit(ReturnStatement node)
    {
        if (node.Value != null && _currentFunction?.ReturnType == null)
            return SemanticResult.Error("Функция не должна возвращать значение");

        if (node.Value == null && _currentFunction?.ReturnType != null)
            return SemanticResult.Error("Функция должна возвращать значение");

        if (node.Value != null && _currentFunction?.ReturnType != null)
        {
            var valueResult = node.Value.Accept(this);
            if (!valueResult.IsValid) return valueResult;

            var valueType = GetExpressionType(node.Value);
            var returnType = _currentFunction.ReturnType.TypeName;

            if (!TypeSystem.AreTypesCompatible(returnType, valueType))
                return SemanticResult.Error($"Нельзя вернуть {valueType} из функции типа {returnType}");
        }

        return SemanticResult.Success();
    }
    public SemanticResult Visit(UnaryExpression node)
    {
        var rightResult = node.Right.Accept(this);
        if (!rightResult.IsValid) return rightResult;

        var rightType = GetExpressionType(node.Right);
        if (rightType == null)
            return SemanticResult.Error("Не удалось определить тип выражения");

        if (!TypeSystem.IsOperationAllowed(node.Operator.Type, rightType))
            return SemanticResult.Error($"Операция '{node.Operator.Value}' не поддерживается для типа {rightType}");

        return node.Right.Accept(this);
    }
    public SemanticResult Visit(InterpolatedStringExpression node)
    {
        // Проверяем все части интерполированной строки
        foreach (var part in node.Parts)
        {
            var partResult = part.Accept(this);
            if (!partResult.IsValid)
                return partResult;
        }

        return SemanticResult.Success();
    }

    private string? GetExpressionType(ExpressionNode expr) => expr switch
    {
        LiteralExpression lit => GetLiteralType(lit),
        VariableExpression varExpr => GetVariableType(varExpr),
        BinaryExpression binExpr => GetBinaryExpressionType(binExpr),
        UnaryExpression unExpr => GetUnaryExpressionType(unExpr),
        InterpolatedStringExpression => "стр",
        _ => null
    };
    private string GetLiteralType(LiteralExpression lit) => lit.Value switch
    {
        int => "цел",
        bool => "лог",
        string => "стр",
        _ => "общ"
    };
    private string? GetVariableType(VariableExpression varExpr)
    {
        return _symbols.Lookup<VariableSymbol>(varExpr.Name)?.Type;
    }
    private string? GetBinaryExpressionType(BinaryExpression binExpr)
    {
        var leftType = GetExpressionType(binExpr.Left);
        var rightType = GetExpressionType(binExpr.Right);

        if (leftType == null || rightType == null)
            return null;

        return TypeSystem.GetBinaryOperationResultType(leftType, rightType, binExpr.Operator.Type);
    }
    private string? GetUnaryExpressionType(UnaryExpression unExpr)
    {
        var rightType = GetExpressionType(unExpr.Right);
        if (rightType == null) return null;

        // Для унарных операций
        return unExpr.Operator.Type switch
        {
            TokenType.Minus when rightType == "цел" => "цел",
            TokenType.Minus => "общ",
            TokenType.Not when rightType == "лог" => "лог",
            TokenType.Not => "общ",
            _ => rightType
        };
    }
}
