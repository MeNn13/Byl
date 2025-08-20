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
    private readonly Dictionary<string, NamespaceScope> _namespaces = new();
    private NamespaceScope _currentNamespace = new("");
    private FunctionDeclaration? _currentFunction;
    private readonly List<string> _imports = new();

    public SemanticResult Visit(ProgramNode node)
    {
        _symbols.EnterScope();

        // Собираем импорты
        foreach (var declaration in node.Declarations.OfType<ImportDeclaration>())
        {
            _imports.Add(declaration.Namespace);
        }

        // Регистрируем пространства имен
        foreach (var declaration in node.Declarations.OfType<NamespaceDeclaration>())
        {
            RegisterNamespace(declaration);
        }

        // Анализируем все объявления
        foreach (var declaration in node.Declarations)
        {
            var result = declaration.Accept(this);
            if (!result.IsValid) return result;
        }

        // Проверяем наличие главной функции
        var mainFunc = FindFunction("главный");
        if (mainFunc == null)
            return SemanticResult.Error("Не найдена функция 'главный'");

        _symbols.ExitScope();
        return SemanticResult.Success();
    }
    public SemanticResult Visit(NamespaceDeclaration node)
    {
        var oldNamespace = _currentNamespace;
        _currentNamespace = _namespaces[node.Name];

        foreach (var member in node.Members)
        {
            var result = member.Accept(this);
            if (!result.IsValid) return result;
        }

        _currentNamespace = oldNamespace;
        return SemanticResult.Success();
    }
    public SemanticResult Visit(ImportDeclaration node)
    {
        // Импорты уже обработаны в Visit(ProgramNode)
        return SemanticResult.Success();
    }
    public SemanticResult Visit(ExpressionStatement node)
    {
        return node.Expression.Accept(this);
    }
    public SemanticResult Visit(FunctionDeclaration node)
    {
        // Регистрируем функцию в SymbolTable
        var funcSymbol = new FunctionSymbol(node); // Передаем declaration
        if (!_symbols.TryAddSymbol(funcSymbol))
            return SemanticResult.Error($"Функция '{node.Name}' уже определена");

        _currentFunction = node;
        _symbols.EnterScope();

        // Добавляем параметры
        foreach (var param in node.Parameters)
        {
            var paramResult = param.Accept(this);
            if (!paramResult.IsValid) return paramResult;
        }

        // Проверяем тело
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
        // Ищем переменную в текущей области
        var variable = _symbols.Lookup<VariableSymbol>(node.Name);
        if (variable != null) return SemanticResult.Success();

        // Ищем в текущем пространстве имен
        var nsVariable = _currentNamespace.GetVariable(node.Name);
        if (nsVariable != null) return SemanticResult.Success();

        // Ищем в импортированных пространствах
        foreach (var import in _imports)
        {
            if (_namespaces.TryGetValue(import, out var ns))
            {
                var importedVar = ns.GetVariable(node.Name);
                if (importedVar != null) return SemanticResult.Success();
            }
        }

        return SemanticResult.Error($"Неизвестная переменная '{node.Name}'");
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
    public SemanticResult Visit(FunctionCallExpression node)
    {
        // Ищем функцию с учетом пространств имен
        var function = FindFunction(node.FunctionName);
        if (function == null)
            return SemanticResult.Error($"Неизвестная функция '{node.FunctionName}'");

        // Проверяем количество аргументов
        if (node.Arguments.Count != function.Parameters.Count)
            return SemanticResult.Error($"Ожидалось {function.Parameters.Count} аргументов, получено {node.Arguments.Count}");

        // Проверяем типы аргументов
        for (int i = 0; i < node.Arguments.Count; i++)
        {
            var argResult = node.Arguments[i].Accept(this);
            if (!argResult.IsValid) return argResult;

            var argType = GetExpressionType(node.Arguments[i]);
            var paramType = function.Parameters[i].Type.TypeName;

            if (!TypeSystem.AreTypesCompatible(paramType, argType))
                return SemanticResult.Error($"Несовместимый тип аргумента {i + 1}");
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
        // Ищем в текущей области
        var variable = _symbols.Lookup<VariableSymbol>(varExpr.Name);
        if (variable != null) return variable.Type;

        // Ищем в текущем пространстве имен
        var nsVariable = _currentNamespace.GetVariable(varExpr.Name);
        if (nsVariable != null) return nsVariable.Type;

        // Ищем в импортированных пространствах
        foreach (var import in _imports)
        {
            if (_namespaces.TryGetValue(import, out var ns))
            {
                var importedVar = ns.GetVariable(varExpr.Name);
                if (importedVar != null) return importedVar.Type;
            }
        }

        return null;
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
    private void RegisterNamespace(NamespaceDeclaration ns)
    {
        if (!_namespaces.ContainsKey(ns.Name))
        {
            _namespaces[ns.Name] = new NamespaceScope(ns.Name);
        }

        var scope = _namespaces[ns.Name];

        foreach (var member in ns.Members)
        {
            if (member is FunctionDeclaration func)
            {
                scope.AddFunction(func);
            }
        }
    }
    private FunctionDeclaration? FindFunction(string name)
    {
        // Ищем в текущем пространстве имен
        if (_currentNamespace.TryGetFunction(name, out var func))
            return func;

        // Ищем в импортированных пространствах
        foreach (var import in _imports)
        {
            if (_namespaces.TryGetValue(import, out var ns) && ns.TryGetFunction(name, out func))
                return func;
        }

        // Ищем в глобальной области
        var globalFunc = _symbols.Lookup<FunctionSymbol>(name);
        return globalFunc?.Declaration;

    }
    private T? FindInImportedNamespaces<T>(string name) where T : Symbol
    {
        foreach (var import in _imports)
        {
            if (_namespaces.TryGetValue(import, out var ns))
            {
                var symbol = ns.Lookup<T>(name);
                if (symbol != null) return symbol;
            }
        }
        return null;
    }

}
