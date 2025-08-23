using System.Text;
using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors;

public class CodeGenerator : IAstVisitor<string>
{
    private readonly Dictionary<string, string> _importedNamespaces = new();
    private string _currentNamespacePrefix = "";
    private readonly StringBuilder _functionDeclarations = new();
    private readonly StringBuilder _mainCode = new();
    private readonly Dictionary<string, string> _functionPrefixes = new();

    public string Visit(TypeNode node)
    {
        throw new NotImplementedException();
    }
    public string Visit(ProgramNode node)
    {
        var sb = new StringBuilder();
        sb.AppendLine("#include <stdio.h>");
        sb.AppendLine("#include <string.h>");
        sb.AppendLine();

        // Собираем импорты и запоминаем префиксы
        foreach (var declaration in node.Declarations.OfType<ImportDeclaration>())
        {
            string prefix = GenerateNamespacePrefix(declaration.Namespace);
            _importedNamespaces[declaration.Namespace] = prefix;
        }

        // Генерируем объявления функций
        foreach (var declaration in node.Declarations.OfType<NamespaceDeclaration>())
        {
            _functionDeclarations.Append(declaration.Accept(this));
        }

        // Генерируем главную функцию
        var mainFunc = node.Declarations.OfType<MethodDeclaration>()
            .FirstOrDefault(f => f.Name == "главный");

        if (mainFunc != null)
        {
            _mainCode.AppendLine("int main() {");
            _mainCode.Append(mainFunc.Body.Accept(this));
            _mainCode.AppendLine("    return 0;");
            _mainCode.AppendLine("}");
        }

        // Собираем итоговый код
        sb.Append(_functionDeclarations);
        sb.AppendLine();
        sb.Append(_mainCode);

        return sb.ToString();
    }
    public string Visit(NamespaceDeclaration node)
    {
        var oldPrefix = _currentNamespacePrefix;
        _currentNamespacePrefix = GenerateNamespacePrefix(node.Name);

        var code = new StringBuilder();
        foreach (var member in node.Members)
        {
            code.Append(member.Accept(this));
        }

        _currentNamespacePrefix = oldPrefix;
        return code.ToString();
    }
    public string Visit(ImportDeclaration node)
    {
        return string.Empty; // Импорты не генерируют код
    }
    public string Visit(MethodDeclaration node)
    {
        var functionName = Transliterate(node.Name); // Транслитерируем имя функции
        var fullFunctionName = _currentNamespacePrefix + functionName;

        var returnType = GetCType(node.ReturnType);

        var sb = new StringBuilder();
        sb.Append($"{returnType} {fullFunctionName}(");

        for (int i = 0; i < node.Parameters.Count; i++)
        {
            if (i > 0) sb.Append(", ");
            var paramName = Transliterate(node.Parameters[i].Name); // Транслитерируем параметры
            sb.Append($"{GetCType(node.Parameters[i].Type)} {paramName}");
        }

        sb.AppendLine(") {");
        sb.Append(node.Body.Accept(this));
        sb.AppendLine("}");
        sb.AppendLine();

        return sb.ToString();
    }
    public string Visit(ParameterNode node)
    {
        throw new NotImplementedException();
    }
    public string Visit(BlockStatement node)
    {
        var sb = new StringBuilder();
        foreach (var stmt in node.Statements)
        {
            sb.Append(stmt.Accept(this));
        }
        return sb.ToString(); ;
    }
    public string Visit(PrintStatement node)
    {
        // Если выражение - вызов функции, обрабатываем особо
        if (node.Expression is FunctionCallExpression funcCall)
        {
            return $"    printf(\"%d\\n\", {funcCall.Accept(this)});\n";
        }

        return $"    printf(\"%d\\n\", {node.Expression.Accept(this)});\n";
    }
    public string Visit(AssignStatement node)
    {
        var varName = Transliterate(node.VariableName);
        return $"    {varName} = {node.Value.Accept(this)};\n";
    }
    public string Visit(IfStatement node)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    if ({node.Condition.Accept(this)}) {{");
        sb.Append(node.ThenBranch.Accept(this));
        sb.AppendLine("    }");

        if (node.ElseBranch != null)
        {
            sb.AppendLine("    else {");
            sb.Append(node.ElseBranch.Accept(this));
            sb.AppendLine("    }");
        }

        return sb.ToString();
    }
    public string Visit(WhileStatement node)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"    while ({node.Condition.Accept(this)}) {{");
        sb.Append(node.Body.Accept(this));
        sb.AppendLine("    }");
        return sb.ToString();
    }
    public string Visit(ReturnStatement node)
    {
        if (node.Value is FunctionCallExpression funcCall)
        {
            return $"    return {funcCall.Accept(this)};\n";
        }

        return node.Value != null
            ? $"    return {node.Value.Accept(this)};\n"
            : "    return;\n";
    }
    public string Visit(VariableDeclarationStatement node)
    {
        var varName = Transliterate(node.VariableName);
        var init = node.Initializer != null ? $" = {node.Initializer.Accept(this)}" : "";
        return $"    int {varName}{init};\n";
    }
    public string Visit(BinaryExpression node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        return node.Operator.Type switch
        {
            TokenType.Plus => $"({left} + {right})",
            TokenType.Minus => $"({left} - {right})",
            TokenType.Multiply => $"({left} * {right})",
            TokenType.Divide => $"({left} / {right})",
            TokenType.Equal => $"({left} == {right})",
            TokenType.NotEqual => $"({left} != {right})",
            TokenType.GreaterThan => $"({left} > {right})",
            _ => $"({left} {node.Operator.Value} {right})"
        };
    }
    public string Visit(LiteralExpression node)
    {
        if (node.Value is string str && str.StartsWith("%\""))
        {
            return ProcessInterpolatedString(str);
        }

        if (node.Value is string regularStr)
        {
            return $"\"{EscapeString(regularStr)}\"";
        }

        if (node.Value is bool boolVal)
        {
            return boolVal ? "1" : "0";
        }

        return node.Value?.ToString() ?? "0";
    }
    public string Visit(VariableExpression node)
    {
        return Transliterate(node.Name);
    }
    public string Visit(UnaryExpression node)
    {
        var right = node.Right.Accept(this);
        return node.Operator.Type switch
        {
            TokenType.Minus => $"(-{right})",
            TokenType.Not => $"(!{right})",
            _ => $"({node.Operator.Value}{right})"
        };
    }
    public string Visit(InterpolatedStringExpression node)
    {
        // Этот метод больше не используется
        // Интерполированные строки теперь обрабатываются как LiteralExpression
        return "\"Интерполяция обрабатывается в LiteralExpression\\n\"";
    }
    public string Visit(ExpressionStatement node)
    {
        if (node.Expression is FunctionCallExpression funcCall)
        {
            return $"    {funcCall.Accept(this)};\n";
        }

        return $"    {node.Expression.Accept(this)};\n";
    }
    public string Visit(FunctionCallExpression node)
    {
        var args = string.Join(", ", node.Arguments.Select(arg => arg.Accept(this)));

        // Определяем полное имя функции с префиксом пространства имен
        string fullFunctionName = GetFullFunctionName(node.FunctionName);

        return $"{fullFunctionName}({args})";
    }

    private string EscapeString(string str)
    {
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n")
                  .Replace("\t", "\\t")
                  .Replace("\r", "\\r")
                  .Replace("%", "%%");
    }
    private string ProcessInterpolatedString(string interpolatedStr)
    {
        // Убираем %" и закрывающую "
        var content = interpolatedStr.Substring(2, interpolatedStr.Length - 3);

        var formatParts = new List<string>();
        var args = new List<string>();
        var currentText = new StringBuilder();
        var inExpression = false;

        formatParts.Add("\"");

        for (int i = 0; i < content.Length; i++)
        {
            if (content[i] == '{' && !inExpression)
            {
                // Начало выражения
                if (currentText.Length > 0)
                {
                    formatParts.Add(EscapeString(currentText.ToString()));
                    currentText.Clear();
                }
                inExpression = true;
            }
            else if (content[i] == '}' && inExpression)
            {
                // Конец выражения
                if (currentText.Length > 0)
                {
                    formatParts.Add("%d"); // Для чисел
                    args.Add(currentText.ToString()); // Имя переменной
                    currentText.Clear();
                }
                inExpression = false;
            }
            else
            {
                currentText.Append(content[i]);
            }
        }

        // Добавляем оставшийся текст
        if (currentText.Length > 0 && !inExpression)
        {
            formatParts.Add(EscapeString(currentText.ToString()));
        }

        formatParts.Add("\\n\"");

        var formatString = string.Join("", formatParts);

        if (args.Count > 0)
        {
            return $"printf({formatString}, {string.Join(", ", args)})";
        }
        else
        {
            return $"printf({formatString})";
        }
    }
    private string GenerateNamespacePrefix(string namespaceName)
    {
        var transliterated = Transliterate(namespaceName.Replace("::", "_"));
        return transliterated + "_";
    }
    private string GetCType(TypeNode? typeNode)
    {
        if (typeNode == null) return "void";

        return typeNode.TypeName switch
        {
            "цел" => "int",
            "вещ" => "float",
            "лог" => "int",
            "стр" => "char*",
            "общ" => "int",
            _ => "int"
        };
    }
    private string GetFullFunctionName(string functionName)
    {
        if (functionName == "главный") return "main";

        string transliteratedName = Transliterate(functionName);

        // Если функция определена в текущем пространстве имен
        if (_functionPrefixes.TryGetValue(functionName, out var prefix) && !string.IsNullOrEmpty(prefix))
        {
            return prefix + transliteratedName;
        }

        // Ищем в импортированных пространствах
        foreach (var import in _importedNamespaces)
        {
            // Предполагаем, что импортированные функции имеют префикс
            return import.Value + transliteratedName;
        }

        // Если не нашли, возвращаем как есть (глобальная функция)
        return transliteratedName;
    }
    private string Transliterate(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var result = new StringBuilder();

        foreach (char c in text)
        {
            string transliterated = c switch
            {
                'а' => "a",
                'б' => "b",
                'в' => "v",
                'г' => "g",
                'д' => "d",
                'е' => "e",
                'ё' => "yo",
                'ж' => "zh",
                'з' => "z",
                'и' => "i",
                'й' => "y",
                'к' => "k",
                'л' => "l",
                'м' => "m",
                'н' => "n",
                'о' => "o",
                'п' => "p",
                'р' => "r",
                'с' => "s",
                'т' => "t",
                'у' => "u",
                'ф' => "f",
                'х' => "h",
                'ц' => "ts",
                'ч' => "ch",
                'ш' => "sh",
                'щ' => "sch",
                'ъ' => "",
                'ы' => "y",
                'ь' => "",
                'э' => "e",
                'ю' => "yu",
                'я' => "ya",
                'А' => "A",
                'Б' => "B",
                'В' => "V",
                'Г' => "G",
                'Д' => "D",
                'Е' => "E",
                'Ё' => "Yo",
                'Ж' => "Zh",
                'З' => "Z",
                'И' => "I",
                'Й' => "Y",
                'К' => "K",
                'Л' => "L",
                'М' => "M",
                'Н' => "N",
                'О' => "O",
                'П' => "P",
                'Р' => "R",
                'С' => "S",
                'Т' => "T",
                'У' => "U",
                'Ф' => "F",
                'Х' => "H",
                'Ц' => "Ts",
                'Ч' => "Ch",
                'Ш' => "Sh",
                'Щ' => "Sch",
                'Ъ' => "",
                'Ы' => "Y",
                'Ь' => "",
                'Э' => "E",
                'Ю' => "Yu",
                'Я' => "Ya",
                _ => c.ToString()
            };

            result.Append(transliterated);
        }

        return result.ToString();
    }
}
